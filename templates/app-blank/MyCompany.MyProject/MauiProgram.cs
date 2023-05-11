#if CommunityToolkit
using CommunityToolkit.Maui;
#endif
#if Mopups
using Mopups.Hosting;
#endif
using InputKit.Shared.Controls;
using UraniumUI;

namespace MyCompany.MyProject;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
#if CommunityToolkit
			.UseMauiCommunityToolkit()
#endif
#if Mopups
			.ConfigureMopups()
#endif
			.UseUraniumUI()
			.UseUraniumUIMaterial()
#if Blurs
			.UseUraniumUIBlurs()
#endif
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
#if FontAwesome
				fonts.AddFontAwesomeIconFonts();
#endif

#if MaterialIcons
				fonts.AddMaterialIconFonts();
#endif
            });

#if CommunityToolkit
        builder.Services.AddCommunityToolkitDialogs();
#endif
#if Mopups
		builder.Services.AddMopupsDialogs();
#endif
		return builder.Build();
	}
}
