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
			.UseUraniumUI()
			.UseUraniumUIMaterial()
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

		return builder.Build();
	}
}
