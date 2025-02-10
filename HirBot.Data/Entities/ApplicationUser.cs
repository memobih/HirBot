
using Microsoft.AspNetCore.Identity;
using HirBot.Comman.Enums;
using System.ComponentModel.DataAnnotations;
using HirBot.Data.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace HirBot.Comman.Idenitity
{
    public class ApplicationUser : IdentityUser
    {
        [Required, MaxLength(1000)]
        public string FullName { get; set; }
        [MaxLength(11)]
        [RegularExpression(@"^\d+$")]
        public string ?  PhoneNumber  { get; set; }
        public UserType role { get; set; }
        public string ? ImagePath { get; set; } 
        public string  ?CoverPath { get; set; }
         public string ? CompanyID { get; set; }
        public int ?VerificationCode {  get; set; } 
         public DateTime ? Code_Send_at { get; set; }
        public bool ?IsVerified { get; set; }
        public List<RefreshToken> refreshTokens { get; set; }
        [InverseProperty("User")]
        public virtual  Portfolio  ?  Portfolio { get; set; }

    }
}
