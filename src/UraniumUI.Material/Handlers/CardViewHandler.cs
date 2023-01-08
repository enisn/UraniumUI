using Microsoft.Maui.Handlers;
using UraniumUI.Material.Controls;

namespace UraniumUI.Material.Handlers;
public partial class CardViewHandler
{
    public static IPropertyMapper<CardView, CardViewHandler> CardViewMapper
       => new PropertyMapper<CardView, CardViewHandler>(ViewHandler.ViewMapper)
       {
           [nameof(CardView.BorderColor)] = MapBorderColor,
           [nameof(CardView.CornerRadius)] = MapCornerRadius,
           [nameof(CardView.Elevation)] = MapElevation,
           [nameof(CardView.LightThemeBackgroundColor)] = MapLightThemeBackgroundColor,
           [nameof(CardView.AcrylicGlowColor)] = MapAcrylicGlowColor,
           [nameof(CardView.MaterialTheme)] = MapMaterialTheme,
           [nameof(CardView.UwpBlurOverlayColor)] = MapUwpBlurOverlayColor,
           [nameof(CardView.MaterialBlurStyle)] = MapMaterialBlurStyle,
       };
    public CardViewHandler() : base(CardViewMapper)
    {
    }
}

#if (NET7_0 || NET6_0) && !ANDROID && !IOS && !MACCATALYST && !WINDOWS
public partial class CardViewHandler : ViewHandler<CardView, object>
{
    public CardViewHandler(IPropertyMapper mapper, CommandMapper commandMapper = null) : base(CardViewMapper, commandMapper)
    {
    }

    protected override object CreatePlatformView()
    {
        throw new NotImplementedException();
    }

    public static void MapCornerRadius(CardViewHandler handler, CardView view)
    {
    }

    public static void MapBorderColor(CardViewHandler handler, CardView view)
    {
    }
    public static void MapElevation(CardViewHandler handler, CardView view)
    {
    }
    public static void MapLightThemeBackgroundColor(CardViewHandler handler, CardView view)
    {
    }
    public static void MapAcrylicGlowColor(CardViewHandler handler, CardView view)
    {
    }
    public static void MapMaterialTheme(CardViewHandler handler, CardView view)
    {
    }
    public static void MapUwpBlurOverlayColor(CardViewHandler handler, CardView view)
    {
    }
    public static void MapMaterialBlurStyle(CardViewHandler handler, CardView view)
    {
    }
}
#endif