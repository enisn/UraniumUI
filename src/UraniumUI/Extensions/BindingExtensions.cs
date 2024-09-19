using System.Globalization;

namespace UraniumUI.Extensions;
public static class BindingExtensions
{
    public static BindingBase CopyAsClone(this BindingBase binding)
    {
        if (binding is Binding b)
        {
            return new Binding(b.Path, b.Mode, b.Converter, b.ConverterParameter, b.StringFormat, b.Source);
        }

        if (binding is MultiBinding mb)
        {
            return new MultiBinding
            {
                Converter = mb.Converter,
                ConverterParameter = mb.ConverterParameter,
                StringFormat = mb.StringFormat,
                Mode = mb.Mode,
                Bindings = mb.Bindings.Select(CopyAsClone).ToList()
            };
        }

        if (binding is BindingBase)
        {
            throw new NotSupportedException("Abstract classes or BindingBase types can not be cloned.");
        }

        return default;
    }

    public static T GetValueOnce<T>(this BindingBase binding, object source)
    {
        if (binding is Binding b)
        {
            var _value = source.GetType().GetProperty(b.Path).GetValue(source);
            if (b.Converter != null)
            {
                b.Converter.Convert(_value, typeof(T), b.ConverterParameter, CultureInfo.CurrentCulture);
            }

            return (T)_value;
        }

        if (binding is MultiBinding mb)
        {
            var values = new List<object>();
            foreach (var bind in mb.Bindings)
            {
                values.Add(bind.GetValueOnce<object>(source));
            }
            return (T)mb.Converter.Convert(values.ToArray(), typeof(T), null, null);
        }

        if (binding is BindingBase)
        {
            throw new NotSupportedException("Abstract classes or BindingBase types can not be cloned.");
        }

        return (T)binding.FallbackValue;
    }
}
