using HirBot.Comman.Idenitity;
using Moujam.Casiher.Comman.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
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
  
        public bool privacy { get; set; }
        [ForeignKey("User")]
        public string UserID { get; set; } 
        public virtual ApplicationUser User { get; set; }
    }
}
