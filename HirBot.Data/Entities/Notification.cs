using HirBot.Comman.Idenitity;
using HirBot.Data.Enums;
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
    public  class Notification : AuditEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [ForeignKey("User")]
        public string UserID { get; set;  } 

        public virtual ApplicationUser User {  get; set; }
         
        public NotificationType type {  get; set; } 
         
        public int ReferenceID {  get; set; } 
         public string ? massage { get ; set; }  


       public  ICollection<Notification>  ?Notifications { get; set; }


    }
}
