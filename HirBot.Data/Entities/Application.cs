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
        public class Application : AuditEntity
        {
            [Key]
            [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
            public int ID { get; set; }
            [ForeignKey("User")]
            public string UserID { get; set; }
            [ForeignKey("Job")]
            public int  JopID { get; set; } 
            public ApplicationStatus status { get; set; }
            public virtual Job Job { get; set; }
            public virtual ApplicationUser User { get; set; } 
        
            public virtual ICollection<Interview>? Interviews { get; set; }= new List<Interview>();
        }
    }
