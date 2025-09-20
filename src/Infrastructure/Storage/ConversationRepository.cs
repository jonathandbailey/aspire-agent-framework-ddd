using System.Text.Json;
using System.Text.Json.Serialization;
using Application.Interfaces;
using Azure.Storage.Blobs;
using Domain.Conversations;
using Infrastructure.Dto;
using Infrastructure.Extensions;
using Microsoft.Extensions.Logging;


namespace Infrastructure.Storage;

public class ConversationRepository(BlobServiceClient blobServiceClient, IAzureStorageRepository storageRepository, ILogger<ConversationRepository> logger) : IConversationRepository
{
    private const string BlobContainerName = "user-conversation-history";
    private const string ApplicationJsonContentType = "application/json";

    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters = { new JsonStringEnumConverter() }
    };

    public async Task SaveAsync(Guid conversationId, Conversation conversation)
    {
        Verify.NotNull(conversation);

        try
        {
            var serializedConversation = JsonSerializer.Serialize(conversation.Map(), SerializerOptions);

            await storageRepository.UploadTextBlobAsync(GetBlobName(conversationId), BlobContainerName, serializedConversation, ApplicationJsonContentType);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "An exception has occurred while trying to Save the Agent Chat History , {threadId}", conversationId);
            throw;
        }
    }

    public async Task<List<Conversation>> GetAllConversations()
    {
        var blobContainerClient = blobServiceClient.GetBlobContainerClient(BlobContainerName);
        var conversations = new List<Conversation>();

        await foreach (var blobItem in blobContainerClient.GetBlobsAsync())
        {
            try
            {
                var blobName = blobItem.Name;
                var serializedConversation = await storageRepository.DownloadTextBlobAsync(blobName, BlobContainerName);

                var conversation = JsonSerializer.Deserialize<ConversationDto>(serializedConversation, SerializerOptions);

                Verify.NotNull(conversation);

                conversations.Add(conversation.Map());
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
        Verify.NotNull(conversationId);
   
        try
        {
            var serializeChatHistory =
                await storageRepository.DownloadTextBlobAsync(GetBlobName(conversationId), BlobContainerName);

            var conversation = JsonSerializer.Deserialize<ConversationDto>(serializeChatHistory, SerializerOptions);

            Verify.NotNull(conversation);

            return conversation.Map();
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
