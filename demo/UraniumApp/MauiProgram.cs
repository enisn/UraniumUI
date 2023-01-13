//using CommunityToolkit.Maui;
using Mopups.Hosting;
using UraniumApp.Pages;
using UraniumUI;

namespace UraniumApp;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            //.UseMauiCommunityToolkit()
            .UseUraniumUI()
            .UseUraniumUIMaterial()
            .ConfigureMopups()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddMaterialIconFonts();
            });

        builder.Services.AddTransient<DialogsPage>();
        //builder.Services.AddCommunityToolkitDialogs();
        builder.Services.AddMopupsDialogs();

        return builder.Build();
    }
}
