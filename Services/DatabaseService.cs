using FeedingApp.Models;
using Microsoft.Maui.Storage;
using SQLite;

namespace FeedingApp.Services
{
    public class DatabaseService : IDatabaseService
    {
        private readonly SQLiteAsyncConnection _db;

        public DatabaseService()
        {
            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "feeding.db3");
            _db = new SQLiteAsyncConnection(dbPath);
            _db.CreateTableAsync<Animal>().Wait();
            _db.CreateTableAsync<FeedingEvent>().Wait();
        }

        public Task<List<Animal>> GetAnimalsAsync() =>
            _db.Table<Animal>().ToListAsync();

        public Task<Animal?> GetAnimalAsync(int id) =>
            _db.FindAsync<Animal>(id);

        public Task<int> SaveAnimalAsync(Animal animal) =>
            animal.Id == 0
                ? _db.InsertAsync(animal)
                : _db.UpdateAsync(animal);

        public async Task DeleteAnimalAsync(Animal animal)
        {
            if (animal.Id == 0)
                return;

            await _db.Table<FeedingEvent>().DeleteAsync(e => e.AnimalId == animal.Id);
            await _db.Table<Animal>().DeleteAsync(a => a.Id == animal.Id);
        }

        public Task<FeedingEvent?> GetEventAsync(int id) =>
            _db.FindAsync<FeedingEvent>(id);

        public Task<List<FeedingEvent>> GetEventsByAnimalAsync(int animalId) =>
            _db.Table<FeedingEvent>()
               .Where(e => e.AnimalId == animalId)
               .ToListAsync();

        public Task<int> SaveEventAsync(FeedingEvent e) =>
            e.Id == 0
                ? _db.InsertAsync(e)
                : _db.UpdateAsync(e);

        public Task<int> DeleteEventAsync(FeedingEvent e) =>
            _db.DeleteAsync(e);

        public Task<List<FeedingEvent>> GetAllEventsAsync() =>
            _db.Table<FeedingEvent>().ToListAsync();
    }
}
