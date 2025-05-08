using HirBot.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using User.Services.Implemntation;

namespace User.Services.Response
{
    public class CompanyDetails
    { 
        public string id { get; set;  }
        public string name {  get; set; } 
        public string email { get; set; }

        public string companyType { get; set; }

        public string country { get; set; }
        public string street { get; set; }

        public string phoneNumber { get; set; }
        public string  ? TaxNumber { get; set; }
        public CompanyStatus status { get; set; }
        public string Governate { get; set; }
        public string  ? logo { get; set; }
        public legalInformation  legalInformation { get; set; } =new legalInformation();
    }
    public class legalInformation
    {
        public string ?SocialMediaLink { get; set; } 
        public string? website { get; set; }
        public string? BuisnessLicense { get; set; }

    }
    
}
