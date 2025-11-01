using Api.Infrastructure.Interfaces;
using Api.Infrastructure.Settings;
using Microsoft.Extensions.Options;

namespace Api.Infrastructure.Services;

public class AzureStorageSeedService(
    IServiceProvider serviceProvider,
    IOptions<AzureStorageSeedSettings> settings,
    ILogger<AzureStorageSeedService> logger) : IHostedService
{
    
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = serviceProvider.CreateScope();

        var storageRepository = scope.ServiceProvider.GetRequiredService<IAzureStorageRepository>();

        var containerName = settings.Value.ContainerName;
        var localFolderPath = settings.Value.LocalFolderPath;

        if (string.IsNullOrWhiteSpace(localFolderPath))
        {
            logger.LogWarning("LocalFolderPath is not configured. Skipping seed data upload.");
            return;
        }

        if (!Path.IsPathRooted(localFolderPath))
        {
            var currentDirectory = Directory.GetCurrentDirectory();
            var solutionRoot = GetSolutionRoot(currentDirectory, localFolderPath);

            if (solutionRoot == null)
                throw new Exception($"Folder {localFolderPath} was not found in the parent hierarchy.");
            
            localFolderPath = Path.Combine(solutionRoot, localFolderPath);
        }

        localFolderPath = Path.GetFullPath(localFolderPath);

        if (!Directory.Exists(localFolderPath))
        {
            logger.LogWarning("Local folder path '{LocalFolderPath}' does not exist. Skipping seed data upload.", localFolderPath);
            return;
        }

        try
        {
            var containerExists = await storageRepository.ContainerExists(containerName);

            if (!containerExists)
            {
                logger.LogInformation("Container '{ContainerName}' does not exist. Creating it now.", containerName);
                await storageRepository.CreateContainerAsync(containerName);
            }
     
            var files = Directory.GetFiles(localFolderPath, "*.*", SearchOption.AllDirectories);

            foreach (var filePath in files)
            {
                var fileName = Path.GetFileName(filePath);
                var fileContent = await File.ReadAllTextAsync(filePath, cancellationToken);
                var contentType = GetContentType(filePath);

                await storageRepository.UploadTextBlobAsync(fileName, containerName, fileContent, contentType);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred during seed data upload to container '{ContainerName}'", containerName);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    private static string? GetSolutionRoot(string startPath, string targetFolder)
    {
        var directory = new DirectoryInfo(startPath);
        
        while (directory != null)
        {
            if ((directory.GetDirectories(targetFolder).Length > 0 && directory.Parent != null))
            {
                return directory.FullName;
            }
            
            directory = directory.Parent;
        }
        
        return null;
    }

    private static string GetContentType(string filePath)
    {
        var extension = Path.GetExtension(filePath).ToLowerInvariant();

        return extension switch
        {
            ".json" => "application/json",
            ".yaml" => "application/yaml",
            ".yml" => "application/yaml",
            ".xml" => "application/xml",
            ".txt" => "text/plain",
            ".csv" => "application/csv",
            ".js" => "application/javascript",
            _ => "text/plain"
        };
    }
}