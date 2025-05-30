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
    public class Job : AuditEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public string Title { get; set; }
        public string ? Description { get; set; }

        public string location { get; set; }
        public int ?   Salary { get; set; }
        public LocationType LocationType { get; set; }
        public EmployeeType EmployeeType { get; set; }
        public JobStatus status {  get; set; }

        [ForeignKey("Company")]
         public string CompanyID { get; set; }
        public virtual Company Company { get; set; }
        [InverseProperty("Job")]
        public virtual ICollection<Application> ?Applications { get; set; } 
         
        
        public virtual List<JobRequirment>? JobRequirments { get; set; } 

        public string  ? Experience {  get; set; }
    } 

}
