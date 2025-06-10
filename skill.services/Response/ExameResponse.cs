using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skill.services.Response
{
    public class ExameResponse
    {
        public Question Questions { get; set; }
        public int  QuestionsNumber {get; set; }
        public string skill;
        public string name;
        public string icon;

        public string level; 
        public int points { get; set; }
    } 
    public class Question
    {
        public int id;
        public string question {  get; set; }

        public List<Option> ? options { get; set; }
    }
    public class Option
    {
        public int id { get; set; } 
         public string option { get; set; }
    }

}
