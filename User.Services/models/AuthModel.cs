using HirBot.Comman.Enums;
using HirBot.Comman.Idenitity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
namespace User.Services.models
{
    public class AuthModel
    {
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string Role { get; set; }
         public string id { get; set; }
        [JsonIgnore]
        public string? Token { get; set; }
        [JsonIgnore]
        public DateTime? ExpiresOn { get; set; }
        [JsonIgnore]
       public string? RefreshToken { get; set; }

    }
}
