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
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Name can only contain letters")]
        [DataType(DataType.Text, ErrorMessage = "Invalid name format")]
        public string CompanyName { get; set; }
        [Required(ErrorMessage = "Email is required"), DataType(DataType.EmailAddress, ErrorMessage = "Invalid email format")]
        [MaxLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
        [RegularExpression(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$", ErrorMessage = "Invalid email format")]
        
        public string CompanyEmail { get; set; }
        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password, ErrorMessage = "Invalid password format")]
        [MaxLength(40, ErrorMessage = "Password cannot exceed 40 characters")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm password is required")]
        [DataType(DataType.Password, ErrorMessage = "Invalid password format")]
        [Compare("Password", ErrorMessage = "Password and Confirm Password do not match")]

        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Phone number is required"), MaxLength(11, ErrorMessage = "Phone number cannot exceed 11 digits")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Phone number must contain only digits")]
        public string ContactNumber { get; set; }
        [Required(ErrorMessage = "Company type is Required")]
        [MaxLength(100, ErrorMessage = "Company type cannot exceed 100 characters")]
        [MinLength(6, ErrorMessage = "Company type must be at least 6 characters long")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Company type can only contain letters")]

        public string CompanyType { get; set; }
        [Required(ErrorMessage = "country type is Required")]
        [MaxLength(100, ErrorMessage = "country type cannot exceed 100 characters")]
        [MinLength(6, ErrorMessage = "country type must be at least 6 characters long")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "country type can only contain letters")]


        public string country { get; set; }
        [Required(ErrorMessage = "Governate  is Required")]
        [MaxLength(100, ErrorMessage = "Governate cannot exceed 100 characters")]
        [MinLength(6, ErrorMessage = "Governate must be at least 6 characters long")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Governate can only contain letters")]

        public string Governate { get; set; }
        [Required(ErrorMessage = "stree is Required")]
        [MaxLength(100, ErrorMessage = "stree cannot exceed 100 characters")]
        [MinLength(6, ErrorMessage = "stree must be at least 6 characters long")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Street can only contain letters")]
        public string street { get; set; }
        public string? TaxIndtefierNumber { get; set; }

        public string? websiteUrl { get; set; }

        public string? SocialMeediaLink { get; set; }

        public string? Comments { get; set; }

        public IFormFile? BusinessLicense { get; set; }

    }
}
