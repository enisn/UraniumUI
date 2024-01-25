namespace UraniumUI;

public static class MaterialIconsConfigurationExtensions
{
    public static IFontCollection AddMaterialSymbolsFonts(this IFontCollection fonts)
    {
        var thisAssembly = typeof(MaterialIconsConfigurationExtensions).Assembly;

        fonts.AddEmbeddedResourceFont(thisAssembly, "MaterialSymbolsOutlined.ttf", "MaterialOutlined");
        fonts.AddEmbeddedResourceFont(thisAssembly, "MaterialSymbolsRounded.ttf", "MaterialRounded");
        fonts.AddEmbeddedResourceFont(thisAssembly, "MaterialSymbolsSharp.ttf", "MaterialSharp");

        return fonts;
    }
}