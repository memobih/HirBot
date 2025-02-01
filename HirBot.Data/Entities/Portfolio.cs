using HirBot.Comman.Idenitity;
using HirBot.Data.Enums;
using Microsoft.AspNetCore.Http.HttpResults;
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
    public class Portfolio : AuditEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public string? PortfolioUrl { get; set; }
        public string? Title { get; set; }
        public string? location { get; set; }
        public string? CVUrl { get; set; }
        public string? GithubUrl { get; set; }
        public string? MainUrl { get; set; }
        public bool Visibility { get; set; }
        public bool IsAutoAplly { get; set; }
        [ForeignKey("User")]
        public string UserID { get; set; }
        public virtual ApplicationUser User {get; set; }
    }
}
