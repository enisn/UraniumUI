namespace UraniumUI;

public static class MaterialIconsConfigurationExtensions
{
    public static IFontCollection AddMaterialSymbolsFonts(this IFontCollection fonts)
    {
        var thisAssembly = typeof(MaterialIconsConfigurationExtensions).Assembly;

        fonts.AddEmbeddedResourceFont(thisAssembly, "MaterialSymbolsOutlined.ttf", "MaterialSymbolsOutlined");
        fonts.AddEmbeddedResourceFont(thisAssembly, "MaterialSymbolsRounded.ttf", "MaterialSymbolsRounded");
        fonts.AddEmbeddedResourceFont(thisAssembly, "MaterialSymbolsSharp.ttf", "MaterialSymbolsSharp");

        return fonts;
    }
}