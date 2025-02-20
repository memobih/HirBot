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
    public class Education : AuditEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public string ? InstituationName {  get; set; }
         public string ? FieldOfStudy { get; set; } 
         
        public string ? Start_Date { get; set; } 
        public string? End_Date { get;set; }
        public string  ? degree { get; set; }
        public EducationType ?  Type { get; set; }
        public bool Privacy { get; set; }
        [ForeignKey("User")]
        [JsonIgnore]
        public string UserID { get; set; }
        [JsonIgnore]  
        public virtual ApplicationUser User { get; set; }
        public bool isGraduated { get; set; }
        public string? logo { get; set; }
    }
}
