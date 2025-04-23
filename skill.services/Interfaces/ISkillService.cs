
using HirBot.ResponseHandler.Models;
using skill.services.DataTransferObjects;

namespace skill.services.Interfaces
{
    public interface ISkillService
    {
        public Task<APIOperationResponse<AddSkillDto>> AddSkill(AddSkillDto skill);
        public Task<APIOperationResponse<object>> GetAllSkills(string? searh = null, int page = 1, int perpage = 10);
        public Task<APIOperationResponse<bool>> DeleteSkill(List<int> ids);
        public Task<APIOperationResponse<GettingAllSkillsDto>> GetSkill(int id);
        public Task<APIOperationResponse<UpdateSkillDto>> UpdateSkill(int  id , UpdateSkillDto skill);

        public Task<APIOperationResponse<object>> GetALLSkillForUsers();
        public Task<APIOperationResponse<object>> AddSkill(int id); 

        public Task<APIOperationResponse<object>> GetUserSkill();
    }
}