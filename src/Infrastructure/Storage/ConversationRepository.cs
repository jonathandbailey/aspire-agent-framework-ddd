using System.Text.Json;
using System.Text.Json.Serialization;
using Application.Interfaces;
using Domain.Conversations;
using Infrastructure.Dto;
using Infrastructure.Exceptions;
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
        Verify.NotNull(conversation.UserId);
      
        try
        {
            var serializedConversation = JsonSerializer.Serialize(conversation.Map(), SerializerOptions);

            await storageRepository.UploadTextBlobAsync(_settings.GetConversationBlobPath(conversation.UserId.Value, conversation.Id),
                _settings.ConversationBlobContainerName, serializedConversation, ApplicationJsonContentType);
        }
        catch (JsonException exception)
        {
            var ex = new ConversationInfrastructureException(
                $"Failed to serialize conversation {conversation.Id} for user {conversation.UserId.Value}", exception);

            logger.LogError(ex, "Failed to serialize conversation {ConversationId} for user {UserId}", conversation.Id, conversation.UserId.Value);

            throw ex;
        }
        catch (Exception exception)
        {
            var ex = new ConversationInfrastructureException(
                $"Error while trying to save conversation {conversation.Id} for user {conversation.UserId.Value}", exception);

            logger.LogError(ex, "Error while trying to save conversation {ConversationId} for user {UserId}", conversation.Id, conversation.UserId.Value);

            throw ex;
        }
    }

    public async Task<Conversation> LoadAsync(Guid userId, Guid conversationId)
    {
        Verify.NotEmpty(conversationId);
        Verify.NotEmpty(userId);

        try
        {
            var serializeChatHistory =
                await storageRepository.DownloadTextBlobAsync(_settings.GetConversationBlobPath(userId, conversationId), _settings.ConversationBlobContainerName);

            var conversation = JsonSerializer.Deserialize<ConversationDto>(serializeChatHistory, SerializerOptions);

            Verify.NotNull(conversation);

            return conversation.Map();
        }
        catch (JsonException exception)
        {
            var ex = new ConversationInfrastructureException($"Failed to deserialize conversation {conversationId} for user {userId}", exception);
            logger.LogError(ex, "Failed to deserialize conversation {ConversationId} for user {UserId}", conversationId, userId);
            throw ex;
        }
        catch (Exception exception)
        {
            var ex = new ConversationInfrastructureException($"Error trying to load conversation {conversationId} for user {userId}", exception);
            logger.LogError(ex, "Error trying to load conversation {ConversationId} for user {UserId}", conversationId, userId);
            throw ex;
        }
    }
}
