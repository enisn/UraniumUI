namespace UraniumUI;
public static class MauiProgramExtensions
{
    public static IFontCollection AddFontAwesomeIconFonts(this IFontCollection fonts)
    {
        fonts.AddFont("MaterialIconsOutlined-Regular.otf", "MaterialOutlined");
        fonts.AddFont("MaterialIcons-Regular.ttf", "MaterialRegular");
        fonts.AddFont("MaterialIconsRound-Regular.otf", "MaterialRoundRegular");
        fonts.AddFont("MaterialIconsSharp-Regular.otf", "MaterialSharpRegular");
        fonts.AddFont("MaterialIconsTwoTone-Regular.otf", "MaterialTwoTone");
       
        return fonts;
    }
}
