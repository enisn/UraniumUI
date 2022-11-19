using CommunityToolkit.Maui;
using InputKit.Handlers;
using Microsoft.Maui.Hosting;
using UraniumUI;
using UraniumUI.Handlers;
using UraniumUI.Material;

namespace UraniumApp;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.UseMauiCommunityToolkit()
			.UseUraniumUI()
			.UseUraniumUIMaterial()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
				fonts.AddMaterialIconFonts();
			});

		return builder.Build();
	}
}
