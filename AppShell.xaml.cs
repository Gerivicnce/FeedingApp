using FeedingApp.Views;

namespace FeedingApp
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(EditAnimalPage), typeof(EditAnimalPage));
        }
    }
}
