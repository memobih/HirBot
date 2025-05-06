using HirBot.Data.Enums;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
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
    public class Exam : AuditEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public string Name { get; set; }

        public ExamType Type { get; set; }
         
       public int Points { get; set; }
        [ForeignKey("UserSkill")]
        public int UserSkillID { get; set; }

        public virtual  UserSkill ? UserSkill { get; set; }
        public int duration { get; set; }
        public virtual ICollection<Interview >   ?Interviews { get; set; }
        public virtual ICollection<Question > Questions { get; set; }

        public bool IsAnswerd { get; set; } = false; 
    }
}
