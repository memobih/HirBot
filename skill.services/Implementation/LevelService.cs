using HirBot.Comman.Enums;
using HirBot.Data.Entities;
using HirBot.ResponseHandler.Models;
using Org.BouncyCastle.Crypto;
using Project.Repository.Repository;
using Project.Services.Interfaces;
using Skill.services.DataTransferObjects;
using Skill.services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Mysqlx.Notice.Warning.Types;

namespace Skill.services.Implementation
{
    public  class LevelService : ILevelService
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly IAuthenticationService _authenticationService;
        public LevelService(UnitOfWork unitOfWork , IAuthenticationService authentication)
        {
            _unitOfWork = unitOfWork;
            _authenticationService = authentication;

        }

        public async Task<APIOperationResponse<object>> AddLevel(LevelDto level)
        {
            try
            {
                var user =await _authenticationService.GetCurrentUserAsync();
                if (user.role != UserType.Admin)
                    return APIOperationResponse<object>.UnOthrized("this email is not admin");

                var newLevel = new HirBot.Data.Entities.Level(); 
                newLevel.max=level.max;
                newLevel.min=level.min;
                newLevel.Name=level.Name;
                newLevel.status=level.status;
               await _unitOfWork.Levels.AddAsync(newLevel);
              await  _unitOfWork.SaveAsync();
                return APIOperationResponse<object>.Success("the level is added " , "the level is added ");
            }
            catch (Exception ex)
            {
                return APIOperationResponse<object>.ServerError("there are error accured");
            }
        }

        public async Task<APIOperationResponse<object>> Delete(List<int> ids)
        {
            try
            {
                var user = await _authenticationService.GetCurrentUserAsync();
                if (user.role != UserType.Admin)
                    return APIOperationResponse<object>.UnOthrized("this email is not admin");

                var levels = _unitOfWork._context.Levels.Where(l => ids.Contains(l.ID)).ToList();
                 _unitOfWork._context.RemoveRange(levels);
                await _unitOfWork.SaveAsync();
                return APIOperationResponse<object>.Success("the level is deleted ", "the level is deleted ");
            }
            catch (Exception ex)
            {
                return APIOperationResponse<object>.ServerError("there are error accured");
            }
        }

        public async Task<APIOperationResponse<object>> GetALLLevel()
        {
            try
            {
                return APIOperationResponse<object>.Success(
                    await _unitOfWork.Levels.GetAllAsync()
                    , message: "all Level"); 


            }
            catch (Exception ex)
            {
                return APIOperationResponse<object>.ServerError("there are error accured");
            }
            throw new NotImplementedException();
        }

        public async Task<APIOperationResponse<object>> GetALLLevelByAdmin(int page = 1, int perpage = 10)
        {
            try
            {
                var user = await _authenticationService.GetCurrentUserAsync();
                if (user.role != UserType.Admin)
                    return APIOperationResponse<object>.UnOthrized("this email is not admin");

                var levels = _unitOfWork._context.Levels.ToList();

             
                return APIOperationResponse<object>.Success(new 
                {
                    currentPage = page,
                    totalPages = (levels.Count() / perpage) + 1,
                    pageSize = perpage,
                    totalRecords = levels.Count(),
                    data = Paginate(levels, page, perpage)
                }, "the level is added ");
            }
            catch (Exception ex)
            {
                return APIOperationResponse<object>.ServerError("there are error accured");
            }
        }

        public async Task<APIOperationResponse<object>> UpdateLevel(int id, LevelDto level)
        {
            try
            {
                var user = await _authenticationService.GetCurrentUserAsync();
                if (user.role != UserType.Admin)
                    return APIOperationResponse<object>.UnOthrized("this email is not admin");

                var editlevel = _unitOfWork._context.Levels.Where(l => l.ID==id).FirstOrDefault();
                if (editlevel == null) return APIOperationResponse<object>.NotFound("this level is not found ");
                editlevel.Name = level.Name;
                editlevel.max=level.max;
                editlevel.min=level.min;
              editlevel.status=level.status;
                _unitOfWork._context.Update(editlevel);
                await _unitOfWork.SaveAsync();
                return APIOperationResponse<object>.Success("the level is updated ", "the level is updated ");
            }
            catch (Exception ex)
            {
                return APIOperationResponse<object>.ServerError("there are error accured");
            }
        }
        #region helper  
        private List<T> Paginate<T>(List<T> source, int page, int pageSize)
        {
            return source.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        }
        #endregion
    }
}
