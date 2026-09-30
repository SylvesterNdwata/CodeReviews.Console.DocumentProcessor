using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;

namespace silvermax.DocumentProcessor.Services;

public class BlobStorageService : IBlobStorageService
{
    private readonly BlobContainerClient _containerClient;
    public BlobStorageService(IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("AzureBlobStorage")
            ?? throw new InvalidOperationException("Azure Blob Connection String must be configured");
        var containerName = configuration["Azure:containerName"]
            ?? throw new InvalidOperationException("Azure Blob Storage Container must be configured");

        _containerClient = new BlobContainerClient(connectionString, containerName);
    }
    public async Task DownloadFileAsync(string blobName, string localFilePath)
    {
        var blobClient = _containerClient.GetBlobClient(blobName);
        await blobClient.DownloadToAsync(localFilePath);
    }

    public async Task UploadFileAsync(string localFilePath, string blobName)
    {
        var blobClient = _containerClient.GetBlobClient(blobName);
        await blobClient.UploadAsync(localFilePath, overwrite: true);
    }
}
