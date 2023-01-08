using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UraniumUI.Material.Controls;
public partial class CardView
{
    public static readonly BindableProperty UwpBlurOverlayColorProperty = BindableProperty.Create(
            nameof(UwpBlurOverlayColor),
            typeof(Color),
            typeof(CardView),
            defaultValueCreator: _ => Colors.White);

    public static readonly BindableProperty UwpHostBackdropBlurProperty = BindableProperty.Create(
        nameof(UwpHostBackdropBlur),
        typeof(bool),
        typeof(CardView),
        defaultValueCreator: _ => false);

    /// <summary>
    /// UWP only.
    /// Changes the overlay color over the blur (should be a transparent color, obviously).
    /// If not set, the different blur style styles take over.
    /// </summary>
    public Color UwpBlurOverlayColor
    {
        get => (Color)GetValue(UwpBlurOverlayColorProperty);
        set => SetValue(UwpBlurOverlayColorProperty, value);
    }

    /// <summary>
    /// UWP only.
    /// HostBackdropBlur reveals the desktop wallpaper and other windows that are behind the currently active app.
    /// If not set, the default in app BackdropBlur take over.
    /// </summary>
    public bool UwpHostBackdropBlur
    {
        get => (bool)GetValue(UwpHostBackdropBlurProperty);
        set => SetValue(UwpHostBackdropBlurProperty, value);
    }
}
