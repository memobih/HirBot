
using HirBot.ResponseHandler.Models;
using skill.services.DataTransferObjects;

namespace skill.services.Interfaces
{
    public interface ISkillService
    {
        public Task<APIOperationResponse<AddSkillDto>> AddSkill(AddSkillDto skill);
        public Task<APIOperationResponse<List<GettingAllSkillsDto>>> GetAllSkills(string? searh = null);
        public Task<APIOperationResponse<bool>> DeleteSkill(int id);
        public Task<APIOperationResponse<GettingAllSkillsDto>> GetSkill(int id);
        public Task<APIOperationResponse<UpdateSkillDto>> UpdateSkill(UpdateSkillDto skill);

        public Task<APIOperationResponse<object>> GetALLSkillForUsers();
        public Task<APIOperationResponse<object>> AddSkill(int id); 

        public Task<APIOperationResponse<object>> GetUserSkill();
    }
}