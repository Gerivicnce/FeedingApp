using FeedingApp.Models;
using FeedingApp.Services;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FeedingApp.ViewModels
{
    public class CalendarViewModel : INotifyPropertyChanged
    {
        private readonly IDatabaseService _db;

        public ObservableCollection<Animal> Animals { get; } = new();
        public ObservableCollection<FeedingEvent> Events { get; } = new();

        private DateTime _selectedDate = DateTime.Today;
        public DateTime SelectedDate
        {
            get => _selectedDate;
            set
            {
                if (_selectedDate != value)
                {
                    _selectedDate = value;
                    OnPropertyChanged();
                    _ = LoadEventsAsync();
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
                    OnPropertyChanged();
                    _ = LoadEventsAsync();
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
                    OnPropertyChanged();
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
                    OnPropertyChanged();
                }
            }
        }

        private string _currentNotes = string.Empty;
        public string CurrentNotes
        {
            get => _currentNotes;
            set
            {
                if (_currentNotes != value)
                {
                    _currentNotes = value;
                    OnPropertyChanged();
                }
            }
        }

        private FeedingEvent? _editingEvent;
        public FeedingEvent? EditingEvent
        {
            get => _editingEvent;
            private set
            {
                if (_editingEvent != value)
                {
                    _editingEvent = value;
                    OnPropertyChanged();
                }
            }
        }

        public ICommand LoadCommand { get; }
        public ICommand LoadEventsCommand { get; }
        public ICommand NewFeedingCommand { get; }
        public ICommand EditFeedingCommand { get; }
        public ICommand SaveFeedingCommand { get; }
        public ICommand DeleteFeedingCommand { get; }

        public event PropertyChangedEventHandler? PropertyChanged;

        public CalendarViewModel(IDatabaseService db)
        {
            _db = db;

            LoadCommand = new Command(async () => await LoadAsync());
            LoadEventsCommand = new Command(async () => await LoadEventsAsync());
            NewFeedingCommand = new Command(StartNewFeeding);
            EditFeedingCommand = new Command<FeedingEvent>(BeginEdit);
            SaveFeedingCommand = new Command(async () => await SaveFeedingAsync());
            DeleteFeedingCommand = new Command<FeedingEvent>(async e => await DeleteEventAsync(e));
        }

        public async Task LoadAsync()
        {
            await LoadAnimalsAsync();

            if (SelectedAnimal == null && Animals.Count > 0)
            {
                SelectedAnimal = Animals[0];
            }

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
            Events.Clear();

            if (SelectedAnimal == null)
                return;

            var events = await _db.GetEventsByAnimalAndDateAsync(SelectedAnimal.Id, SelectedDate);

            foreach (var e in events)
            {
                Events.Add(e);
            }
        }

        private async Task SaveFeedingAsync()
        {
            if (SelectedAnimal == null) return;

            var feedingDate = SelectedDate.Date;

            if (EditingEvent is not null)
            {
                EditingEvent.FeedingTime = feedingDate + EditingEvent.FeedingTime.TimeOfDay;
                EditingEvent.WeightGrams = CurrentWeight;
                EditingEvent.PhotoPath = CurrentPhotoPath;
                EditingEvent.Notes = CurrentNotes;

                await _db.SaveEventAsync(EditingEvent);
            }
            else
            {
                var ev = new FeedingEvent
                {
                    AnimalId = SelectedAnimal.Id,
                    FeedingTime = feedingDate + DateTime.Now.TimeOfDay,
                    WeightGrams = CurrentWeight,
                    PhotoPath = CurrentPhotoPath,
                    Notes = CurrentNotes
                };

                await _db.SaveEventAsync(ev);
            }

            await LoadEventsAsync();

            ClearCurrentEntry();
        }

        public void BeginEdit(FeedingEvent? feedingEvent)
        {
            if (feedingEvent == null)
                return;

            EditingEvent = feedingEvent;
            SelectedDate = feedingEvent.FeedingTime.Date;

            CurrentWeight = feedingEvent.WeightGrams;
            CurrentPhotoPath = feedingEvent.PhotoPath;
            CurrentNotes = feedingEvent.Notes;
        }

        public void StartNewFeeding()
        {
            ClearCurrentEntry();
        }

        private async Task DeleteEventAsync(FeedingEvent? feedingEvent)
        {
            if (feedingEvent == null) return;

            await _db.DeleteEventAsync(feedingEvent);
            await LoadEventsAsync();

            if (EditingEvent?.Id == feedingEvent.Id)
                ClearCurrentEntry();
        }

        private void ClearCurrentEntry()
        {
            EditingEvent = null;
            CurrentWeight = null;
            CurrentPhotoPath = string.Empty;
            CurrentNotes = string.Empty;
        }

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
