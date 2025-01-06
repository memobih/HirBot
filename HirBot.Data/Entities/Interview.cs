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
    public class Interview : AuditEntity
    {

        [Key ]
        public string ID { get; set; } = Guid.NewGuid().ToString();

        [ForeignKey("Exam")]
        public int ? ExamID {  get; set; }
        [ForeignKey("Application")]
        public int ApplicationID { get; set; } 
        public virtual Exam ?Exam { get; set; } 
        public virtual Application Application { get; set; }
        public  DateTime Date {  get; set; }
        public string Time {  get; set; } 
        public string Location { get; set; }
        public string ? Notes { get; set; }
        

    }
}
