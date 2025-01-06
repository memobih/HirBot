using HirBot.Comman.Idenitity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HirBot.Data.Entities
{
    public  class User : ApplicationUser
    { 
       public  Company company {  get; set; }  

    }

}
