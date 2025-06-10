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
    public class UserSkill : AuditEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [ForeignKey("User")]
        public string UserID { get; set; }

        [ForeignKey("Skill")]
        public int SkillID { get; set; }


        public int Rate { get; set; }
        public virtual Skill   Skill { get; set; }
        public virtual ApplicationUser   User { get; set; } 
        public virtual ICollection<Exam>  ? Exams { get; set; }
        public DateTime  ? Delete_at { get; set; }
    }
}
