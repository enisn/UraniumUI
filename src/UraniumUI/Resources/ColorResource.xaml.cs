namespace UraniumUI.Resources;

public partial class ColorResource : ResourceDictionary
{
    public ColorResource()
    {
        InitializeComponent();
    }
    
    public static Color GetColor(string key, Color fallBack = default)
    {
        if (Application.Current.Resources.TryGetValue(key, out object value))
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
