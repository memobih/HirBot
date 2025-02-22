using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using HirBot.Data.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;

namespace Project.Repository.Repository
{
    public class ImageHandler : IImageHandler
    {
        private readonly string _uploadrootPath;
        public ImageHandler(IHostEnvironment env)
        {
            _uploadrootPath = Path.Combine(env.ContentRootPath, "wwwroot", "uploads");
        }
   public async Task<(bool IsSuccess, string FilePath, string ErrorMessage)> UploadImageAsync(IFormFile file, string folderName)
    {
        try
        {
            if (file == null || file.Length == 0)
                return (false, null, "Invalid file!");

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var fileExtension = Path.GetExtension(file.FileName).ToLower();
            if (!Array.Exists(allowedExtensions, ext => ext == fileExtension))
                return (false, null, "Only JPG, PNG, and GIF files are allowed.");

            if (file.Length > 2 * 1024 * 1024) // Max 2MB
                return (false, null, "File size must be less than 2MB.");

            // Create Folder if Not Exists
            string folderPath = Path.Combine(_uploadrootPath, folderName);
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            // Generate Unique File Name
            string newFileName = $"{Guid.NewGuid()}{fileExtension}";
            string filePath = Path.Combine(folderPath, newFileName);

            // Save File
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Return File Path
            string relativePath = $"/uploads/{folderName}/{newFileName}";
            return (true, relativePath, null);
        }
        catch (Exception ex)
        {
            return (false, null, $"Error: {ex.Message}");
        }
    }

    // 🔴 DELETE: Remove Image
    public async Task<(bool,string)> DeleteImage(string filePath)
    {
        try
        {
            string fullPath = Path.Combine(_uploadrootPath, filePath.TrimStart('/'));
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
                return (true, null);
            }
            return (false, "File not found!");
        }
        catch (Exception ex)
        {
            return (false, $"Error: {ex.Message}");
        }
    }

    // 🔄 UPDATE: Replace Existing Image
    public async Task<(bool IsSuccess, string FilePath, string ErrorMessage)> UpdateImageAsync(IFormFile newFile, string existingFilePath, string folderName)
    {
        // Delete old file first
        if (!string.IsNullOrEmpty(existingFilePath))
             await  DeleteImage(existingFilePath);

        // Upload new file
        return await UploadImageAsync(newFile, folderName);
    }

    // 🟡 READ: Get Image URL
    public Task<string> GetImageUrl(string filePath)
    {
        return Task.FromResult(filePath);
    }
    }
}