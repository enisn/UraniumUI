namespace UraniumUI.Resources;

public static class ColorResource
{
    public static Color GetColor(string key, Color fallBack = default)
    {
        if (Application.Current?.Resources.TryGetValue(key, out object value) == true)
        {
            return (Color)value;
        }
        else
        {
            return fallBack ?? Colors.Transparent;
        }
    }

    public static Color GetColor(string lightKey, string darkKey, Color fallBack = default)
    {
        if (Application.Current is null)
        {
            return fallBack ?? Colors.Transparent;
        }

        var key = Application.Current.RequestedTheme == AppTheme.Light ? lightKey : darkKey;

        if (Application.Current.Resources.TryGetValue(key, out object value))
        {
            return (Color)value;
        }
        else
        {
            return fallBack ?? Colors.Transparent;
        }
    }
}
