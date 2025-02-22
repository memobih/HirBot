using Azure.Storage.Blobs;
using System;
using System.IO;
using System.Threading.Tasks;

namespace HirBot.Common.Helpers
{
    public static class FileHelper
    {
        private static readonly string ConnectionString = "DefaultEndpointsProtocol=https;AccountName=hirbot;AccountKey=MAojMV/ugkSzqN2ZACqOs6bVZRVDc1hbmxGfccP3E8rzpHGYTFNeCKI+4urvTeIc0Tfcm6vK4IMX+ASt04QX2w==;EndpointSuffix=core.windows.net";

        // 📌 Upload file to Azure Blob Storage
        public static async Task<string> UploadFileAsync(Stream fileStream, string fileName , string ContainerName)
        {
            try
            {
                BlobServiceClient blobServiceClient = new BlobServiceClient(ConnectionString);
                BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(ContainerName);
                BlobClient blobClient = containerClient.GetBlobClient(fileName);

                await blobClient.UploadAsync(fileStream, overwrite: true);
                return blobClient.Uri.ToString(); // Return the file name as confirmation
            }
            catch (Exception ex)
            {
                throw new Exception($"File upload failed: {ex.Message}");
            }
        }

        // 📌 Update an existing file in Azure Blob Storage
        public static async Task<string> UpdateFileAsync(Stream fileStream,string fileUrl ,  string fileName , string ContainerName)
        {
            try
            {
                await DeleteFileAsync(fileUrl , ContainerName); // Delete the old file
                return await UploadFileAsync(fileStream, fileName, ContainerName); // Overwrites the existing file
            }
            catch (Exception ex)
            {
                throw new Exception($"File update failed: {ex.Message}");
            }


        }

        // 📌 Delete file from Azure Blob Storage
        public static async Task<bool> DeleteFileAsync(string fileUrl, string ContainerName)
        {
            try
            {
                Uri uri = new Uri(fileUrl);
                string blobName = uri.Segments.Last();

                BlobServiceClient blobServiceClient = new BlobServiceClient(ConnectionString);
                BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(ContainerName);
                BlobClient blobClient = containerClient.GetBlobClient(blobName);

                return await blobClient.DeleteIfExistsAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"File deletion failed: {ex.Message}");
            }
        }

        // 📌 Download file from Azure Blob Storage
        public static async Task<Stream> DownloadFileAsync(string fileName , string ContainerName)
        {
            try
            {
                BlobServiceClient blobServiceClient = new BlobServiceClient(ConnectionString);
                BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(ContainerName);
                BlobClient blobClient = containerClient.GetBlobClient(fileName);
                var response = await blobClient.DownloadAsync();
                return response.Value.Content;
            }
            catch (Exception ex)
            {
                throw new Exception($"File download failed: {ex.Message}");
            }
        }
    }
}
