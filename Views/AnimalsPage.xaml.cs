using FeedingApp.ViewModels;

namespace FeedingApp.Views;

public partial class AnimalsPage : ContentPage
{
    private readonly AnimalsViewModel _vm;

    public AnimalsPage(AnimalsViewModel vm)
    {
        InitializeComponent();

        _vm = vm;
        BindingContext = _vm;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (_vm.LoadCommand.CanExecute(null))
            _vm.LoadCommand.Execute(null);
    }
}
