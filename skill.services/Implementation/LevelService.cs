using HirBot.ResponseHandler.Models;
using Project.Repository.Repository;
using Skill.services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skill.services.Implementation
{
    public  class LevelService : ILevelService
    {
        private readonly UnitOfWork _unitOfWork;
        public LevelService(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
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
    }
}
