using System.Text.Json;
using FluentValidation;
using Infrastructure.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Api.Middleware;

public class GlobalExceptionHandler : IMiddleware, IExceptionHandler
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            await Handle(context, exception, CancellationToken.None);
        }
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        await Handle(httpContext, exception, cancellationToken);

        return true;
    }


    private static ProblemDetails CreateProblemDetails(Exception exception, HttpContext context)
    {
        return exception switch
        {
            ConversationRepositoryNotAvailableException => new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
                Title = "Application Error",
                Status = StatusCodes.Status500InternalServerError,
                Detail = "The application is currently unable to access conversation data. Please try again later."
            },
            ConversationInfrastructureException => new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
                Title = "Application Error",
                Status = StatusCodes.Status500InternalServerError,
                Detail = "The current conversation cannot be processed."
            },

            InfrastructureException => new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
                Title = "Application Error",
                Status = StatusCodes.Status500InternalServerError,
                Detail = "Applications Services are not available. Please try again later."
            },

            ValidationException => 
                new ValidationProblemDetails(GetValidationErrors(exception))
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                Status = StatusCodes.Status400BadRequest,
                Detail = exception.Message
            },
        
            _ => new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
                Title = "Application Error",
                Status = StatusCodes.Status500InternalServerError,
                Detail = "Applications Services are not available. Please try again later."
            }
        };
    }

    private static Dictionary<string, string[]> GetValidationErrors(Exception exception)
    {
        if (exception is not ValidationException validationException)
        {
            throw new InvalidOperationException("Exception must be of type ValidationException.");
        }

        var errors = validationException.Errors
            .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
            .ToDictionary(failureGroup => failureGroup.Key, failureGroup => failureGroup.ToArray());
       
        return errors;
    }

  
    private static async Task Handle(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var problemDetails = CreateProblemDetails(exception, httpContext);

        httpContext.Response.ContentType = "application/problem+json";
        httpContext.Response.StatusCode = problemDetails.Status ?? StatusCodes.Status500InternalServerError;

        var problemDetailsJson = JsonSerializer.Serialize(problemDetails);
        await httpContext.Response.WriteAsync(problemDetailsJson, cancellationToken: cancellationToken);
    }
}