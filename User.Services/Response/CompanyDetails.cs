using HirBot.Data.Entities;
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
        public string  ? id { get; set;  }
        public string  ? name {  get; set; } 
        public string  ? email { get; set; }

        public string?  companyType { get; set; }

        public string ? country { get; set; }
        public string  ? street { get; set; }

        public string  ? phoneNumber { get; set; }
        public string  ? TaxNumber { get; set; }
        public CompanyStatus status { get; set; }
        public string  ? Governate { get; set; }
        public string  ? logo { get; set; }
        public SocialMedia SocialMedia { get; set; } = new SocialMedia();
        public legalInformation  legalInformation { get; set; } =new legalInformation();
    }
    public class legalInformation
    {
        public string? website { get; set; }
        public string? BuisnessLicense { get; set; }

    }
    public class SocialMedia
    {
        public string? FacebookLink { get; set; }
        public string? TikTokLink { get; set; }
        public string? InstgrameLink { get; set; }
        public string? TwitterLink { get; set; }

    }

    public class EmployeeList
    {
        public int id { get; set; }
        public string? name { get; set; }
        public string Title { get; set; }

        public EmployeeType jobType { get; set; }
        public LocationType workType { get; set; }

        public int Rate { get; set; }
        public DateTime? start_date { get; set; }
        public DateTime? end_date { get; set; }
        public string? image { get; set; }
        public string? location { get; set; }
        public string? email { get; set; }

    }


}
