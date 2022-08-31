namespace UraniumUI.Extensions;
public static class NumericExtensions
{
    public static double Clamp(this double value, double min, double max)
    {
        if (value > max)
        {
            return max;
        }

        if (value < min)
        {
            return min;
        }

        return value;
    }
}
