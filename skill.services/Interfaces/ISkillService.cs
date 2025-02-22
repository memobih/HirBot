
using HirBot.ResponseHandler.Models;
using skill.services.DataTransferObjects;

namespace skill.services.Interfaces
{
    public interface ISkillService
    {
        public Task<APIOperationResponse<AddSkillDto>> AddSkill(AddSkillDto skill);
        public Task<APIOperationResponse<List<GettingAllSkillsDto>>> GetAllSkills();
        public Task<APIOperationResponse<bool>> DeleteSkill(int id);
        public Task<APIOperationResponse<GettingAllSkillsDto>> GetSkill(int id);
        public Task<APIOperationResponse<UpdateSkillDto>> UpdateSkill(UpdateSkillDto skill);
    }
}