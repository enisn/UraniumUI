using InputKit.Shared.Validations;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace UraniumUI.Options;
public sealed class AutoFormViewOptions
{
    public Dictionary<Type, EditorForType> EditorMapping { get; } = new();

    public delegate View EditorForType(PropertyInfo property, object source);

    public List<Action<View, PropertyInfo>> PostEditorActions { get; } = new();

    public Func<PropertyInfo, string> PropertyNameFactory { get; set; } = DefaultPropertyNameFactory;

    public Func<PropertyInfo, IEnumerable<IValidation>> ValidationFactory { get; set; }

    private static string DefaultPropertyNameFactory(PropertyInfo property)
    {
        var attribute = property.GetCustomAttribute<DisplayAttribute>();
        if (attribute != null)
        {
            return attribute.Name;
        }

        return property.Name;
    }
}
