using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace HirBot.Comman.Idenitity
{
    [Owned]
    public class RefreshToken
    {
        public string token { get; set; }
        public DateTime expirationOn { get; set; }

        public bool isActive =>
TimeZoneInfo.ConvertTimeFromUtc(
    DateTime.UtcNow,
    TimeZoneInfo.FindSystemTimeZoneById(
        RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? "Egypt Standard Time"
            : "Africa/Cairo"
    )
)<=expirationOn;
        public DateTime CreatedOn { get; set; }
    }
}
