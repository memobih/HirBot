using HirBot.Data.Entities;
using HirBot.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jop.Services.Responses
{
    public class Applications
    {
        public string name { get; set; }
        public string email { get; set; }
        public int Score { get; set; }
        public ApplicationStatus status { get; set; }
        public string ?  CVLink {get; set; } 
        public DateTime created_at { get; set; }
    }
}
