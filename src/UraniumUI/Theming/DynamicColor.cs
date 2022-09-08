using CommunityToolkit.Maui.Behaviors;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Platform;

namespace UraniumUI.Theming;

//[ContentProperty("Color")]
//public class DynamicColor : IMarkupExtension
//{
//    public object Value { get; set; }

//    public float Opacity { get; set; }

//    public object ProvideValue(IServiceProvider serviceProvider)
//    {
//        if (Value is Color color)
//        {
//            return color.MultiplyAlpha(Opacity);
//        }

//        return Value;
//    }
//}

public static class ExtraParameters
{
    public static readonly BindableProperty TextColorOpacityProperty =
        BindableProperty.CreateAttached("TextColorOpacity", typeof(float), typeof(View), 1f, 
            propertyChanged: (bo, ov, nv)=> UpdateOpacityOfProperty(bo as View, "TextColor", Convert.ToSingle(nv)));


    public static readonly BindableProperty BackgroundColorOpacityProperty =
        BindableProperty.CreateAttached("BackgroundColorOpacity", typeof(float), typeof(View), 1f,
            propertyChanged: (bo, ov, nv) => UpdateOpacityOfProperty(bo as View, "BackgroundColor", Convert.ToSingle(nv)));

    private static void UpdateOpacityOfProperty(View view, string propertyName, float opacity)
    {
        var prop = view.GetType().GetProperty(propertyName);
        var color = prop.GetValue(view) as Color;

        prop.SetValue(view, color.MultiplyAlpha(opacity));
    }
    public static float GetTextColorOpacity(BindableObject view)
    {
        return (float)view.GetValue(TextColorOpacityProperty);
    }

    public static void SetTextColorOpacity(BindableObject view, bool value)
    {
        view.SetValue(TextColorOpacityProperty, value);
    }

    public static float GetBackgroundColorOpacity(BindableObject view)
    {
        return (float)view.GetValue(BackgroundColorOpacityProperty);
    }

    public static void SetBackgroundColorOpacity(BindableObject view, bool value)
    {
        view.SetValue(BackgroundColorOpacityProperty, value);
    }
}