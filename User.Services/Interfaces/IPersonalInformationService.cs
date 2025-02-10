using HirBot.ResponseHandler.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using User.Services.DataTransferObjects.Authencation.ProfileDto;

namespace User.Services.Interfaces
{
    public  interface IPersonalInformationService
    {
        public Task<APIOperationResponse<PersonalInfoDto>> GetPersonalInformationAsync();
        public Task<APIOperationResponse<PersonalInfoDto>> UpdatePersonalInformationAsync(PersonalInfoDto personalInfoDto); 
    }
}
