using Google.Protobuf;
using HirBot.Common.Helpers;
using HirBot.Data.Entities;
using HirBot.ResponseHandler.Models;
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

        public async Task<APIOperationResponse<object>> UpdateCv(FileDto cv)
        {

            var user = await _authenticationService.GetCurrentUserAsync();
            user = _unitOfWork._context.users.Include(U => U.Portfolio).First(u => u.Id == user.Id);
            if (user.Portfolio == null) user.Portfolio = new HirBot.Data.Entities.Portfolio();
            if (cv.File !=null && cv.File.Length > 0)
            {
                string extension = Path.GetExtension(cv.File.FileName);
                if(extension!=".pdf") return APIOperationResponse<object>.BadRequest("cv must be  pdf");
                try
                {
                    using var stream = cv.File.OpenReadStream();

                    user.Portfolio.CVUrl = await FileHelper.UpdateFileAsync(stream, user.Portfolio.CVUrl, user.Id + "CV" + extension, "cvs");
                    _unitOfWork._context.users.Update(user);
                    _unitOfWork._context.SaveChanges();
                    return APIOperationResponse<object>.Success(new { Cv = user.Portfolio.CVUrl });
                }
                catch (Exception ex)
                {
                    return APIOperationResponse<object>.BadRequest("can not uploud cv try ageain");
                }
            }
            return APIOperationResponse<object>.BadRequest("cv is required");
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
