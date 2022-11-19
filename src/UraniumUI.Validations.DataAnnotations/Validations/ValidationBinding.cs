using InputKit.Shared.Abstraction;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace UraniumUI.Validations;

[ContentProperty(nameof(Path))]
public class ValidationBinding : IMarkupExtension
{
    public string Path { get; set; }

    public object Source { get; set; }

    public object ProvideValue(IServiceProvider serviceProvider)
    {
        var root = serviceProvider.GetRequiredService<IRootObjectProvider>()
            .RootObject as BindableObject;

        var targetObject = serviceProvider.GetRequiredService<IProvideValueTarget>()
            .TargetObject as BindableObject;

        var targetProperty = serviceProvider.GetRequiredService<IProvideValueTarget>()
            .TargetProperty as BindableProperty;

        var source = Source ?? root.BindingContext;

        var attributes = source.GetType().GetProperty(Path).GetCustomAttributes<ValidationAttribute>(true);

        if (targetObject is IValidatable validatable)
        {
            foreach (var attribute in attributes)
            {
                validatable.Validations.Add(new DataAnnotationValidation(attribute, Path));
            }
        }

        targetObject.SetBinding(targetProperty, new Binding(Path, source: source));
        return null;
    }
}
