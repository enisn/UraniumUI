using InputKit.Handlers;
using Microsoft.Maui.Hosting;
using UraniumUI;
using UraniumUI.Handlers;

namespace UraniumApp;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
				fonts.AddFontAwesomeIconFonts();
			})
			.ConfigureMauiHandlers(handlers =>
			{
				handlers.AddUraniumUIHandlers();
            });

		return builder.Build();
	}
}
