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
 MinLength(6, ErrorMessage = "Name must be at least 6 characters long")]
        public string CompanyName { get; set; }
        [Required(ErrorMessage = "Email is required"), DataType(DataType.EmailAddress, ErrorMessage = "Invalid email format")]
        public string CompanyEmail { get; set; }
        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password, ErrorMessage = "Invalid password format")]

        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm password is required")]
        [DataType(DataType.Password, ErrorMessage = "Invalid password format")]
        [Compare("Password", ErrorMessage = "Password and Confirm Password do not match")]

        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Phone number is required"), MaxLength(11, ErrorMessage = "Phone number cannot exceed 11 digits")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Phone number must contain only digits")]
        public string ContactNumber { get; set; }
        [Required(ErrorMessage ="Company type is Required")]
        public string CompanyType { get; set; }
        [Required(ErrorMessage = "country type is Required")]

        public string country { get; set; }
        [Required(ErrorMessage = "Governate  is Required")]

        public string Governate { get; set; }
        [Required(ErrorMessage = "stree is Required") ] 
        public string street { get; set; }
        public string  ? TaxIndtefierNumber { get; set; }

        public string  ? websiteUrl { get; set; }

        public string  ? SocialMeediaLink { get; set; }

        public string  ? Comments { get; set; }
       
        public IFormFile? BusinessLicense { get; set; }

    }
}
