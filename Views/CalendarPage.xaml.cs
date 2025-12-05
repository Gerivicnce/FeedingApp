using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Maui.Views;
using FeedingApp.ViewModels;
using Microsoft.Maui.Storage;
using System;
using System.IO;
using System.Threading;
using Microsoft.Maui.ApplicationModel;
using FeedingApp.Models;
using CameraView = CommunityToolkit.Maui.Views.CameraView;

namespace FeedingApp.Views
{
    public partial class CalendarPage : ContentPage
    {
        private readonly CalendarViewModel _vm;
        private readonly CameraView? _cameraView;

        public CalendarPage(CalendarViewModel vm)
        {
            InitializeComponent();

            _vm = vm;
            BindingContext = _vm;

            // Kamera inicializlasa csak tmogatott platformokon
#if ANDROID || IOS
            _cameraView = new CameraView
            {
                HeightRequest = 220,
                HorizontalOptions = LayoutOptions.Fill,
            };

            _cameraView.MediaCaptured += OnMediaCaptured;
            CameraContainer.Content = _cameraView;
#else
            CameraContainer.Content = new Label
            {
                Text = "A kamera csak Android és iOS eszközökön érhető el.",
                Padding = new Thickness(4, 8),
                TextColor = Colors.Gray,
                FontSize = 12,
            };
#endif
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
                if (_cameraView is null)
                {
                    await DisplayAlert("Nem elérhető", "A kamera csak Android és iOS eszközökön érhető el.", "OK");
                    return;
                }

                if (!await EnsureCameraPermissionAsync())
                {
                    await DisplayAlert("Engedély szükséges", "A kamera használatához engedély szükséges.", "OK");
                    return;
                }

                if (!_cameraView.IsAvailable)
                {
                    await DisplayAlert("Nem elérhető", "A kamera nem érhető el ezen az eszközön.", "OK");
                    return;
                }

                using var cts = new CancellationTokenSource();
                // Ez csak elindtja a fot ksztst,
                // a stream az OnMediaCaptured-ben jn meg
                await _cameraView.CaptureImage(cts.Token);
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

        private void OnAddFeedingClicked(object sender, EventArgs e)
        {
            _vm.StartNewFeeding();

            var popup = new FeedingPopup(_vm);
            this.ShowPopup(popup);   // lsd a kvetkez pontot a using-hoz
        }

        private void OnEditEventClicked(object sender, EventArgs e)
        {
            if (sender is not Button button || button.BindingContext is not FeedingEvent feedingEvent)
                return;

            _vm.BeginEdit(feedingEvent);

            var popup = new FeedingPopup(_vm);
            this.ShowPopup(popup);
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
