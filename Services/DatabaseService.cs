using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using FeedingApp.Models;

namespace FeedingApp.Services
{
    public class DatabaseService
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

        public async Task<Animal?> GetAnimalAsync(int id) =>
            await _db.FindAsync<Animal>(id);

        public Task<int> SaveAnimalAsync(Animal animal)
        {
            if (animal.Id == 0)
                return _db.InsertAsync(animal);
            return _db.UpdateAsync(animal);
        }

        public async Task DeleteAnimalAsync(Animal animal)
        {
            if (animal == null || animal.Id == 0)
                return;

            await _db.Table<FeedingEvent>().DeleteAsync(e => e.AnimalId == animal.Id);
            // Delete by entity instance to ensure the primary key is respected even
            // if SQLite-net changes the overload resolution.
            await _db.DeleteAsync(animal);
        }

        // ---- FeedingEvent CRUD ----
        public async Task<FeedingEvent?> GetEventAsync(int id) =>
            await _db.FindAsync<FeedingEvent>(id);

        public Task<List<FeedingEvent>> GetEventsByAnimalAsync(int animalId) =>
            _db.Table<FeedingEvent>()
               .Where(e => e.AnimalId == animalId)
               .ToListAsync();

        public Task<int> SaveEventAsync(FeedingEvent e)
        {
            if (e.Id == 0)
                return _db.InsertAsync(e);
            return _db.UpdateAsync(e);
        }

        public Task<int> DeleteEventAsync(FeedingEvent e) =>
            _db.DeleteAsync(e);

        public Task<List<FeedingEvent>> GetAllEventsAsync() =>
            _db.Table<FeedingEvent>().ToListAsync();
    }
}
