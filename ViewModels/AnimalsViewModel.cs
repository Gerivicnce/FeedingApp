using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using FeedingApp.Models;
using FeedingApp.Services;

namespace FeedingApp.ViewModels
{
    public class AnimalsViewModel : INotifyPropertyChanged
    {
        private readonly DatabaseService _db;

        public ObservableCollection<Animal> Animals { get; } = new();
        private Animal? _selectedAnimal;
        public Animal? SelectedAnimal
        {
            get => _selectedAnimal;
            set { _selectedAnimal = value; OnPropertyChanged(); }
        }

        public ICommand LoadCommand { get; }
        public ICommand AddAnimalCommand { get; }
        public ICommand EditAnimalCommand { get; }
        public ICommand DeleteAnimalCommand { get; }

        public event PropertyChangedEventHandler? PropertyChanged;

        public AnimalsViewModel(DatabaseService db)
        {
            _db = db;
            LoadCommand = new Command(async () => await LoadAsync());
            AddAnimalCommand = new Command(async () => await AddAnimalAsync());
            EditAnimalCommand = new Command<Animal>(async a => await EditAnimalAsync(a));
            DeleteAnimalCommand = new Command<Animal>(async a => await DeleteAnimalAsync(a));
        }

        private async Task LoadAsync()
        {
            Animals.Clear();
            var animals = await _db.GetAnimalsAsync();
            foreach (var a in animals)
                Animals.Add(a);
        }

        private async Task AddAnimalAsync()
        {
            // Pl. egy külön Edit/Detail page navigáció, ShellNavigation mintára a példából
            await Shell.Current.GoToAsync("EditAnimalPage");
        }

        private async Task EditAnimalAsync(Animal? animal)
        {
            if (animal == null) return;
            var route = $"EditAnimalPage?animalId={animal.Id}";
            await Shell.Current.GoToAsync(route);
        }

        private async Task DeleteAnimalAsync(Animal? animal)
        {
            if (animal == null)
                return;

            var debugInfo = await _db.DeleteAnimalWithDebugAsync(animal);
            Animals.Remove(animal);
            SelectedAnimal = null;

            var summary = new StringBuilder();
            summary.AppendLine(debugInfo.Summary ?? "Deletion attempted.");
            summary.AppendLine($"Database: {debugInfo.DatabasePath}");
            summary.AppendLine($"Animal Id: {debugInfo.AnimalId}");
            summary.AppendLine($"Existed before delete: {debugInfo.ExistedBefore}");
            summary.AppendLine($"Animals before/after: {debugInfo.AnimalsBefore} -> {debugInfo.AnimalsAfter}");
            summary.AppendLine($"Events for animal before/after: {debugInfo.EventsForAnimalBefore} -> {debugInfo.EventsForAnimalAfter}");
            summary.AppendLine($"Total events before/after: {debugInfo.FeedingEventsBefore} -> {debugInfo.FeedingEventsAfter}");
            summary.AppendLine($"Events removed: {debugInfo.FeedingEventsRemoved}");
            summary.AppendLine($"Animal rows removed: {debugInfo.AnimalRowsRemoved}");

            await Shell.Current.DisplayAlert("Delete debug", summary.ToString(), "OK");
        }

        protected void OnPropertyChanged([CallerMemberName] string? name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
