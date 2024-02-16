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
}
