using System;
using System.IO;
using CommunityToolkit.Maui.Views;
using FeedingApp.ViewModels;
using Microsoft.Maui.Storage;

namespace FeedingApp.Views;

public partial class FeedingPopup : Popup
{
    public FeedingPopup(CalendarViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        if (BindingContext is CalendarViewModel vm && vm.SaveFeedingCommand.CanExecute(null))
        {
            vm.SaveFeedingCommand.Execute(null);
        }

        await CloseAsync();
    }

    private async void OnPickPhotoClicked(object sender, EventArgs e)
    {
        if (BindingContext is not CalendarViewModel vm)
            return;

        try
        {
            if (!MediaPicker.Default.IsPickPhotoSupported && !MediaPicker.Default.IsCaptureSupported)
            {
                await Shell.Current.DisplayAlert("Nem elérhető", "Ez a platform nem támogatja a fotó kiválasztást.", "OK");
                return;
            }

            FileResult? result = null;

            if (MediaPicker.Default.IsPickPhotoSupported)
            {
                result = await MediaPicker.Default.PickPhotoAsync(new MediaPickerOptions
                {
                    Title = "Etetési fotó kiválasztása"
                });
            }

            if (result == null && MediaPicker.Default.IsCaptureSupported)
            {
                result = await MediaPicker.Default.CapturePhotoAsync(new MediaPickerOptions
                {
                    Title = "Etetési fotó"
                });
            }

            if (result == null)
                return;

            var newFile = Path.Combine(FileSystem.AppDataDirectory, $"feeding_{DateTime.Now:yyyyMMdd_HHmmss}.jpg");
            await using var stream = await result.OpenReadAsync();
            await using var newStream = File.OpenWrite(newFile);
            await stream.CopyToAsync(newStream);

            vm.CurrentPhotoPath = newFile;
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Hiba", $"A fotó mentése nem sikerült: {ex.Message}", "OK");
        }
    }
}
