using HirBot.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jop.Services.Responses
{
    public class CompanyList
    {
        public string name {  get; set; } 
        public string email { get; set; }
        public string Contact {  get; set; }
        public string Category { get; set; }

        public string Country { get; set; } 

        public string? TaxNumber { get; set; }
        public CompanyStatus status { get; set; }

    }

}
