using FeedingApp.Models;
using FeedingApp.Services;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FeedingApp.ViewModels
{
    public class CalendarViewModel : INotifyPropertyChanged
    {
        private readonly DatabaseService _db;

        public ObservableCollection<Animal> Animals { get; } = new();
        public ObservableCollection<FeedingEvent> EventsForSelectedDate { get; } = new();

        private DateTime _selectedDate = DateTime.Today;
        public DateTime SelectedDate
        {
            get => _selectedDate;
            set
            {
                if (_selectedDate != value)
                {
                    _selectedDate = value;
                    OnPropertyChanged(nameof(SelectedDate));
                }
            }
        }

        private Animal? _selectedAnimal;
        public Animal? SelectedAnimal
        {
            get => _selectedAnimal;
            set
            {
                if (_selectedAnimal != value)
                {
                    _selectedAnimal = value;
                    OnPropertyChanged(nameof(SelectedAnimal));
                }
            }
        }

        private double? _currentWeight;
        public double? CurrentWeight
        {
            get => _currentWeight;
            set
            {
                if (_currentWeight != value)
                {
                    _currentWeight = value;
                    OnPropertyChanged(nameof(CurrentWeight));
                }
            }
        }

        private string _currentPhotoPath = string.Empty;
        public string CurrentPhotoPath
        {
            get => _currentPhotoPath;
            set
            {
                if (_currentPhotoPath != value)
                {
                    _currentPhotoPath = value;
                    OnPropertyChanged(nameof(CurrentPhotoPath));
                }
            }
        }

        public ICommand LoadCommand { get; }
        public ICommand LoadEventsCommand { get; }
        public ICommand SaveFeedingCommand { get; }

        public event PropertyChangedEventHandler? PropertyChanged;

        public CalendarViewModel(DatabaseService db)
        {
            _db = db;

            LoadCommand = new Command(async () => await LoadAsync());
            LoadEventsCommand = new Command(async () => await LoadEventsAsync());
            SaveFeedingCommand = new Command(async () => await SaveFeedingAsync());
        }

        private async Task LoadAsync()
        {
            await LoadAnimalsAsync();
            await LoadEventsAsync();
        }

        private async Task LoadAnimalsAsync()
        {
            Animals.Clear();
            var animals = await _db.GetAnimalsAsync();
            foreach (var a in animals)
                Animals.Add(a);
        }

        private async Task LoadEventsAsync()
        {
            EventsForSelectedDate.Clear();
            var all = await _db.GetAllEventsAsync();

            foreach (var e in all)
            {
                if (e.FeedingTime.Date == SelectedDate.Date)
                    EventsForSelectedDate.Add(e);
            }
        }

        private async Task SaveFeedingAsync()
        {
            if (SelectedAnimal == null) return;

            var ev = new FeedingEvent
            {
                AnimalId = SelectedAnimal.Id,
                FeedingTime = SelectedDate,
                WeightGrams = CurrentWeight,
                PhotoPath = CurrentPhotoPath
            };

            await _db.SaveEventAsync(ev);
            await LoadEventsAsync();

            // űrlap resetelése
            CurrentWeight = null;
            CurrentPhotoPath = string.Empty;
        }

        private void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
