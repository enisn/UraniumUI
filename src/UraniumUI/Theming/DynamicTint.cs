namespace UraniumUI.Theming;
public static class DynamicTint
{
    public static readonly BindableProperty BackgroundColorOpacityProperty = BindableProperty.CreateAttached(
        "BackgroundColorOpacity",
        typeof(float),
        typeof(DynamicTint),
        defaultValue: 1f,
        propertyChanged: OnBackgroundColorOpacityChanged);

    public static float GetBackgroundColorOpacity(BindableObject view)
    {
        return (float)view.GetValue(BackgroundColorOpacityProperty);
    }

    public static void SetBackgroundColorOpacity(BindableObject view, float value)
    {
        view.SetValue(BackgroundColorOpacityProperty, value);
    }
    
    private static void OnBackgroundColorOpacityChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is not View view)
        {
            return;
        }

        if (newValue is float dynamicTintOpacity)
        {
            view.BackgroundColor = view.BackgroundColor.WithAlpha(dynamicTintOpacity);
        }
    }
}
