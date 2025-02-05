using Azure.Storage.Blobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HirBot.Comman.Helpers
{
    static public class FileHelper
    {

        private static readonly string ConnectionString = "DefaultEndpointsProtocol=https;AccountName=hirbot;AccountKey=MAojMV/ugkSzqN2ZACqOs6bVZRVDc1hbmxGfccP3E8rzpHGYTFNeCKI+4urvTeIc0Tfcm6vK4IMX+ASt04QX2w==;EndpointSuffix=core.windows.net";
        private static readonly string ContainerName = "hirbot";

        // 📌 Upload file to Azure Blob Storage
        public static async Task<string> UploadFileAsync(string base64Data, string fileName)
        {
            try
            {
                byte[] fileBytes = Convert.FromBase64String(FixBase64String(base64Data));

                BlobServiceClient blobServiceClient = new BlobServiceClient(ConnectionString);
                BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(ContainerName);
                BlobClient blobClient = containerClient.GetBlobClient(fileName);

                using (var stream = new MemoryStream(fileBytes))
                {
                    await blobClient.UploadAsync(stream, overwrite: true);
                }

                return blobClient.Uri.ToString();  // Return the file URL
            }
            catch (Exception ex)
            {
                throw new Exception($"File upload failed: {ex.Message}");
            }
        }

        // 📌 Update an existing file in Azure Blob Storage
        public static async Task<string> UpdateFileAsync(string base64Data, string fileName)
        {
            return await UploadFileAsync(base64Data, fileName); // Overwrites the existing file
        }

        // 📌 Delete file from Azure Blob Storage
        public static async Task<bool> DeleteFileAsync(string fileName)
        {
            try
            {
                BlobServiceClient blobServiceClient = new BlobServiceClient(ConnectionString);
                BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(ContainerName);
                BlobClient blobClient = containerClient.GetBlobClient(fileName);

                return await blobClient.DeleteIfExistsAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"File deletion failed: {ex.Message}");
            }
        }

        // 📌 Fix Base64 padding
        private static string FixBase64String(string base64)
        {
            while (base64.Length % 4 != 0)
                base64 += "=";
            return base64;
        }
    }
}
