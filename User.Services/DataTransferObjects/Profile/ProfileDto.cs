using HirBot.Data.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using User.Services.Response;

namespace User.Services.DataTransferObjects.Profile
{
    public  class ProfileDto
    {  
        public string? Title { get; set; }
        public string ? ProfileImagePath { get; set; } 
        public string ? CoverImagePath { get; set; } 
        public ContactInfoDto ?  ContactInfo { get; set; }
        public string ?  CVUrl { get; set; }
        public string? FullName { get; set; }
        public ExperienceResponse ? CurrentJop { get; set; }
        public int   FrindsCount {  get; set; }  

       
    }
}
