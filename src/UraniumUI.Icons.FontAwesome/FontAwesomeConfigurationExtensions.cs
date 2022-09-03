namespace UraniumUI;
public static class FontAwesomeConfigurationExtensions
{
    public static IFontCollection AddFontAwesomeIconFonts(this IFontCollection fonts)
    {
        fonts.AddEmbeddedResourceFont(
            typeof(FontAwesomeConfigurationExtensions).Assembly,
           filename: "FontAwesome6Free-Solid-900.otf",
           alias: "FASolid");

        fonts.AddEmbeddedResourceFont(
            typeof(FontAwesomeConfigurationExtensions).Assembly,
           filename: "FontAwesomeFree-Regular-400.otf",
           alias: "FARegular");

        return fonts;
    }
}
