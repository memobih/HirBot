using HirBot.Comman.Idenitity;
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
    public class Category  : AuditEntity
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] 
        public int ID { get; set; } 
        public string Name { get; set; }
        public string  ? Description { get; set; }
        
        public string  ? image {  get; set; }
        [ForeignKey("user")]
        [JsonIgnore]
        public string UserID { get; set; }
        [JsonIgnore]
        public virtual ApplicationUser user {  get; set; }


        public virtual List<Exam>  ? exams { get; set;  } = new List<Exam>();
    }
}
