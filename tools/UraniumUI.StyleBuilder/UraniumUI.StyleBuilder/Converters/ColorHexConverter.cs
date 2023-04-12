using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UraniumUI.StyleBuilder.Converters;
public class ColorHexConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (targetType != typeof(string))
        {
            throw new InvalidOperationException("targetType must be string");
        }

        if (value is Color color)
        {
            return color.ToHex();
        }
        else
        {
            throw new InvalidOperationException("targetType must be Color");
        }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (targetType != typeof(Color))
        {
            throw new InvalidOperationException("targetType must be Color");
        }

        if (value is string hex)
        {
            //if (Color.TryParse(hex, out ))
            //{

            //}
            return Color.FromArgb(hex);
        }
        else
        {
            throw new InvalidOperationException("targetType must be Color");
        }
    }
}