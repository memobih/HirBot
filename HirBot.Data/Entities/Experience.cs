using HirBot.Comman.Idenitity;
using HirBot.Data.Enums;
using Moujam.Casiher.Comman.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HirBot.Data.Entities
{
    public  class Experience : AuditEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public string Title { get; set; }
        public EmployeeType employeeType { get; set; }
        public LocationType workType {  get; set; } 
        public string  ?  location { get; set; }
        public DateTime? Start_Date { get; set; }
        public DateTime? End_Date { get; set; } 
        public string  ? companyName { get; set; } 

        public PrivacyEnum privacy { get; set; }
        [ForeignKey("Company")]
        public string ? CompanyID { get; set; } 
         public int rate { get; set; }
        [ForeignKey("User")]
        [JsonIgnore]

        public string ?  UserID { get; set; }
        [JsonIgnore]
        public virtual ApplicationUser ?  User { get; set; }

        [JsonIgnore]
        public virtual Company ?  Company { get; set; }
        [JsonIgnore]
        [InverseProperty("CurentJop")]
        public virtual  ApplicationUser ? UserJop { get; set; }  

        public bool IsStill { get; set; }

    } 

}
