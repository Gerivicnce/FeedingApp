using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using FeedingApp.Services;
using FeedingApp.ViewModels;
using FeedingApp.Views;

namespace FeedingApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .UseMauiCommunityToolkitCamera()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            builder.Services.AddSingleton<DatabaseService>();

            builder.Services.AddSingleton<AnimalsViewModel>();
            builder.Services.AddSingleton<AnimalsPage>();

            builder.Services.AddSingleton<CalendarViewModel>();
            builder.Services.AddSingleton<CalendarPage>();

            builder.Services.AddSingleton<GalleryViewModel>();
            builder.Services.AddSingleton<GalleryPage>();

            builder.Services.AddTransient<EditAnimalPage>();
            builder.Services.AddSingleton<AppShell>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
