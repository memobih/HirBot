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
    public  class Option : AuditEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public string Content { get; set; } 

        public bool IsCorrect { get; set; }

        [ForeignKey("Question")]
        public int QuestionID {get ; set;}

        public virtual Question Question { get; set;} 

        public ICollection<UserAnwer> ?UserAnwers { get; set;}
    }
}
