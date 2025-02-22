using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace HirBot.Data.Interfaces
{
    public interface IImageHandler
    {
        
    public  Task<(bool IsSuccess, string FilePath, string ErrorMessage)> UploadImageAsync(IFormFile file, string folderName);
    public Task<bool> DeleteImage(string filePath);
    public Task<(bool IsSuccess, string FilePath, string ErrorMessage)> UpdateImageAsync(IFormFile newFile, string existingFilePath, string folderName);
    public Task<string> GetImageUrl(string filePath);
    }
}