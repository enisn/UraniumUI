using System.ComponentModel;
using System.Globalization;

namespace UraniumUI.Converters;

public class DynamicContentValueTypeConverter : TypeConverter
{
    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
    {
        return true;
    }

    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
    {
        if (value is string s)
        {
            if (bool.TryParse(s, out bool boolean))
            {
                return boolean;
            }

            if (int.TryParse(s, out int number))
            {
                return number;
            }

            if (double.TryParse(s, out double doubleNumber))
            {
                return doubleNumber;
            }

            return s;
        }

        return value;
    }
}