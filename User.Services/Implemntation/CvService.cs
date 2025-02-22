using Google.Protobuf;
using HirBot.Common.Helpers;
using HirBot.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Mysqlx.Crud;
using Project.Repository.Repository;
using Project.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using User.Services.DataTransferObjects.images;
using User.Services.Interfaces;
using static System.Net.Mime.MediaTypeNames;

namespace User.Services.Implemntation
{
    public  class CvService : ICVService
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly UnitOfWork _unitOfWork; 
        public CvService(IAuthenticationService authenticationService , UnitOfWork unitOfWork)
        {
            _authenticationService = authenticationService;
            _unitOfWork = unitOfWork; 

        }

        public async Task<bool> UpdateCv(ImageDto cv)
        {

            var user = await _authenticationService.GetCurrentUserAsync();
            if (user == null ) return false;
            user = _unitOfWork._context.users.Include(U => U.Portfolio).First(u => u.Id == user.Id);
            if (user.Portfolio == null) user.Portfolio = new HirBot.Data.Entities.Portfolio();
            if (cv.image !=null && cv.image.Length > 0)
            {
                string extension = Path.GetExtension(cv.image.FileName);
                if(extension!=".pdf") return false;
                try
                {
                    using var stream = cv.image.OpenReadStream();

                    user.Portfolio.CVUrl = await FileHelper.UpdateFileAsync(stream, user.Portfolio.CVUrl, user.Id + "CV" + extension, "cvs");
                    _unitOfWork._context.users.Update(user);
                    _unitOfWork._context.SaveChanges();
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
            return false;
        }
        public async Task<bool> DeleteCv()
        {
            var user = await _authenticationService.GetCurrentUserAsync();
            if (user == null) return false;
            user = _unitOfWork._context.users.Include(U => U.Portfolio).First(u => u.Id == user.Id);
            if (user.Portfolio == null) user.Portfolio = new HirBot.Data.Entities.Portfolio();
            try
            {
                var result = await FileHelper.DeleteFileAsync(user.Portfolio.CVUrl , "cvs");
                if (result == false) return result;
                user.Portfolio.CVUrl = null;
                _unitOfWork._context.users.Update(user);
                _unitOfWork._context.SaveChanges();
            } 
            catch (Exception ex) {
                return false;    
                }
            return true;
        }
    }
}
