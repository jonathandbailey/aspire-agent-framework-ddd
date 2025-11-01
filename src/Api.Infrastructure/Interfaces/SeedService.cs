namespace Api.Infrastructure.Interfaces;

public class SeedService(IServiceProvider serviceProvider, IConfiguration configuration, ILogger<SeedService> logger) : IHostedService
{
    private const string ContainerNameKey = "SeedService:ContainerName";
    private const string LocalFolderPathKey = "SeedService:LocalFolderPath";
    private const string DefaultContainerName = "agent-templates";
    private const string DefaultContentType = "application/yaml";

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = serviceProvider.CreateScope();

        var storageRepository = scope.ServiceProvider.GetRequiredService<IAzureStorageRepository>();

        // Get configuration values
        var containerName = configuration[ContainerNameKey] ?? DefaultContainerName;
        var localFolderPath = configuration[LocalFolderPathKey];

        if (string.IsNullOrWhiteSpace(localFolderPath))
        {
            logger.LogWarning("LocalFolderPath is not configured. Skipping seed data upload.");
            return;
        }

        // Make path absolute if it's relative - resolve from solution root
        if (!Path.IsPathRooted(localFolderPath))
        {
            // Get the solution root by going up from the current directory
            var currentDirectory = Directory.GetCurrentDirectory();
            var solutionRoot = GetSolutionRoot(currentDirectory);
            
            if (solutionRoot != null)
            {
                localFolderPath = Path.Combine(solutionRoot, localFolderPath);
            }
            else
            {
                // Fallback to current directory if solution root not found
                localFolderPath = Path.Combine(currentDirectory, localFolderPath);
            }
        }

        // Normalize the path
        localFolderPath = Path.GetFullPath(localFolderPath);

        if (!Directory.Exists(localFolderPath))
        {
            logger.LogWarning("Local folder path '{LocalFolderPath}' does not exist. Skipping seed data upload.", localFolderPath);
            return;
        }

        try
        {
            // Check if container exists
            var containerExists = await storageRepository.ContainerExists(containerName);

            // Create container if it doesn't exist
            if (!containerExists)
            {
                logger.LogInformation("Container '{ContainerName}' does not exist. Creating it now.", containerName);
                await storageRepository.CreateContainerAsync(containerName);
            }
            else
            {
                logger.LogInformation("Container '{ContainerName}' already exists.", containerName);
            }

            // Parse all files in the local folder
            var files = Directory.GetFiles(localFolderPath, "*.*", SearchOption.AllDirectories);

            logger.LogInformation("Found {FileCount} files to upload from '{LocalFolderPath}'", files.Length, localFolderPath);

            // Upload each file to the container (overwrite)
            foreach (var filePath in files)
            {
                var fileName = Path.GetFileName(filePath);
                var fileContent = await File.ReadAllTextAsync(filePath, cancellationToken);
                var contentType = GetContentType(filePath);

                logger.LogInformation("Uploading file '{FileName}' to container '{ContainerName}'", fileName, containerName);

                await storageRepository.UploadTextBlobAsync(fileName, containerName, fileContent, contentType);

                logger.LogInformation("Successfully uploaded '{FileName}'", fileName);
            }

            logger.LogInformation("Seed data upload completed. Uploaded {FileCount} files.", files.Length);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred during seed data upload to container '{ContainerName}'", containerName);
            // Don't throw - we don't want to prevent the application from starting
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    private static string? GetSolutionRoot(string startPath)
    {
        var directory = new DirectoryInfo(startPath);
        
        while (directory != null)
        {
            // Look for .sln file or src directory as indicators of solution root
            if (directory.GetFiles("*.sln").Length > 0 || 
                (directory.GetDirectories("src").Length > 0 && directory.Parent != null))
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