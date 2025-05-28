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

        public NotificationType Notifiable_Type {  get; set; } 
         
        public string? Notifiable_ID {  get; set; } 
         public string ? massage { get ; set; } 
         
        public int ? type { get; set; }
       public  ICollection<NotificationReciver>Recivers { get; set; }
    }
}
