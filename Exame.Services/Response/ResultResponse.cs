using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exame.Services.Response
{
    public class ResultResponse
    {
        public string skill {  get; set; }
        public int CorrectQuestion { get; set; }
        public int TotalQuestion { get; set; }
    }
}
