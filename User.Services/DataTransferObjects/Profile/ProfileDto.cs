using HirBot.Data.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using User.Services.Response;

namespace User.Services.DataTransferObjects.Profile
{
    public  class ProfileDto
    {  
        public string? title { get; set; }
        public string ? profileImageSrc { get; set; } 
        public string ? coverImageSrc { get; set; } 
        public ContactInfo?  ContactInfo { get; set; }
        public string ?  CVUrl { get; set; }
        public string? FullName { get; set; }
        public ExperienceResponse ? CurrentJop { get; set; }
        public int friendsCount {  get; set; }  

       
    }

   public class ContactInfo
    {
        [Url(ErrorMessage = "this must be Url")]
        public string? PortfolioURL { get; set; }

        public string? Location { get; set; }
        [MaxLength(11)]
        [RegularExpression(@"^\d+$")]
        public string? ContactNumber { get; set; }
        [Url(ErrorMessage = "this must be Url")]
        public string? GithubURL { get; set; }
    } 
}
