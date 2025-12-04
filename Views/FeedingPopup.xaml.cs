using CommunityToolkit.Maui.Views;
using FeedingApp.ViewModels;

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
}
