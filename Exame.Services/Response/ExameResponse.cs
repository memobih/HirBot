using HirBot.Data.Entities;
using Microsoft.Extensions.DependencyModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exame.services.Response
{
    public class ExameResponse
    {
        public int id { get; set; }
        public List<Questions> Questions { get; set; } = new List<Questions>();
        public int QuestionsNumber { get; set; }
        public string skill { get; set; }
        public string name {  get; set; }
        public string? icon {  get; set; }
        public int duration {  get; set; }
        public string level;
        public int points { get; set; }
    } 
    public class Questions
    {
        public int id { get; set; }
        public string question {  get; set; }

        public List<Options> options { get; set; } =new List<Options>();
    }
    public class Options
    {
        public int id { get; set; } 
         public string option { get; set; }
    }

}
