namespace UraniumUI;

public static class MaterialIconsConfigurationExtensions
{
    public static IFontCollection AddMaterialSymbolsFonts(this IFontCollection fonts)
    {
        var thisAssembly = typeof(MaterialIconsConfigurationExtensions).Assembly;

        fonts.AddEmbeddedResourceFont(thisAssembly, "MaterialSymbolsOutlined.ttf", FontAliases.MaterialOutlined);
        fonts.AddEmbeddedResourceFont(thisAssembly, "MaterialSymbolsOutlinedFilled.ttf", FontAliases.MaterialOutlinedFilled);
        fonts.AddEmbeddedResourceFont(thisAssembly, "MaterialSymbolsRounded.ttf", FontAliases.MaterialRounded);
        fonts.AddEmbeddedResourceFont(thisAssembly, "MaterialSymbolsRoundedFilled.ttf", FontAliases.MaterialRoundedFilled);
        fonts.AddEmbeddedResourceFont(thisAssembly, "MaterialSymbolsSharp.ttf", FontAliases.MaterialSharp);
        fonts.AddEmbeddedResourceFont(thisAssembly, "MaterialSymbolsSharpFilled.ttf", FontAliases.MaterialSharpFilled);

        return fonts;
    }
}