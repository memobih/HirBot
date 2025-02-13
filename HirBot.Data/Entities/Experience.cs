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
        public string employeeType { get; set; }
        public string  Location {  get; set; }
        public string? Start_Date { get; set; }
        public string? End_Date { get; set; }
          public bool privacy { get; set; }
        [ForeignKey("User")]
        [JsonIgnore]

        public string ?  UserID { get; set; }
        [JsonIgnore]
        public virtual ApplicationUser ?  User { get; set; }
    } 

}
