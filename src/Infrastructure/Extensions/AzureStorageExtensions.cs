using Infrastructure.Settings;

namespace Infrastructure.Extensions;

public static class AzureStorageExtensions
{
    public static string GetConversationBlobPath(this AzureStorageSettings settings, Guid userId, Guid conversationId)
    {
        return settings.ConversationBlobNameFormat
            .Replace("{userId}", userId.ToString())
            .Replace("{conversationId}", conversationId.ToString());
    }
}