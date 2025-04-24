using HirBot.Comman.Idenitity;
using HirBot.Data.Enums;
using Moujam.Casiher.Comman.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HirBot.Data.Entities
{
    public  class Company : AuditEntity
    {
        [Key,MaxLength(200)]
        public string ID { get; set; }=Guid.NewGuid().ToString();
        [Required(ErrorMessage = "Company name is Required")]
        [MaxLength(200)]
        public string? Name { get; set; }
        [Required(ErrorMessage = "Company type is Required")]
        public string CompanyType { get; set; }
        public string country { get; set; }
        [Required(ErrorMessage = "Governate  is Required")]

        public string Governate { get; set; }
        [Required(ErrorMessage = "stree is Required")]
        public string street { get; set; }
        public string? TaxIndtefierNumber { get; set; }
        public CompanyStatus status { get; set; }
        public string? websiteUrl { get; set; }

        public string? SocialMeediaLink { get; set; }

        public string? Comments { get; set; }

        public string ? BusinessLicense { get; set; }

        public string ?Logo {  get; set; }
        [ForeignKey("account")]
        [JsonIgnore]
        public string UserID { get; set; }
        [JsonIgnore]
        public virtual ApplicationUser account { get; set; } 
        public virtual  List<Job> ? jobs { get; set; }



    }
}
