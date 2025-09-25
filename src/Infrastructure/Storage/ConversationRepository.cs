using System.Text.Json;
using System.Text.Json.Serialization;
using Application.Interfaces;
using Domain.Conversations;
using Infrastructure.Dto;
using Infrastructure.Extensions;
using Infrastructure.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;


namespace Infrastructure.Storage;

public class ConversationRepository(
    IAzureStorageRepository storageRepository,
    IOptions<AzureStorageSettings> settings,
    ILogger<ConversationRepository> logger) : IConversationRepository
{
    private const string ApplicationJsonContentType = "application/json";

    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters = { new JsonStringEnumConverter() }
    };

    private readonly AzureStorageSettings _settings = settings.Value;

    public async Task SaveAsync(Conversation conversation)
    {
        Verify.NotNull(conversation);

        try
        {
            var serializedConversation = JsonSerializer.Serialize(conversation.Map(), SerializerOptions);

            await storageRepository.UploadTextBlobAsync(GetFullBlobName(conversation.UserId.Value, conversation.Id), _settings.ConversationBlobContainerName, serializedConversation, ApplicationJsonContentType);
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
                await storageRepository.DownloadTextBlobAsync(GetFullBlobName(userId, conversationId), _settings.ConversationBlobContainerName);

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

    private string GetFullBlobName(Guid userId, Guid conversationId)
    {
        return _settings.ConversationBlobNameFormat
            .Replace("{userId}", userId.ToString())
            .Replace("{conversationId}", conversationId.ToString());
    }
}
