using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Maui.Views;
using FeedingApp.Models;
using FeedingApp.Services;
using FeedingApp.ViewModels;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Storage;
using System;
using System.IO;
using System.Threading;

namespace FeedingApp.Views
{
    public partial class CalendarPage : ContentPage
    {
        private readonly CalendarViewModel _vm;

        public CalendarPage()
        {
            InitializeComponent();

            var db = new DatabaseService();
            _vm = new CalendarViewModel(db);
            BindingContext = _vm;

            // kamera event feliratkozs
            CameraViewControl.MediaCaptured += OnMediaCaptured;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            try
            {
                await _vm.LoadAsync();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Hiba", $"A naptr betltse kzben hiba trtnt: {ex.Message}", "OK");
            }
        }

        private void OnDateSelected(object sender, DateChangedEventArgs e)
        {
            if (_vm.LoadEventsCommand.CanExecute(null))
                _vm.LoadEventsCommand.Execute(null);
        }

        private async void OnTakePhotoClicked(object sender, EventArgs e)
        {
            try
            {
                if (!await EnsureCameraPermissionAsync())
                {
                    await DisplayAlert("Engedély szükséges", "A kamera használatához engedély szükséges.", "OK");
                    return;
                }

                if (!CameraViewControl.IsAvailable)
                {
                    await DisplayAlert("Nem elérhető", "A kamera nem érhető el ezen az eszközön.", "OK");
                    return;
                }

                using var cts = new CancellationTokenSource();
                // Ez csak elindtja a fot ksztst,
                // a stream az OnMediaCaptured-ben jn meg
                await CameraViewControl.CaptureImage(cts.Token);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Hiba", $"Nem sikerlt fott kszteni: {ex.Message}", "OK");
            }
        }

        // Itt kapod meg tnylegesen a fot streamjt
        private async void OnMediaCaptured(object? sender, MediaCapturedEventArgs e)
        {
            try
            {
                var fileName = $"feeding_{DateTime.Now:yyyyMMdd_HHmmss}.jpg";
                var filePath = Path.Combine(FileSystem.AppDataDirectory, fileName);

                await using var stream = e.Media;
                await using var fileStream = File.OpenWrite(filePath);
                await stream.CopyToAsync(fileStream);

                _vm.CurrentPhotoPath = filePath;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Hiba", $"Nem sikerlt fott menteni: {ex.Message}", "OK");
            }
        }

        private async void OnAddFeedingClicked(object sender, EventArgs e)
        {
            _vm.StartNewFeeding();

            var popup = new FeedingPopup(_vm);
            await this.ShowPopupAsync(popup);   // lsd a kvetkez pontot a using-hoz
        }

        private async void OnEditEventClicked(object sender, EventArgs e)
        {
            if (sender is not Button button || button.BindingContext is not FeedingEvent feedingEvent)
                return;

            _vm.BeginEdit(feedingEvent);

            var popup = new FeedingPopup(_vm);
            await this.ShowPopupAsync(popup);
        }

        private async void OnDeleteEventClicked(object sender, EventArgs e)
        {
            if (sender is not Button button || button.BindingContext is not FeedingEvent feedingEvent)
                return;

            try
            {
                var confirm = await DisplayAlert("Megerősítés", "Biztosan törlöd ezt az etetést?", "Igen", "Mégse");
                if (!confirm)
                    return;

                if (_vm.DeleteFeedingCommand.CanExecute(feedingEvent))
                    _vm.DeleteFeedingCommand.Execute(feedingEvent);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Hiba", $"Az etetés törlése nem sikerült: {ex.Message}", "OK");
            }
        }

        private static async Task<bool> EnsureCameraPermissionAsync()
        {
            var status = await Permissions.CheckStatusAsync<Permissions.Camera>();

            if (status == PermissionStatus.Granted)
            {
                return true;
            }

            status = await Permissions.RequestAsync<Permissions.Camera>();
            return status == PermissionStatus.Granted;
        }

    }
}
