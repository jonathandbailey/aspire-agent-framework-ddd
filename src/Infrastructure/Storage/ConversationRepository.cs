using System.Text.Json;
using System.Text.Json.Serialization;
using Application.Interfaces;
using Domain.Conversations;
using Infrastructure.Dto;
using Infrastructure.Extensions;
using Microsoft.Extensions.Logging;


namespace Infrastructure.Storage;

public class ConversationRepository(IAzureStorageRepository storageRepository, ILogger<ConversationRepository> logger) : IConversationRepository
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

    public async Task SaveAsync(Conversation conversation)
    {
        Verify.NotNull(conversation);

        try
        {
            var serializedConversation = JsonSerializer.Serialize(conversation.Map(), SerializerOptions);

            await storageRepository.UploadTextBlobAsync(GetFullBlobName(conversation.UserId, conversation.Id), BlobContainerName, serializedConversation, ApplicationJsonContentType);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "An exception has occurred while trying to Save the Agent Chat History , {Id}", conversation.Id);
            throw;
        }
    }

    public async Task<Conversation> LoadAsync(Guid userId, Guid conversationId)
    {
        Verify.NotNull(conversationId);

        try
        {
            var serializeChatHistory =
                await storageRepository.DownloadTextBlobAsync(GetFullBlobName(userId, conversationId), BlobContainerName);

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

    private static string GetFullBlobName(Guid userId, Guid conversationId)
    {
        return $"user-{userId}/conversations/conversation-{conversationId}.json";
    }
}
