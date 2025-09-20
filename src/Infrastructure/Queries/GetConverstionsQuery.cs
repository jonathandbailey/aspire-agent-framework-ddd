using Application.Dto;
using Application.Interfaces;
using Azure;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text.Json.Serialization;
using Infrastructure.Exceptions;

namespace Infrastructure.Queries;

public class ConversationQuery(BlobServiceClient blobServiceClient, IAzureStorageRepository storageRepository, ILogger<ConversationQuery> logger) : IConversationQuery
{
    private const string BlobContainerName = "user-conversation-history";
  
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters = { new JsonStringEnumConverter() }
    };

    public async Task<List<Conversation>> GetAllConversationsAsync(Guid userId)
    {
        var blobContainerClient = blobServiceClient.GetBlobContainerClient(BlobContainerName);
        var conversations = new List<Conversation>();

        var prefix = $"user-{userId}/conversations/";
       
        try
        {
            await foreach (var blobItem in blobContainerClient.GetBlobsAsync(prefix: prefix))
            {

                var blobName = blobItem.Name;
                var serializedConversation =
                    await storageRepository.DownloadTextBlobAsync(blobName, BlobContainerName);

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
                await storageRepository.DownloadTextBlobAsync(GetFullBlobName(userId, conversationId), BlobContainerName);

            var conversation = JsonSerializer.Deserialize<Conversation>(serializeChatHistory, SerializerOptions);

            Verify.NotNull(conversation);

            return conversation;
        }
        catch (JsonException jsonEx)
        {
            logger.LogError(jsonEx, "Failed to deserialize chat history for thread {threadId}", conversationId);
            throw;
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "An exception has occurred while trying to Load the Agent Chat History, {threadId}", conversationId);
            throw new InfrastructureException("Infrastructure services are not available.", exception);
        }
    }

    private static string GetFullBlobName(Guid userId, Guid conversationId)
    {
        return $"user-{userId}/conversations/conversation-{conversationId}.json";
    }
}