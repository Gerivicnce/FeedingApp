using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Maui.Core.Primitives;
using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Storage;

namespace FeedingApp.Views;

public partial class CameraCapturePage : ContentPage
{
    private readonly Action<string> _onPhotoSaved;
    private CameraInfo? _selectedCamera;

    public CameraCapturePage(Action<string> onPhotoSaved)
    {
        InitializeComponent();
        _onPhotoSaved = onPhotoSaved;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (_selectedCamera is null && Camera.Cameras.Count > 0)
        {
            await StartCameraAsync(Camera.Cameras.First());
        }
    }

    protected override async void OnDisappearing()
    {
        try
        {
            await Camera.StopCameraAsync();
        }
        catch
        {
            // Ha a kamera még nem indult el, nincs mit leállítani.
        }
        base.OnDisappearing();
    }

    private async void OnCamerasLoaded(object? sender, EventArgs e)
    {
        if (Camera.Cameras.Count == 0)
        {
            StatusLabel.Text = "Nem található kamera az eszközön.";
            return;
        }

        _selectedCamera ??= Camera.Cameras.First();
        await StartCameraAsync(_selectedCamera);
    }

    private async Task StartCameraAsync(CameraInfo camera)
    {
        try
        {
            await Camera.StartCameraAsync(camera);
            _selectedCamera = camera;
            StatusLabel.Text = "Készen áll a fotózásra.";
        }
        catch (Exception ex)
        {
            StatusLabel.Text = $"Nem sikerült elindítani a kamerát: {ex.Message}";
        }
    }

    private async void OnSwitchCameraClicked(object? sender, EventArgs e)
    {
        if (Camera.Cameras.Count <= 1)
        {
            await Shell.Current.DisplayAlert("Nincs több kamera", "Az eszközön nem váltható kamera.", "OK");
            return;
        }

        var cameras = Camera.Cameras.ToList();
        var currentIndex = _selectedCamera is null ? -1 : cameras.IndexOf(_selectedCamera);
        var nextIndex = currentIndex >= 0 ? (currentIndex + 1) % cameras.Count : 0;

        await StartCameraAsync(cameras[nextIndex]);
    }

    private async void OnCaptureClicked(object? sender, EventArgs e)
    {
        try
        {
            StatusLabel.Text = "Fotó készítése...";
            await Camera.CapturePhotoAsync();
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Hiba", $"A fotózás nem sikerült: {ex.Message}", "OK");
            StatusLabel.Text = "Hiba történt a fotózás során.";
        }
    }

    private async void OnMediaCaptured(object? sender, MediaCapturedEventArgs e)
    {
        var filePath = Path.Combine(FileSystem.AppDataDirectory, $"feeding_{DateTime.Now:yyyyMMdd_HHmmss}.jpg");

        try
        {
            e.Media.Position = 0;
            await using var source = e.Media;
            await using var destination = File.OpenWrite(filePath);
            await source.CopyToAsync(destination);

            _onPhotoSaved?.Invoke(filePath);
            await Shell.Current.DisplayAlert("Mentve", "A fotó elmentve a bejegyzéshez.", "OK");
            await Shell.Current.Navigation.PopModalAsync();
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Hiba", $"A fotó mentése nem sikerült: {ex.Message}", "OK");
        }
    }

    private async void OnCancelClicked(object? sender, EventArgs e)
    {
        await Shell.Current.Navigation.PopModalAsync();
    }
}
