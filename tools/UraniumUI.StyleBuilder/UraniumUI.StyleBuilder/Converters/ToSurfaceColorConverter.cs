using InputKit.Shared.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UraniumUI.StyleBuilder.Converters;
public class ToSurfaceColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (targetType != typeof(Color))
        {
            throw new InvalidOperationException("targetType must be Color!");
        }

        if (value is Color color)
        {
            return color.ToSurfaceColor();
        }
        else
        {
            throw new InvalidOperationException("value must be Color!");
        }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
