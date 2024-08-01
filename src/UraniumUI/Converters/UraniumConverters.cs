using System.Reflection;

namespace UraniumUI.Converters;
public static class UraniumConverters
{
    public static StringIsNotNullOrEmptyConverter StringIsNotNullOrEmptyConverter { get; } = new();
}
