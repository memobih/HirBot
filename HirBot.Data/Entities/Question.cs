﻿using HirBot.Data.Enums;
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
    public class Question : AuditEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [ForeignKey("Exam")]
       public int ExamID { get; set; } 
        public string Content { get; set; }
        public int points { get; set; }

        public QuestionType QuestionType { get; set; }

        public virtual Exam Exam { get; set; } 
        public ICollection<Option> ? Options { get; set; }
        public ICollection<UserAnwer> ? UserAnwers { get; set; }
    }

}