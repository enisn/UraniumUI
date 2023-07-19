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

        var propertyInfo = GetProperty(source.GetType(), Path);

        var attributes = propertyInfo .GetCustomAttributes<ValidationAttribute>(true);

        var displayAttribute = propertyInfo.GetCustomAttribute<DisplayAttribute>(true);

        if (targetObject is IValidatable validatable)
        {
            foreach (var attribute in attributes)
            {
                validatable.Validations.Add(new DataAnnotationValidation(attribute, displayAttribute?.GetName() ?? propertyInfo.Name));
            }
        }

        targetObject.SetBinding(targetProperty, new Binding(Path, source: source));

        return null;
    }

    protected virtual PropertyInfo GetProperty(Type type, string propertyName)
    {
        if (!IsNestedProperty(propertyName))
        {
            return type.GetProperty(propertyName);
        }

        var splitted = propertyName.Split('.');

        var property = type.GetProperty(splitted[0]);

        return GetProperty(property.PropertyType, string.Join('.', splitted.Skip(1)));
    }

    protected bool IsNestedProperty(string propertyName)
    {
        return propertyName.Contains('.');
    }
}
