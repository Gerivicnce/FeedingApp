using FeedingApp.ViewModels;

namespace FeedingApp.Views;

public partial class GalleryPage : ContentPage
{
    private readonly GalleryViewModel _vm;

    public GalleryPage(GalleryViewModel vm)
    {
        InitializeComponent();

        _vm = vm;
        BindingContext = _vm;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (_vm.LoadPhotosCommand.CanExecute(null))
            _vm.LoadPhotosCommand.Execute(null);
    }
}
