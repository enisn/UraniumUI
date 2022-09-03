namespace UraniumUI;

public static class MaterialIconsConfigurationExtensions
{
    public static IFontCollection AddMaterialIconFonts(this IFontCollection fonts)
    {
        var thisAssembly = typeof(MaterialIconsConfigurationExtensions).Assembly;

        fonts.AddEmbeddedResourceFont(thisAssembly, "MaterialIconsOutlined-Regular.otf", "MaterialOutlined");
        fonts.AddEmbeddedResourceFont(thisAssembly, "MaterialIcons-Regular.ttf", "MaterialRegular");
        fonts.AddEmbeddedResourceFont(thisAssembly, "MaterialIconsRound-Regular.otf", "MaterialRoundRegular");
        fonts.AddEmbeddedResourceFont(thisAssembly, "MaterialIconsSharp-Regular.otf", "MaterialSharpRegular");
        fonts.AddEmbeddedResourceFont(thisAssembly, "MaterialIconsTwoTone-Regular.otf", "MaterialTwoTone");

        return fonts;
    }
}
