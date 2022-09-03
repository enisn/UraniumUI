using InputKit.Handlers;

namespace UraniumUI;
public static class MauiProgramExtensions
{
    public static IMauiHandlersCollection AddUraniumUIHandlers(this IMauiHandlersCollection collection)
    {
        return collection.AddInputKitHandlers();
    }

    public static IFontCollection AddFontAwesomeIconFonts(this IFontCollection fonts)
    {
        fonts.AddFont("Font Awesome 6 Free-Regular-400.otf", "FARegular");
        fonts.AddFont("Font Awesome 6 Free-Solid-900.otf", "FASolid");

        return fonts;
    }
}
