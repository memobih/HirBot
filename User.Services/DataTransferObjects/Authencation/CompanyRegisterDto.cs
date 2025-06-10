using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace User.Services.DataTransferObjects.Authencation
{
    public class CompanyRegisterDto
    {
        [Required(ErrorMessage = "Name is required"),
         MaxLength(100, ErrorMessage = "Name cannot exceed 100 characters"),
         ]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Name can only contain letters")]
        [DataType(DataType.Text, ErrorMessage = "Invalid name format")]
        public string CompanyName { get; set; }



        [Required(ErrorMessage = "Email is required"), DataType(DataType.EmailAddress, ErrorMessage = "Invalid email format")]
        [MaxLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
        [RegularExpression(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$", ErrorMessage = "Invalid email format")]
        
        public string CompanyEmail { get; set; }




        [Required(ErrorMessage = "Password is required.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!#%?&])[A-Za-z\d@$!#%?&]{8,20}$", ErrorMessage = "Password must be between 8 and 20 characters long, contain at least one uppercase letter, one lowercase letter, one digit, and one special character.")]

        public string Password { get; set; }
        [Required(ErrorMessage = "Password is required.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!#%?&])[A-Za-z\d@$!#%?&]{8,20}$", ErrorMessage = "Password must be between 8 and 20 characters long, contain at least one uppercase letter, one lowercase letter, one digit, and one special character.")]

        public string ConfirmPassword { get; set; }

        [RegularExpression(@"^\d+$", ErrorMessage = "Phone number must contain only digits")]
        public string ContactNumber { get; set; }
        [Required(ErrorMessage = "Company type is Required")]
        [MaxLength(100, ErrorMessage = "Company type cannot exceed 100 characters")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Company type can only contain letters")]

        public string CompanyType { get; set; }
        [Required(ErrorMessage = "country type is Required")]
        [MaxLength(100, ErrorMessage = "country type cannot exceed 100 characters")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "country type can only contain letters")]


        public string country { get; set; }
        [Required(ErrorMessage = "Governate  is Required")]
        [MaxLength(100, ErrorMessage = "Governate cannot exceed 100 characters")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Governate can only contain letters")]

        public string Governate { get; set; }
        [Required(ErrorMessage = "stree is Required")]
        [MaxLength(100, ErrorMessage = "stree cannot exceed 100 characters")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Street can only contain letters")]
        public string street { get; set; }
        public string? TaxID { get; set; }

        public string? websiteURL { get; set; }

        public string? FacebookLink { get; set; }
        public string? TikTokLink { get; set; }
        public string? InstgrameLink { get; set; }
        public string? TwitterLink { get; set; }
        public string? AdditionalInformation { get; set; }

        public IFormFile? BusinessLicense { get; set; }

    }
}
