using System;
using System.IO;
using CommunityToolkit.Maui.Views;
using FeedingApp.ViewModels;
using Microsoft.Maui.ApplicationModel;
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
            FileResult? result = null;

            try
            {
                result = await MediaPicker.Default.PickPhotoAsync(new MediaPickerOptions
                {
                    Title = "Etetési fotó kiválasztása"
                });
            }
            catch (FeatureNotSupportedException)
            {
                // Picking photos is not supported on this device.
            }
            catch (PermissionException ex)
            {
                await Shell.Current.DisplayAlert("Hozzáférés megtagadva", $"A művelet engedélyt igényel: {ex.Message}", "OK");
                return;
            }

            if (result == null && MediaPicker.Default.IsCaptureSupported)
            {
                try
                {
                    result = await MediaPicker.Default.CapturePhotoAsync(new MediaPickerOptions
                    {
                        Title = "Etetési fotó"
                    });
                }
                catch (PermissionException ex)
                {
                    await Shell.Current.DisplayAlert("Hozzáférés megtagadva", $"A művelet engedélyt igényel: {ex.Message}", "OK");
                    return;
                }
                catch (FeatureNotSupportedException)
                {
                    // Capturing photos is not supported on this device.
                }
            }

            if (result == null)
            {
                await Shell.Current.DisplayAlert("Nem elérhető", "Ez a platform nem támogatja a fotó kiválasztást.", "OK");
                return;
            }

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
