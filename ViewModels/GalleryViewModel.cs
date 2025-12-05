using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;
using FeedingApp.Models;
using FeedingApp.Services;
using Microsoft.Maui.ApplicationModel.DataTransfer;

namespace FeedingApp.ViewModels
{
    public class GalleryViewModel
    {
        private readonly IDatabaseService _db;

        public ObservableCollection<FeedingEvent> Photos { get; } = new();

        public ICommand LoadPhotosCommand { get; }
        public ICommand ShareCommand { get; }

        public GalleryViewModel(IDatabaseService db)
        {
            _db = db;
            LoadPhotosCommand = new Command(async () => await LoadAsync());
            ShareCommand = new Command<string>(async path => await ShareAsync(path));
        }

        private async Task LoadAsync()
        {
            Photos.Clear();
            var all = await _db.GetAllEventsAsync();
            foreach (var e in all.Where(e => !string.IsNullOrEmpty(e.PhotoPath)))
                Photos.Add(e);
        }

        private async Task ShareAsync(string path)
        {
            if (File.Exists(path))
            {
                await Share.Default.RequestAsync(new ShareFileRequest
                {
                    Title = "Etetési fotó",
                    File = new ShareFile(path)
                });
            }
        }
    }
}
