using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UraniumUI;

public static class FluentIconsConfigurationExtensions
{
    public static IFontCollection AddFluentIconFonts(this IFontCollection fonts)
    {
        var thisAssembly = typeof(FluentIconsConfigurationExtensions).Assembly;

        fonts.AddEmbeddedResourceFont(thisAssembly, "Segoe Fluent Icons.ttf", "Fluent");

        return fonts;
    }
}
