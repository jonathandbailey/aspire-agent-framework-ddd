namespace Api.Infrastructure;

public interface IAzureStorageRepository
{
    Task<string> DownloadTextBlobAsync(string blobName, string containerName);
    Task UploadTextBlobAsync(string blobName, string containerName, string content, string contentType);
    Task<bool> ContainerExists(string containerName);
    Task CreateContainerAsync(string containerName);
}