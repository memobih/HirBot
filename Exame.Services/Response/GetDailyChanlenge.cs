using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exame.Services.Response
{
    public  class GetDailyChanlenge
    {
        public int ExameID { get; set; }
        public string  ? skill {  get; set; }
        public string ? level { get; set; }
        public int Rate { get; set; }   
       public string  ? logo { get; set; }
      public  int   exameDuration {  get; set; } 


    }
}
