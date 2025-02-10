using HirBot.Comman.Helpers;
using HirBot.Comman.Idenitity;
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
                user.ImagePath = await FileHelper.UpdateFileAsync(image.base64Data, "profile" + user.Id);
               await  _userManager.UpdateAsync(user);
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
           if(!result) return false;
           user.ImagePath = null; 
            await _userManager.UpdateAsync(user);
             return true ;
        }
       
        
        public async Task<bool> deleteCoverImage()
        {

            var user = await _authenticationService.GetCurrentUserAsync();
            if (user == null) return false;
        
                var result = await FileHelper.DeleteFileAsync("cover" + user.Id);
            user.CoverPath = null;
            await _userManager.UpdateAsync(user);
            return result;

        }
        public async Task<bool> editCoverImage(ImageDto image)
        {

            var user = await _authenticationService.GetCurrentUserAsync();
            if (user == null) return false;
            try
            {
                user.CoverPath = await FileHelper.UpdateFileAsync(image.base64Data, "cover" + user.Id);
                await _userManager.UpdateAsync(user);
                return true;

            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
