using System.Text.Json;
using Infrastructure.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api.Middleware;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IMiddleware
{
    private readonly ILogger<GlobalExceptionHandler> _logger = logger;

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "An unhandled exception occurred.");

            var problemDetails = CreateProblemDetails(exception, context);

            context.Response.ContentType = "application/problem+json";
            context.Response.StatusCode = problemDetails.Status ?? StatusCodes.Status500InternalServerError;

            var problemDetailsJson = JsonSerializer.Serialize(problemDetails);
            await context.Response.WriteAsync(problemDetailsJson);
        }
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

            InfrastructureException => new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
                Title = "Application Error",
                Status = StatusCodes.Status500InternalServerError,
                Detail = "Applications Services are not available. Please try again later."
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
}