using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeedingApp.Models
{
    public class Animal
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;    
        public string Species { get; set; } = string.Empty; 

        
        public int FeedingIntervalDays { get; set; }

        public string Notes { get; set; } = string.Empty;   
    }
}
