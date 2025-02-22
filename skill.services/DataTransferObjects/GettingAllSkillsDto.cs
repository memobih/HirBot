using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace skill.services.DataTransferObjects
{
    public class GettingAllSkillsDto
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string ImagePath { get; set; }
        public int TotalUsers { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}