using Azure.Storage.Blobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
                if (base64Data.Contains(","))
                {
                    base64Data = base64Data.Split(',')[1];
                }
                byte[] fileBytes = Convert.FromBase64String(FixBase64String(base64Data));
                  string ext=  GetFileExtensionFromBase64(base64Data); 
                BlobServiceClient blobServiceClient = new BlobServiceClient(ConnectionString);
                BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(ContainerName);
                BlobClient blobClient = containerClient.GetBlobClient(fileName+ext);
                using (var stream = new MemoryStream(fileBytes))
                {
                    await blobClient.UploadAsync(stream, overwrite: true);
                }

                return fileName+ext;  // Return the file URL
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
        public static string GetFileExtensionFromBase64(string base64String)
        {
            // Check if Base64 contains a data URI scheme
            var match = Regex.Match(base64String, @"data:(?<type>.+?);base64,");

            if (match.Success)
            {
                string mimeType = match.Groups["type"].Value;
                return MimeTypeToExtension(mimeType);
            }

            throw new Exception("Invalid Base64 format: No MIME type found.");
        }

        private static string MimeTypeToExtension(string mimeType)
        {
            return mimeType.ToLower() switch
            {
                "application/pdf" => ".pdf",
                "application/msword" => ".doc",
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document" => ".docx",
                "image/png" => ".png",
                "image/jpeg" => ".jpg",
                "image/gif" => ".gif",
                "text/plain" => ".txt",
                _ => throw new Exception($"Unsupported MIME type: {mimeType}")
            };
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
