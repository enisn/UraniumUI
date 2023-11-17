using UraniumApp.Converters;

namespace UraniumApp;
public static class UraniumConverters
{
    public static BooleanInverter BooleanInverter { get; } = new();

    public static BoolToOpactityConverter BoolToOpactityConverter { get; } = new();
}
