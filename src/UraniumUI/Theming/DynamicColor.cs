using System.ComponentModel;
using UraniumUI.Resources;

namespace UraniumUI.Theming;

public class DynamicResourceColor : UraniumBindableObject
{
    public DynamicResourceColor()
    {
        Application.Current.RequestedThemeChanged += (s, e) =>
        {
            InvokePropertyChanged();
        };
    }

    private void InvokePropertyChanged()
    {
        OnPropertyChanged(nameof(Primary20));
        OnPropertyChanged(nameof(Primary40));
    }

    public Color Primary20 { get => ColorResource.GetColor("Primary").WithAlpha(0.2f); }
    public Color Primary40 { get => ColorResource.GetColor("Primary").WithAlpha(0.4f); }
}

public static class ExtraParameters
{

    public static readonly BindableProperty TextColorOpacityProperty =
        BindableProperty.CreateAttached("TextColorOpacity", typeof(float), typeof(View), 1f,
            propertyChanged: (bo, ov, nv) => UpdateOpacityOfColor(bo as View, "TextColor", Convert.ToSingle(nv)));

    public static readonly BindableProperty BackgroundOpacityProperty =
        BindableProperty.CreateAttached("BackgroundOpacity", typeof(float), typeof(View), 1f,
            propertyChanged: (bo, ov, nv) => UpdateOpacityOfBrush(bo as View, "Background", Convert.ToSingle(nv)));

    private static void UpdateOpacityOfBrush(View view, string propertyName, float opacity)
    {
        var prop = view.GetType().GetProperty(propertyName)
            ?? throw new InvalidOperationException($"The type {view.GetType().FullName} doesn't have any property named 'Background'.");
        var brush = prop.GetValue(view) as SolidColorBrush;

        if (brush?.Color is null)
        {
            view.PropertyChanged -= View_PropertyChanged;
            view.PropertyChanged += View_PropertyChanged;
            return;
        }

        brush.Color = brush.Color.WithAlpha(opacity);

        void View_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == propertyName)
            {
                brush = prop.GetValue(view) as SolidColorBrush;
                if (brush?.Color is not null)
                {
                    brush.Color = brush.Color.WithAlpha(opacity);
                }

                view.PropertyChanged -= View_PropertyChanged;
            }
        }
    }

    private static void UpdateOpacityOfColor(View view, string propertyName, float opacity)
    {
        var prop = view.GetType().GetProperty(propertyName);
        var color = prop.GetValue(view) as Color;

        //view.PropertyChanged -= View_PropertyChanged;
        //view.PropertyChanged += View_PropertyChanged;

        if (color == Colors.Transparent || color.Alpha == opacity)
        {
            return;
        }

        prop.SetValue(view, color.WithAlpha(opacity));

        void View_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            view.PropertyChanged -= View_PropertyChanged;

            var color = prop.GetValue(view) as Color;

            prop.SetValue(view, color.WithAlpha(opacity));
        }
    }

    public static float GetTextColorOpacity(BindableObject view)
    {
        return (float)view.GetValue(TextColorOpacityProperty);
    }

    public static void SetTextColorOpacity(BindableObject view, bool value)
    {
        view.SetValue(TextColorOpacityProperty, value);
    }

    public static float GetBackgroundOpacity(BindableObject view)
    {
        return (float)view.GetValue(BackgroundOpacityProperty);
    }

    public static void SetBackgroundOpacity(BindableObject view, bool value)
    {
        view.SetValue(BackgroundOpacityProperty, value);
    }
}