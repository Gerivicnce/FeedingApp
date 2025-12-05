using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using FeedingApp.Models;
using FeedingApp.Services;
using FeedingApp.Views;

namespace FeedingApp.ViewModels
{
    public class AnimalsViewModel : INotifyPropertyChanged
    {
        private readonly IDatabaseService _db;

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

        public AnimalsViewModel(IDatabaseService db)
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

        private static Task AddAnimalAsync() =>
            Shell.Current.GoToAsync(nameof(Views.EditAnimalPage));

        private static Task EditAnimalAsync(Animal? animal)
        {
            if (animal == null) return Task.CompletedTask;
            var route = $"{nameof(Views.EditAnimalPage)}?animalId={animal.Id}";
            return Shell.Current.GoToAsync(route);
        }

        private async Task DeleteAnimalAsync(Animal? animal)
        {
            if (animal == null)
                return;

            await _db.DeleteAnimalAsync(animal);
            Animals.Remove(animal);
            SelectedAnimal = null;
        }

        protected void OnPropertyChanged([CallerMemberName] string? name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
