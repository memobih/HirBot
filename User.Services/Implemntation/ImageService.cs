using HirBot.Comman.Helpers;
using Microsoft.AspNetCore.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using User.Services.DataTransferObjects.images;
using User.Services.Interfaces;

namespace User.Services.Implemntation
{
    public class ImageService :IIamge
    {
        private readonly Project.Services.Interfaces.IAuthenticationService _authenticationService;
        public ImageService(Project.Services.Interfaces.IAuthenticationService authenticationService) { 
            _authenticationService = authenticationService;
       
        }

        public async Task<bool>  addProfileImage(ImageDto image)
        {
            
            var user =  await _authenticationService.GetCurrentUserAsync(); 
            if (user == null) return false;
            try
            {
                user.ImagePath = await FileHelper.UploadFileAsync(image.base64Data, "profile" + user.Id);
                return true;

            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public async Task<bool> editProfileImage(ImageDto image)
        {

            var user = await _authenticationService.GetCurrentUserAsync();
            if (user == null) return false;
            try
            {
                user.ImagePath = await FileHelper.UpdateFileAsync(image.base64Data, "profile" + user.Id);
                return true;

            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public async Task<bool> deleteProfileImage()
        {

            var user = await _authenticationService.GetCurrentUserAsync();
            if (user == null) return false;
                  var result = await FileHelper.DeleteFileAsync("profile" + user.Id);
            return false;
        }
        public async Task<bool> addCoverImage(ImageDto image)
        {

            var user = await _authenticationService.GetCurrentUserAsync();
            if (user == null) return false;
            try
            {
                user.CoverPath = await FileHelper.UploadFileAsync(image.base64Data, "cover" + user.Id);
                return true;

            }
            catch (Exception ex)
            {
                return false;
            }

        }
        public async Task<bool> deleteCoverImage()
        {

            var user = await _authenticationService.GetCurrentUserAsync();
            if (user == null) return false;
        
                var result = await FileHelper.DeleteFileAsync("cover" + user.Id);
                return result;

        }
        public async Task<bool> editCoverImage(ImageDto image)
        {

            var user = await _authenticationService.GetCurrentUserAsync();
            if (user == null) return false;
            try
            {
                user.CoverPath = await FileHelper.UpdateFileAsync(image.base64Data, "cover" + user.Id);
                return true;

            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
