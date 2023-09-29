namespace UraniumUI.Extensions;
public static class ColorExtensions
{
    public static bool IsNullOrTransparent(this Color color)
    {
        return color == null || color == Colors.Transparent;
    }

    public static Brush ToSolidColorBrush(this Color color)
    {
        return new SolidColorBrush(color);
    }
}
