using InputKit.Shared.Validations;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using UraniumUI.Extensions;

namespace UraniumUI.Options;
public sealed class AutoFormViewOptions
{
    public Dictionary<Type, EditorForType> EditorMapping { get; } = new();

    public Dictionary<Type, EditorForProperty> PropertyEditorMapping { get; } = new();

    public delegate View EditorForType(PropertyInfo property, Func<PropertyInfo, string> propertyNameFactory, object source);

    public delegate View EditorForProperty(AutoFormProperty property, Func<AutoFormProperty, string> propertyNameFactory, object source);

    public List<Action<View, PropertyInfo>> PostEditorActions { get; } = new();

    public List<Action<View, AutoFormProperty>> PostPropertyEditorActions { get; } = new();

    public Func<PropertyInfo, string> PropertyNameFactory { get; set; } = DefaultPropertyNameFactory;

    public Func<AutoFormProperty, string> PropertyDisplayNameFactory { get; set; }

    public Func<PropertyInfo, IEnumerable<IValidation>> ValidationFactory { get; set; }

    public Func<AutoFormProperty, IEnumerable<IValidation>> PropertyValidationFactory { get; set; }

    public Func<object, IEnumerable<AutoFormProperty>> PropertyProvider { get; set; }

    public string GetPropertyDisplayName(AutoFormProperty property)
    {
        ArgumentNullException.ThrowIfNull(property);

        if (PropertyDisplayNameFactory != null)
        {
            return PropertyDisplayNameFactory(property);
        }

        if (property.PropertyInfo != null)
        {
            return PropertyNameFactory(property.PropertyInfo);
        }

        return property.GetDefaultDisplayName();
    }

    public IEnumerable<IValidation> CreateValidations(AutoFormProperty property)
    {
        ArgumentNullException.ThrowIfNull(property);

        if (PropertyValidationFactory != null)
        {
            return PropertyValidationFactory(property) ?? Enumerable.Empty<IValidation>();
        }

        if (ValidationFactory != null && property.PropertyInfo != null)
        {
            return ValidationFactory(property.PropertyInfo) ?? Enumerable.Empty<IValidation>();
        }

        return Enumerable.Empty<IValidation>();
    }

    public EditorForProperty GetPropertyEditor(AutoFormProperty property)
    {
        ArgumentNullException.ThrowIfNull(property);

        return PropertyEditorMapping.FirstOrDefault(x => x.Key.IsAssignableFrom(property.PropertyType.AsNonNullable())).Value;
    }

    public EditorForType GetLegacyEditor(AutoFormProperty property)
    {
        ArgumentNullException.ThrowIfNull(property);

        if (property.PropertyInfo is null)
        {
            return null;
        }

        return EditorMapping.FirstOrDefault(x => x.Key.IsAssignableFrom(property.PropertyType.AsNonNullable())).Value;
    }

    private static string DefaultPropertyNameFactory(PropertyInfo property)
    {
        var attribute = property.GetCustomAttribute<DisplayAttribute>();
        if (attribute != null)
        {
            return attribute.GetName();
        }

        return property.Name;
    }
}
