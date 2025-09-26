using Application.Dto;
using Application.Interfaces;
using Azure;
using Azure.Storage.Blobs;
using Infrastructure.Exceptions;
using Infrastructure.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Infrastructure.Queries;

public class ConversationQuerieses(BlobServiceClient blobServiceClient, IOptions<AzureStorageSettings> settings, IAzureStorageRepository storageRepository, ILogger<ConversationQuerieses> logger) : IConversationQueries
{
  
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters = { new JsonStringEnumConverter() }
    };

    private readonly AzureStorageSettings _settings = settings.Value;

    public async Task<List<ConversationSummaryItem>> GetConversationSummaries(Guid userId)
    {
        var conversations = await GetAllConversationsAsync(userId);

        return conversations.Select(conversation => new ConversationSummaryItem(conversation.Id, conversation.Name)).ToList();
    }

    public async Task<List<Conversation>> GetAllConversationsAsync(Guid userId)
    {
        var blobContainerClient = blobServiceClient.GetBlobContainerClient(_settings.ConversationBlobContainerName);
        var conversations = new List<Conversation>();

        var prefix = _settings.ConversationsPrefix.Replace("{userId}", userId.ToString());

        try
        {
            await foreach (var blobItem in blobContainerClient.GetBlobsAsync(prefix: prefix))
            {
                var blobName = blobItem.Name;
                var serializedConversation =
                    await storageRepository.DownloadTextBlobAsync(blobName, _settings.ConversationBlobContainerName);

                var conversation =
                    JsonSerializer.Deserialize<Conversation>(serializedConversation, SerializerOptions);

                Verify.NotNull(conversation);

                conversations.Add(conversation);
            }
        }
        catch (RequestFailedException exception)
        {
            logger.LogError(exception,
                "Failed to load conversations for user {userId} : {ErrorCode}", userId, exception.ErrorCode);
            
            throw new ConversationRepositoryNotAvailableException("Conversation Data is not available.", exception);
        }
        catch (Exception exception)
        {
            logger.LogError(exception,
                "Unknown Exception when loading conversations for user {userId}", userId);
            
            throw new InfrastructureException("Infrastructure services are not available.", exception);
        }
  
        return conversations;
    }

    public async Task<Conversation> LoadAsync(Guid userId, Guid conversationId)
    {
        Verify.NotEmpty(conversationId);
        Verify.NotEmpty(userId);
      
        try
        {
            var serializeChatHistory =
                await storageRepository.DownloadTextBlobAsync(GetFullBlobName(userId, conversationId), _settings.ConversationBlobContainerName);

            var conversation = JsonSerializer.Deserialize<Conversation>(serializeChatHistory, SerializerOptions);

            Verify.NotNull(conversation);

            return conversation;
        }
        catch (JsonException jsonEx)
        {
            logger.LogError(jsonEx, "Failed to deserialize conversation {conversationId} for user {userId}", conversationId, userId);
            throw new ConversationInfrastructureException("There is an issue processing this conversation.", jsonEx);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "An exception has occurred while trying to Load the Agent Chat History, {threadId}", conversationId);
            throw new InfrastructureException("Infrastructure services are not available.", exception);
        }
    }

    private string GetFullBlobName(Guid userId, Guid conversationId)
    {
        return _settings.ConversationBlobNameFormat
            .Replace("{userId}", userId.ToString())
            .Replace("{conversationId}", conversationId.ToString());
    }
}