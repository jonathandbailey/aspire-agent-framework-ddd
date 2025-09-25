namespace Infrastructure.Settings;

public class AzureStorageSettings
{
    public string ConversationBlobNameFormat { get; set; } = string.Empty;

    public string ConversationBlobContainerName { get; set; } = string.Empty;

    public string ConversationsPrefix { get; set; } = string.Empty;
}