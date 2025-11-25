using FeedingApp.Services;
using FeedingApp.ViewModels;

namespace FeedingApp.Views;

public partial class GalleryPage : ContentPage
{
    private readonly GalleryViewModel _vm;

    public GalleryPage()
    {
        InitializeComponent();

        var db = new DatabaseService();
        _vm = new GalleryViewModel(db);
        BindingContext = _vm;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (_vm.LoadPhotosCommand.CanExecute(null))
            _vm.LoadPhotosCommand.Execute(null);
    }
}
