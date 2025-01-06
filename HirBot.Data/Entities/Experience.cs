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
    public  class Experience : AuditEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public string Title { get; set; }
        public EmployeeType employeeType { get; set; }
        public LocationType Location {  get; set; }
        public string? Start_Date { get; set; }
        public string? End_Date { get; set; }
        [ForeignKey("User")]
        public string UserID { get; set; }
         
        public virtual ApplicationUser User { get; set; }
    } 

}
