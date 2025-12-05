namespace FeedingApp.Models
{
    public class DeleteDebugInfo
    {
        public string? DatabasePath { get; set; }
        public int AnimalId { get; set; }
        public int AnimalsBefore { get; set; }
        public int AnimalsAfter { get; set; }
        public int FeedingEventsBefore { get; set; }
        public int FeedingEventsAfter { get; set; }
        public bool ExistedBefore { get; set; }
        public int EventsForAnimalBefore { get; set; }
        public int EventsForAnimalAfter { get; set; }
        public int FeedingEventsRemoved { get; set; }
        public int AnimalRowsRemoved { get; set; }
        public string? Summary { get; set; }
    }
}
