using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace User.Services.DataTransferObjects.Authencation.ProfileDto
{
    public class PersonalInfoDto
    {
        public string FullName { get; set; }
        public string Title { get; set; }
        public string ImagePath { get; set; }
        public string? PortLink { get; set; }

    }
}