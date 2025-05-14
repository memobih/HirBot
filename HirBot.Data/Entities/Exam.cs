using HirBot.Data.Enums;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
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
    public class Exam : AuditEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public string Name { get; set; }
        [JsonIgnore]
        public ExamType Type { get; set; }
        [JsonIgnore]

        public int Points { get; set; }
        [ForeignKey("UserSkill")]
        [JsonIgnore]

        public int ? UserSkillID { get; set; }
        [JsonIgnore]

        public virtual  UserSkill ? UserSkill { get; set; }
        [JsonIgnore]

        public int duration { get; set; }
        [JsonIgnore]

        public virtual List<Interview >   ?Interviews { get; set; }=new List<Interview>();
        [JsonIgnore]

        public virtual List<Question >  ? Questions { get; set; } = new List<Question>();
        [JsonIgnore]

        public bool IsAnswerd { get; set; }
    }
}
