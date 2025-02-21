
using HirBot.ResponseHandler.Models;
using skill.services.DataTransferObjects;

namespace skill.services.Interfaces
{
    public interface ISkillService
    {
        public Task<APIOperationResponse<AddSkillDto>> AddSkill(AddSkillDto skill);
    }
}