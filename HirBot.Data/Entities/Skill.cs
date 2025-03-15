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
    public  class Skill : AuditEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public string ImagePath { get; set; }
        //public virtual ICollection<JobRequirment>? JobRequirments { get; set; }
        [JsonIgnore]
        public virtual ICollection<UserSkill> ?UserSkills { get; set; }
    }
}
