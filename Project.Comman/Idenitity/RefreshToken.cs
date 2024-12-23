using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HirBot.Comman.Idenitity
{
    [Owned]
    public class RefreshToken
    {
        public string token { get; set; }
        public DateTime expirationOn { get; set; }

        public bool isActive => DateTime.UtcNow <=expirationOn;
        public DateTime CreatedOn { get; set; }
    }
}
