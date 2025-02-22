using HirBot.Comman.Idenitity;
using HirBot.Common.Helpers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
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
        private readonly UserManager<ApplicationUser> _userManager;
        private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png" };

        public ImageService(Project.Services.Interfaces.IAuthenticationService authenticationService , UserManager<ApplicationUser> userManager) { 
            _authenticationService = authenticationService;
            _userManager = userManager; 
       
        }

        public async Task<bool> editProfileImage(ImageDto image)
        {

            var user = await _authenticationService.GetCurrentUserAsync();
            if (user == null) return false;
            try
            {
                string extension = Path.GetExtension(image.image.FileName).ToLower();
                if (!AllowedExtensions.Contains (extension) ) 
                    return false;
                if (image.image != null && image.image.Length > 0)
                {
                    using var stream = image.image.OpenReadStream();

                    user.ImagePath = await FileHelper.UpdateFileAsync(stream, user.ImagePath, "profile" + user.Id+extension, "userprofileimages");
                    await _userManager.UpdateAsync(user);

                    return true;
                }
                return false; 

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
          
            var result = await FileHelper.DeleteFileAsync(user.ImagePath , "userprofileimages"); 
           if(!result) return false;
           user.ImagePath = "https://hirbot.blob.core.windows.net/user/images.jpeg.jpg";
            try
            {
                await _userManager.UpdateAsync(user); 
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
              var result = await FileHelper.DeleteFileAsync(user.CoverPath, "usercoverimages");
             if (!result) return false;
            user.CoverPath = "https://hirbot.blob.core.windows.net/user/images.jpeg.jpg";

            try
            {
                await _userManager.UpdateAsync(user); 
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }
        public async Task<bool> editCoverImage(ImageDto image)
        {

            var user = await _authenticationService.GetCurrentUserAsync();
            if (user == null) return false;
            try
            {
                string extension = Path.GetExtension(image.image.FileName).ToLower();
                if (!AllowedExtensions.Contains(extension))
                    return false;
                if (image.image!= null && image.image.Length > 0)
                {
                    using var stream = image.image.OpenReadStream();

                    user.CoverPath = await FileHelper.UpdateFileAsync(stream, user.ImagePath, "cover" + user.Id + extension, "usercoverimages");
                    await _userManager.UpdateAsync(user);

                    return true;
                }
                return false;

            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
