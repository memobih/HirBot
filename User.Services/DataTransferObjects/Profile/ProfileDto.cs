using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace User.Services.DataTransferObjects.Profile
{
    public  class ProfileDto
    {  
        public string? PortfolioUrl { get; set; }
        public string? Title { get; set; }
        public string? location { get; set; }
        public string? CVUrl { get; set; }
        public string? GithubUrl { get; set; }
        public string ? ProfileImagePath { get; set; } 
        public string ? CoverImagePath { get; set; } 
        public string ? ContactNumber { get; set; }
         public string ? Email {  get; set; }

        public string? FullName { get; set; } 
        
    }
}
