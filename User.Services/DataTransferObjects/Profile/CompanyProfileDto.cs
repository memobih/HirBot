using HirBot.Data.Enums;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace User.Services.DataTransferObjects.Profile
{
    public  class CompanyProfileDto
    {
      
        public string CompanyType { get; set; }
        public string name { get; set; }

        public string? ImagePath { get; set; }
        public string? CoverPath { get; set; }
        public CompanyStatus status { get; set; }


        public Documents Documents { get; set; } 
        public Contact ContactInfo { get; set; } 
    } 
    public class Contact
    {
        public string? SocialMeediaLink { get; set; }

        public string? websiteUrl { get; set; }
        public string   ? country { get; set; }

        public string   ? Governate { get; set; }
        public string ?  street { get; set; } 
    } 
    public class Documents
    {
        public string? TaxIndtefierNumber { get; set; }

        public string? BusinessLicense { get; set; }

    }
}
