namespace UraniumUI;
public static class FontAwesomeConfigurationExtensions
{
    public static IFontCollection AddFontAwesomeIconFonts(this IFontCollection fonts)
    {
        fonts.AddFont("Font Awesome 6 Free-Regular-400.otf", "FARegular");
        fonts.AddFont("Font Awesome 6 Free-Solid-900.otf", "FASolid");

        return fonts;
    }
}
