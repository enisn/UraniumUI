using System.Globalization;

namespace UraniumUI.Converters;
public class StringIsNotNullOrEmptyConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not string text)
        {
            text = value?.ToString();
        }

        return !string.IsNullOrEmpty(text);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
