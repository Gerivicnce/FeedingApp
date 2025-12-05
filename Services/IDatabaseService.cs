using FeedingApp.Models;

namespace FeedingApp.Services
{
    public interface IDatabaseService
    {
        Task<List<Animal>> GetAnimalsAsync();
        Task<Animal?> GetAnimalAsync(int id);
        Task<int> SaveAnimalAsync(Animal animal);
        Task<int> DeleteAnimalAsync(Animal animal);

        Task<FeedingEvent?> GetEventAsync(int id);
        Task<List<FeedingEvent>> GetEventsByAnimalAsync(int animalId);
        Task<List<FeedingEvent>> GetEventsByAnimalAndDateAsync(int animalId, DateTime date);
        Task<int> SaveEventAsync(FeedingEvent feedingEvent);
        Task<int> DeleteEventAsync(FeedingEvent feedingEvent);
        Task<List<FeedingEvent>> GetAllEventsAsync();
    }
}
