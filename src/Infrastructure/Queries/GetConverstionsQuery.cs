using Application.Dto;
using Application.Interfaces;
using Azure.Storage.Blobs;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;

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

    public async Task<List<Conversation>> GetAllConversationsAsync()
    {
        var blobContainerClient = blobServiceClient.GetBlobContainerClient(BlobContainerName);
        var conversations = new List<Conversation>();

        await foreach (var blobItem in blobContainerClient.GetBlobsAsync())
        {
            try
            {
                var blobName = blobItem.Name;
                var serializedConversation = await storageRepository.DownloadTextBlobAsync(blobName, BlobContainerName);

                var conversation = JsonSerializer.Deserialize<Conversation>(serializedConversation, SerializerOptions);

                Verify.NotNull(conversation);

                conversations.Add(conversation);
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Failed to load or deserialize conversation blob {blobName}", blobItem.Name);
                throw;
            }
        }

        return conversations;
    }

    public async Task<Conversation> LoadAsync(Guid conversationId)
    {
        Verify.NotEmpty(conversationId);

        var blobContainerClient = blobServiceClient.GetBlobContainerClient(BlobContainerName);
        var blobClient = blobContainerClient.GetBlobClient(GetBlobName(conversationId));

        try
        {
            var serializeChatHistory =
                await storageRepository.DownloadTextBlobAsync(GetBlobName(conversationId), BlobContainerName);

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
            throw;
        }
    }

    private static string GetBlobName(Guid threadId)
    {
        return $"{threadId}.json";
    }
}