using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Maui.Views;
using FeedingApp.Services;
using FeedingApp.ViewModels;
using Microsoft.Maui.Storage;
using System;
using System.IO;
using System.Threading;
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

        // kamera event feliratkozás
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
            await DisplayAlert("Hiba", $"A naptár betöltése közben hiba történt: {ex.Message}", "OK");
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
            using var cts = new CancellationTokenSource();
            // Ez csak elindítja a fotó készítést,
            // a stream az OnMediaCaptured-ben jön meg
            await CameraViewControl.CaptureImage(cts.Token);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Hiba", $"Nem sikerült fotót készíteni: {ex.Message}", "OK");
        }
    }

    // Itt kapod meg ténylegesen a fotó streamjét
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
            await DisplayAlert("Hiba", $"Nem sikerült fotót menteni: {ex.Message}", "OK");
        }
    }

    private void OnAddFeedingClicked(object sender, EventArgs e)
    {
        var popup = new FeedingPopup(_vm);
        this.ShowPopup(popup);   // lásd a következõ pontot a using-hoz
    }
}
