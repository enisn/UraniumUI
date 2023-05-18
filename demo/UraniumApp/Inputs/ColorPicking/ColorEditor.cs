using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UraniumApp.Inputs.ColorPicking;

public class ColorEditor : Plainer.Maui.Controls.EntryView
{
    protected override void OnHandlerChanging(HandlerChangingEventArgs args)
    {
        base.OnHandlerChanging(args);

        if (args.NewHandler == null)
        {
            this.TextChanged -= ColorEditor_TextChanged;
        }
        else
        {
            this.TextChanged += ColorEditor_TextChanged;
        }
    }

    private void ColorEditor_TextChanged(object sender, TextChangedEventArgs e)
    {
        UpdateColorFromText();
    }

    public Color Color { get => (Color)GetValue(ColorProperty); set => SetValue(ColorProperty, value); }

    public static readonly BindableProperty ColorProperty = BindableProperty.Create(
        nameof(Color),
        typeof(Color),
        typeof(ColorEditor),
        defaultValue: Colors.Transparent,
        defaultBindingMode: BindingMode.TwoWay,
        propertyChanged: (bindable, oldValue, newValue) =>
        {
            if (bindable is ColorEditor colorEditor && newValue is Color newColor && oldValue != newValue)
            {
                colorEditor.UpdateTextFromColor();
            }
        });

    protected void UpdateTextFromColor()
    {
        if (IsFocused)
        {
            return;
        }

        var newHex = Color.ToHex();
        if (Text != newHex)
        {
            Text = newHex;
        }
    }

    protected void UpdateColorFromText()
    {
        if (Color.TryParse(Text, out var color) && color != Color)
        {
            Color = color;
        }
    }
}
