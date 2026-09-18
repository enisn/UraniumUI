using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UraniumUI.Material.Converters;


public class UniversalGridLengthConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        culture ??= CultureInfo.CurrentCulture;

        return value switch
        {
            null => GridLength.Auto,
            GridLength gl => gl,

            // Строковые форматы
            string str => ParseAnyGridLengthFormat(str, culture),

            // Числовые типы
            double d => new GridLength(d),
            int i => new GridLength(i),
            float f => new GridLength(f),
            decimal dec => new GridLength((double)dec),

            // Из массива [value, type]
            object[] arr when arr.Length == 2 && arr[0] is IConvertible val && arr[1] is GridUnitType type =>
                new GridLength(System.Convert.ToDouble(val, culture), type),

            _ => GridLength.Auto
        };
    }


    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        culture ??= CultureInfo.CurrentCulture;

        if (value is not GridLength gridLength)
        {
            // Возвращаем значение по умолчанию в зависимости от типа
            if (targetType == typeof(string))
                return "Auto";
            if (targetType == typeof(double))
                return -1.0;
            if (targetType == typeof(GridLength))
                return GridLength.Auto;

            return targetType.IsValueType ? Activator.CreateInstance(targetType) : null;
        }

        // Преобразуем GridLength в нужный тип
        if (targetType == typeof(string))
            return GridLengthToString(gridLength, culture);

        if (targetType == typeof(double) || targetType == typeof(float) || targetType == typeof(int))
        {
            // Для числовых типов возвращаем значение только если это Absolute
            if (gridLength.GridUnitType == GridUnitType.Absolute)
            {
                return targetType == typeof(int) ? (int)gridLength.Value : gridLength.Value;
            }
            return targetType == typeof(int) ? -1 : -1.0;
        }

        if (targetType == typeof(GridLength))
            return gridLength;

        return gridLength.Value; // По умолчанию double
    }


    /*


    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        culture ??= CultureInfo.CurrentCulture;

        if (value is not GridLength gridLength)
            return targetType.IsValueType ? Activator.CreateInstance(targetType) : null;

        return targetType.Name switch
        {
            nameof(Double) or "Double" => gridLength.Value,
            nameof(Single) or "Single" => (float)gridLength.Value,
            nameof(Int32) or "Int32" => (int)gridLength.Value,
            nameof(String) or "String" => GridLengthToString(gridLength, culture),
            _ => gridLength.Value // По умолчанию double
        };
    }  */

    private GridLength ParseAnyGridLengthFormat(string value, CultureInfo culture)
    {
        value = value?.Trim() ?? string.Empty;

        // Специальные значения
        if (value.Equals("Auto", StringComparison.OrdinalIgnoreCase) ||
            value.Equals("A", StringComparison.OrdinalIgnoreCase))
            return GridLength.Auto;

        // Проверяем все возможные форматы
        if (TryParseGridLength(value, culture, out var result))
            return result;

        // Пробуем заменить запятую на точку
        string normalized = value.Replace(',', '.');
        if (TryParseGridLength(normalized, culture, out result))
            return result;

        return GridLength.Auto;
    }

    private bool TryParseGridLength(string value, CultureInfo culture, out GridLength result)
    {
        result = GridLength.Auto;

        if (string.IsNullOrWhiteSpace(value))
            return false;

        // Формат: "100" (пиксели)
        if (double.TryParse(value, NumberStyles.Float, culture, out double pixels))
        {
            result = new GridLength(pixels);
            return true;
        }

        // Формат: "100*" или "*" (звездочки)
        if (value.EndsWith("*"))
        {
            string numberPart = value[..^1].Trim();

            if (string.IsNullOrEmpty(numberPart))
            {
                result = GridLength.Star;
                return true;
            }

            if (double.TryParse(numberPart, NumberStyles.Float, culture, out double stars))
            {
                result = new GridLength(stars, GridUnitType.Star);
                return true;
            }
        }

        // Формат: "100px"
        if (value.EndsWith("px", StringComparison.OrdinalIgnoreCase))
        {
            string numberPart = value[..^2].Trim();
            if (double.TryParse(numberPart, NumberStyles.Float, culture, out double px))
            {
                result = new GridLength(px);
                return true;
            }
        }

        return false;
    }

    private string GridLengthToString(GridLength gridLength, CultureInfo culture)
    {
        return gridLength.GridUnitType switch
        {
            GridUnitType.Absolute => $"{gridLength.Value.ToString(culture)}",
            GridUnitType.Star => gridLength.Value == 1 ? "*" : $"{gridLength.Value.ToString(culture)}*",
            GridUnitType.Auto => "Auto",
            _ => gridLength.Value.ToString(culture)
        };
    }
}
