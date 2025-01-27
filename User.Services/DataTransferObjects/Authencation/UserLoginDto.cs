using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace User.Services.DataTransferObjects.Authencation
{
    public class UserLoginDto
    {
        public string userId { get; set; }
        public string LoginProvider { get; set; }
        public string ProviderKey { get; set; }
        public string ProviderDisplayName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
       public DateTime LoginTime { get; set; }  
    }
}