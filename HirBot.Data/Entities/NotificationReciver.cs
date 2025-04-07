using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using HirBot.Comman.Idenitity;
using Moujam.Casiher.Comman.Base;

namespace HirBot.Data.Entities
{
    public  class NotificationReciver : AuditEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [ForeignKey("User")]
        public string ReciverID { get; set; }

        [ForeignKey("Notification")]
        public int NotificationID { get; set; } 
        public DateTime  ? read_at { get; set; }
        public virtual ApplicationUser User { get; set; }
        public virtual Notification Notification { get; set; } 
        


    }
}
