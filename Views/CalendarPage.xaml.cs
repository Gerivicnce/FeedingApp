using CommunityToolkit.Maui.Views;
using FeedingApp.ViewModels;
using System;
using Microsoft.Maui.ApplicationModel;
using FeedingApp.Models;

namespace FeedingApp.Views
{
    public partial class CalendarPage : ContentPage
    {
        private readonly CalendarViewModel _vm;

        public CalendarPage(CalendarViewModel vm)
        {
            InitializeComponent();

            _vm = vm;
            BindingContext = _vm;
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

                if (!MediaPicker.Default.IsCaptureSupported)
                {
                    await DisplayAlert("Nem elérhető", "Ez a platform nem támogatja a fotó készítést.", "OK");
                    return;
                }

                var result = await MediaPicker.Default.CapturePhotoAsync(new MediaPickerOptions
                {
                    Title = "Etetési fotó"
                });

                if (result == null)
                    return;

                _vm.CurrentPhotoPath = result.FullPath;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Hiba", $"Nem sikerlt fott kszteni: {ex.Message}", "OK");
            }
        }

        private async void OnAddFeedingClicked(object sender, EventArgs e)
        {
            _vm.StartNewFeeding();

            var popup = new FeedingPopup(_vm);
            await this.ShowPopupAsync(popup);
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
