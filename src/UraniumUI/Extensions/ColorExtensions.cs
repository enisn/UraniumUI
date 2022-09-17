namespace UraniumUI.Extensions;
public static class ColorExtensions
{
    public static bool IsNullOrTransparent(this Color color)
    {
        return color == null || color == Colors.Transparent;
    }
}
