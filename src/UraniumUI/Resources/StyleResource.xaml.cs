namespace UraniumUI.Resources;

public partial class StyleResource : ResourceDictionary
{
    public StyleResource()
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
}
