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
    public class Question : AuditEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [ForeignKey("Exam")]
        public int? ExamID { get; set; }
        public string Content { get; set; } 

        public int Points { get; set; }
    
          [JsonIgnore]
         public int ?  SKillID {  get; set; }
        public QuestionType QuestionType { get; set; }

        public virtual Exam    Exam { get; set; } 
        public List<Option>  Options { get; set; } = new List<Option>();
        public List<UserAnwer> ? UserAnwers { get; set; }= new List<UserAnwer>();
    }

}
