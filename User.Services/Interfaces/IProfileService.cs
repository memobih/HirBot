using HirBot.ResponseHandler.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using User.Services.DataTransferObjects.Authencation.ProfileDto;

namespace User.Services.Interfaces
{
    public interface IProfileService
    {
        public Task<APIOperationResponse<PersonalInfoDto>> GetPersonalInfoAsync();
    }
}