#if WINDOWS
using Microsoft.Maui.Controls.Compatibility.Platform.UWP;
using Microsoft.Maui.Handlers;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UraniumUI.Material.Controls;
using Grid = Microsoft.UI.Xaml.Controls.Grid;
using Microsoft.UI.Composition;
using Microsoft.UI.Xaml.Shapes;
using Microsoft.Maui.Platform;
using CommunityToolkit.Maui.Core.Extensions;
using Microsoft.Maui.Controls.Platform;
using Windows.Services.Maps;
using Microsoft.UI.Xaml.Media;

namespace UraniumUI.Material.Handlers;
public partial class CardViewHandler : ViewHandler<CardView, Grid>
{
    private static readonly Color DarkBlurOverlayColor = Color.FromArgb("#80000000");
    private static readonly Color DarkFallBackColor = Color.FromArgb("#333333");

    private static readonly Color LightBlurOverlayColor = Color.FromArgb("#40FFFFFF");
    private static readonly Color LightFallBackColor = Color.FromArgb("#F3F3F3");

    private static readonly Color ExtraLightBlurOverlayColor = Color.FromArgb("#B0FFFFFF");
    private static readonly Color ExtraLightFallBackColor = Color.FromArgb("#FBFBFB");

    private Rectangle _acrylicRectangle;
    private Rectangle _shadowHost;
    private Grid _grid;

    private Compositor _compositor;
    private SpriteVisual _shadowVisual;
    
    protected override Grid CreatePlatformView()
    {
        return new Grid();
    }

    protected override void ConnectHandler(Grid platformView)
    {
        base.ConnectHandler(platformView);

        PackChild();
        UpdateBorder();
        UpdateCornerRadius();
        UpdateMaterialTheme();
    }

    private void PackChild()
    {
        if (VirtualView.Content == null)
        {
            return;
        }

        IVisualElementRenderer renderer = VirtualView.Content.GetOrCreateRenderer();
        FrameworkElement frameworkElement = renderer.ContainerElement;

        _acrylicRectangle = new Rectangle();
        _shadowHost = new Rectangle { Fill = Colors.Transparent.ToPlatform() };

        _grid = new Grid();
        _grid.Children.Add(_acrylicRectangle);
        _grid.Children.Add(frameworkElement);

        PlatformView.Children.Add(_shadowHost);
        PlatformView.Children.Add(_grid);
    }
    private void UpdateBorder()
    {
        if (_grid == null)
        {
            return;
        }

        if (VirtualView.BorderColor != Colors.Transparent)
        {
            _grid.BorderBrush = VirtualView.BorderColor.ToPlatform();
            _grid.BorderThickness = new Microsoft.UI.Xaml.Thickness(1);
        }
        else
        {
            _grid.BorderBrush = new Color(0, 0, 0, 0).ToPlatform();
        }
    }

    private void UpdateCornerRadius()
    {
        if (_grid == null)
        {
            return;
        }

        float cornerRadius = VirtualView.CornerRadius;

        if (cornerRadius == -1f)
        {
            cornerRadius = 5f; // default corner radius
        }

        _grid.CornerRadius = new Microsoft.UI.Xaml.CornerRadius(cornerRadius);

        _shadowHost.RadiusX = cornerRadius;
        _shadowHost.RadiusY = cornerRadius;

        _acrylicRectangle.RadiusX = cornerRadius;
        _acrylicRectangle.RadiusY = cornerRadius;
    }

    private void UpdateMaterialTheme()
    {
        if (_grid == null)
        {
            return;
        }

        switch (VirtualView.MaterialTheme)
        {
            case CardView.Theme.Acrylic:
                SetAcrylicTheme();
                break;

            case CardView.Theme.AcrylicBlur:
                SetAcrylicBlurTheme();
                break;

            case CardView.Theme.Dark:
                SetDarkTheme();
                break;

            case CardView.Theme.Light:
                SetLightTheme();
                break;
        }

        UpdateBorder();
        UpdateCornerRadius();
        UpdateElevation();
    }

    private void SetDarkTheme()
    {
        ToggleAcrylicRectangle(false);
    }

    private void SetLightTheme()
    {
        ToggleAcrylicRectangle(false);

        UpdateLightThemeBackgroundColor();
    }

    private void SetAcrylicTheme()
    {
        ToggleAcrylicRectangle(true);

        UpdateLightThemeBackgroundColor();
        UpdateAcrylicGlowColor();
    }

    private void SetAcrylicBlurTheme()
    {
        ToggleAcrylicRectangle(false);

        UpdateBlur();
    }
    
    private void ToggleAcrylicRectangle(bool enable)
    {
        _acrylicRectangle.Margin = new Microsoft.UI.Xaml.Thickness(0, enable ? 2 : 0, 0, 0);
        if (!enable)
        {
            _acrylicRectangle.Fill = Colors.Transparent.ToPlatform();
        }
    }

    private void UpdateAcrylicGlowColor()
    {
        if (_grid == null || VirtualView.MaterialTheme != CardView.Theme.Acrylic)
        {
            return;
        }

        _grid.Background = VirtualView.AcrylicGlowColor.ToPlatform();
    }

    private void UpdateLightThemeBackgroundColor()
    {
        if (_grid == null)
        {
            return;
        }

        switch (VirtualView.MaterialTheme)
        {
            case CardView.Theme.Acrylic:
                _acrylicRectangle.Fill = VirtualView.LightThemeBackgroundColor.ToPlatform();
                break;

            case CardView.Theme.AcrylicBlur:
            case CardView.Theme.Dark:
                return;

            case CardView.Theme.Light:
                _grid.Background = VirtualView.LightThemeBackgroundColor.ToPlatform();
                break;
        }
    }
    
    private void UpdateBlur()
    {
        if (_grid == null)
        {
            return;
        }

        if (VirtualView.UwpBlurOverlayColor != Colors.Transparent)
        {
            var acrylicBrush = new AcrylicBrush
            {
                BackgroundSource = Element.UwpHostBackdropBlur ? AcrylicBackgroundSource.HostBackdrop : AcrylicBackgroundSource.Backdrop,
                TintColor = Element.UwpBlurOverlayColor.ToWindowsColor(),
            };

            _grid.Background = acrylicBrush;
            return;
        }

        UpdateMaterialBlurStyle();
    }

    public static void MapCornerRadius(CardViewHandler handler, CardView view)
    {
        handler.UpdateCornerRadius();
    }

    public static void MapBorderColor(CardViewHandler handler, CardView view)
    {
        handler.UpdateBorder();
    }
    
    public static void MapElevation(CardViewHandler handler, CardView view)
    {
    }
    
    public static void MapLightThemeBackgroundColor(CardViewHandler handler, CardView view)
    {
        handler.UpdateLightThemeBackgroundColor();
    }
    
    public static void MapAcrylicGlowColor(CardViewHandler handler, CardView view)
    {
        handler.UpdateAcrylicGlowColor();
    }
    public static void MapMaterialTheme(CardViewHandler handler, CardView view)
    {
        handler.UpdateMaterialTheme();
    }
    public static void MapUwpBlurOverlayColor(CardViewHandler handler, CardView view)
    {
        handler.UpdateBlur();
    }
    public static void MapMaterialBlurStyle(CardViewHandler handler, CardView view)
    {
    }
}
#endif