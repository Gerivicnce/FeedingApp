using System;
using System.IO;
using System.Threading.Tasks;
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
            var action = await Shell.Current.DisplayActionSheet(
                "Fotó hozzáadása",
                "Mégse",
                null,
                "Új fotó készítése",
                "Meglévő fotó kiválasztása");

            if (string.IsNullOrEmpty(action) || action == "Mégse")
                return;

            if (action == "Új fotó készítése")
            {
                if (!MediaPicker.Default.IsCaptureSupported)
                {
                    await Shell.Current.DisplayAlert("Nem elérhető", "Ez a platform nem támogatja a fotó készítést.", "OK");
                    return;
                }

                await CaptureAndSavePhotoAsync(vm);
            }
            else if (action == "Meglévő fotó kiválasztása")
            {
                await PickAndSavePhotoAsync(vm);
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Hiba", $"A fotó mentése nem sikerült: {ex.Message}", "OK");
        }
    }

    private static async Task CaptureAndSavePhotoAsync(CalendarViewModel vm)
    {
        try
        {
            var result = await MediaPicker.Default.CapturePhotoAsync(new MediaPickerOptions
            {
                Title = "Etetési fotó"
            });

            await SavePhotoResultAsync(result, vm);
        }
        catch (PermissionException ex)
        {
            await Shell.Current.DisplayAlert("Hozzáférés megtagadva", $"A művelet engedélyt igényel: {ex.Message}", "OK");
        }
        catch (FeatureNotSupportedException)
        {
            await Shell.Current.DisplayAlert("Nem elérhető", "Ez a platform nem támogatja a fotó készítést.", "OK");
        }
    }

    private static async Task PickAndSavePhotoAsync(CalendarViewModel vm)
    {
        try
        {
            var result = await MediaPicker.Default.PickPhotoAsync(new MediaPickerOptions
            {
                Title = "Etetési fotó kiválasztása"
            });

            await SavePhotoResultAsync(result, vm);
        }
        catch (PermissionException ex)
        {
            await Shell.Current.DisplayAlert("Hozzáférés megtagadva", $"A művelet engedélyt igényel: {ex.Message}", "OK");
        }
        catch (FeatureNotSupportedException)
        {
            await Shell.Current.DisplayAlert("Nem elérhető", "Ez a platform nem támogatja a fotó kiválasztást.", "OK");
        }
    }

    private static async Task SavePhotoResultAsync(FileResult? result, CalendarViewModel vm)
    {
        if (result == null)
            return;

        var newFile = Path.Combine(FileSystem.AppDataDirectory, $"feeding_{DateTime.Now:yyyyMMdd_HHmmss}.jpg");
        await using var stream = await result.OpenReadAsync();
        await using var newStream = File.OpenWrite(newFile);
        await stream.CopyToAsync(newStream);

        vm.CurrentPhotoPath = newFile;
    }
}
