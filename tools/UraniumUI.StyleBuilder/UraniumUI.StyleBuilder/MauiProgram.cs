using InputKit.Shared.Controls;
using Mopups.Hosting;
using UraniumUI;

namespace UraniumUI.StyleBuilder;
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureMopups()
            .UseUraniumUI()
            .UseUraniumUIMaterial()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");

                fonts.AddMaterialIconFonts();
            });

        builder.Services.AddMopupsDialogs();
        return builder.Build();
    }
}
