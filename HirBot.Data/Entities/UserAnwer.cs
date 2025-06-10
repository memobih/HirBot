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
    public class UserAnwer : AuditEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public string? TextAnwer { get; set; }

        public int Point { get; set; }
        [ForeignKey("Option")]
        public int? OptionID { get; set; }
        [ForeignKey("User")]
        public string ? UserID { get; set; }
        [ForeignKey("Question")]
        public int QuestionID { get; set; }
        public virtual Question Question { get; set; }
        public virtual Option? Option { get; set; }
        public virtual ApplicationUser User {get; set;}

    }   
}
