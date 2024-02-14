namespace UraniumUI.Material.Controls;

public class DataGridValueBindingExtension : IMarkupExtension, IMarkupExtension<BindingBase>
{
    public BindingMode Mode { get; set; }
    public object ProvideValue(IServiceProvider serviceProvider)
    {
        var targetObject = serviceProvider.GetRequiredService<IProvideValueTarget>()
            .TargetObject as View;

        var targetProperty = serviceProvider.GetRequiredService<IProvideValueTarget>()
           .TargetProperty as BindableProperty;

        targetObject.BindingContextChanged += (s, e) =>
        {
            if (targetObject.BindingContext is BindingBase binding)
            {
                binding.Mode = Mode;
                targetObject.SetBinding(targetProperty, binding);
            }
        };

        return default;
    }

    BindingBase IMarkupExtension<BindingBase>.ProvideValue(IServiceProvider serviceProvider)
    {
        return (this as IMarkupExtension<BindingBase>).ProvideValue(serviceProvider);
    }
}