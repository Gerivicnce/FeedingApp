using CommunityToolkit.Maui.Views;
using FeedingApp.Services;
using FeedingApp.ViewModels;
using Microsoft.Maui.Storage;
using System;
using System.IO;

namespace FeedingApp.Views;

public partial class CalendarPage : ContentPage
{
    private readonly CalendarViewModel _vm;

    public CalendarPage()
    {
        InitializeComponent();

        var db = new DatabaseService();
        _vm = new CalendarViewModel(db);
        BindingContext = _vm;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (_vm.LoadCommand.CanExecute(null))
            _vm.LoadCommand.Execute(null);
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
}
