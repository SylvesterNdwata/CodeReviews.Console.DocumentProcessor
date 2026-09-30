namespace silvermax.DocumentProcessor.Services;

public interface IBlobStorageService
{
    Task UploadFileAsync(string localFilePath, string blobName);
    Task DownloadFileAsync(string blobName, string localFilePath);
}
