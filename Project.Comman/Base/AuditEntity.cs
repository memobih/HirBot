using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Moujam.Casiher.Comman.Base
{
    public class AuditEntity<T> : BaseEntity<T>
    {
        [JsonIgnore]
        public DateTime CreationDate { get; set; } =
    TimeZoneInfo.ConvertTimeFromUtc(
        DateTime.UtcNow,
        TimeZoneInfo.FindSystemTimeZoneById(
            RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                ? "Egypt Standard Time"
                : "Africa/Cairo"
        )
    );
        [JsonIgnore]

        public DateTime? ModificationDate { get; set; }
        [JsonIgnore]

        public string ModifiedBy { get; set; }
        [JsonIgnore]

        public string CreatedBy { get; set; }


    }
    public class AuditEntity
    {

        [JsonIgnore]

        public DateTime CreationDate { get; set; } = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.FindSystemTimeZoneById("Arab Standard Time")) :
             TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.FindSystemTimeZoneById("Asia/Riyadh")); //DateTime.UtcNow;
        [JsonIgnore]

        public DateTime? ModificationDate { get; set; }
        [JsonIgnore]

        public string ModifiedBy { get; set; }
        [JsonIgnore]

        public string CreatedBy { get; set; }
    }
}
