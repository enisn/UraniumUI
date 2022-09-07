using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UraniumUI.Theming;

[ContentProperty("Color")]
public class DynamicColor : IMarkupExtension
{
    public Color Color { get; set; }

    public float Opacity { get; set; }

    public object ProvideValue(IServiceProvider serviceProvider)
    {
        return Color.MultiplyAlpha(Opacity);
    }
}
