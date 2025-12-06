using CommunityToolkit.Maui.Views;
using FeedingApp.Models;
using FeedingApp.ViewModels;
using Microsoft.Maui.Storage;
using System;
using System.IO;

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
                var photoStream = await CameraViewControl.TakePhotoAsync();

                if (photoStream == null)
                    return;

                var fileName = $"feeding_{DateTime.Now:yyyyMMdd_HHmmss}.jpg";
                var filePath = Path.Combine(FileSystem.AppDataDirectory, fileName);

                await using var stream = photoStream;
                await using var fileStream = File.OpenWrite(filePath);
                await stream.CopyToAsync(fileStream);

                _vm.CurrentPhotoPath = filePath;
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

    }
}
