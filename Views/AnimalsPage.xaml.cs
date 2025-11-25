using FeedingApp.Services;
using FeedingApp.ViewModels;

namespace FeedingApp.Views;

public partial class AnimalsPage : ContentPage
{
    private readonly AnimalsViewModel _vm;
    public AnimalsPage()
	{
        InitializeComponent();

        var db = new DatabaseService();
        _vm = new AnimalsViewModel(db);
        BindingContext = _vm;
    }
    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (_vm.LoadCommand.CanExecute(null))
            _vm.LoadCommand.Execute(null);
    }
}