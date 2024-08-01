using Microsoft.Maui.Platform;
using UraniumUI.Resources;

namespace UraniumUI.Material.Extensions;
public static class EntryProperties
{
    public static readonly BindableProperty SelectionHighlightColorProperty = BindableProperty.CreateAttached(
        "SelectionHighlightColor",
        typeof(Color),
        typeof(Entry),
        ColorResource.GetColor("Primary", "PrimaryDark", Colors.Purple),
        propertyChanged: OnSelectionHighlightColorChanged);

    private static void OnSelectionHighlightColorChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is not Entry entry)
        {
            return;
        }

        if (newValue is not Color color)
        {
            throw new InvalidOperationException($"The type of the parameter of {typeof(EntryProperties).FullName}.{nameof(SelectionHighlightColorProperty)} must be a {typeof(Color).FullName}.");
        }

#if WINDOWS
        if (entry.Handler?.PlatformView is Microsoft.UI.Xaml.Controls.TextBox textBox)
        {
            textBox.SelectionHighlightColor = new Microsoft.UI.Xaml.Media.SolidColorBrush(color.ToWindowsColor());
            textBox.BorderThickness = new Microsoft.UI.Xaml.Thickness(0);

            textBox.Style = null;
        }
#elif ANDROID
        if (entry.Handler?.PlatformView is AndroidX.AppCompat.Widget.AppCompatEditText editText)
        {
            editText.SetHighlightColor(color.ToPlatform());
        }

#elif IOS || MACCATALYST
        if (entry.Handler?.PlatformView is UIKit.UITextField textField)
        {
            textField.TintColor = color.ToPlatform();
        }
#endif
    }

    public static void SetSelectionHighlightColor(BindableObject bindable, Color value)
    {
        bindable.SetValue(SelectionHighlightColorProperty, value);
    }

    public static Color GetSelectionHighlightColor(BindableObject bindable)
    {
        return (Color)bindable.GetValue(SelectionHighlightColorProperty);
    }
}
