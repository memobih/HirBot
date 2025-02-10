using HirBot.Comman.Helpers;
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
            string name = "cv" + user.Id; 
            user.Portfolio.CVUrl = await FileHelper.UpdateFileAsync(cv.base64Data ,name );
            _unitOfWork._context.users.Update(user);
            _unitOfWork._context.SaveChanges(); 
            return true ;  
        }
        public async Task<bool> DeleteCv()
        {
            var user = await _authenticationService.GetCurrentUserAsync();
            if (user == null) return false;
            user = _unitOfWork._context.users.Include(U => U.Portfolio).First(u => u.Id == user.Id);
            if (user.Portfolio == null) user.Portfolio = new HirBot.Data.Entities.Portfolio();
            string name = "cv" + user.Id;
            var result=  await FileHelper.DeleteFileAsync (name);
            if (result == false) return result;
            user.Portfolio.CVUrl = null; 
            _unitOfWork._context.users.Update(user);
            _unitOfWork._context.SaveChanges();

            return true;
        }
    }
}
