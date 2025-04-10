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
        public string id { get; set; }
        public string name {  get; set; } 
        public string email { get; set; }
        public string contact {  get; set; }
        public string category { get; set; }

        public string country { get; set; } 
        public string  ? logo { get; set; }
        public string? TaxNumber { get; set; }
        public CompanyStatus status { get; set; }

    }

}
