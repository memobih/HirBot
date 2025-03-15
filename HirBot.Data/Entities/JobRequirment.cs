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
    public class JobRequirment : AuditEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [ForeignKey("Jop")]
        public int JopID { get; set; }
        [ForeignKey("Skill")]
        public int SkillID { get; set; }
        [ForeignKey("Level")]
        public int LevelID { get; set; }
        public  Job Jop { get; set; }
        public virtual Skill Skill { get; set; }
        public virtual Level Level { get; set; }
    }
}
