using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HirBot.ResponseHandler.Models;
using User.Services.DataTransferObjects;

namespace User.Services.Interfaces
{
    public interface IContactInfoService
    {
        public Task<APIOperationResponse<ContactInfoDto>> GetContactInfoAsync();
        public Task<APIOperationResponse<ContactInfoDto>> UpdateContactInfoAsync(ContactInfoDto contactInfoDto);

    }
}