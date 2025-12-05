using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace FeedingApp.Models
{
    public class FeedingEvent
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public int AnimalId { get; set; }         
        public DateTime FeedingTime { get; set; } 

        public double? WeightGrams { get; set; }  
        public string PhotoPath { get; set; } = string.Empty; 
        public string Notes { get; set; } = string.Empty;
    }
}
