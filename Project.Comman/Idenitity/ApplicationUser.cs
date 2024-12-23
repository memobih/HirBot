
using Microsoft.AspNetCore.Identity;
using HirBot.Comman.Enums;
using System.ComponentModel.DataAnnotations;

namespace HirBot.Comman.Idenitity
{
    public class ApplicationUser : IdentityUser
    {
        [Required, MaxLength(1000)]
        public string FullName { get; set; }
        [Required, MaxLength(11)]
        [RegularExpression(@"^\d+$")]
        public string PhoneNumber { get; set; }
        public UserType UserType { get; set; }
        public List<RefreshToken> refreshTokens { get; set; }
    }
}
