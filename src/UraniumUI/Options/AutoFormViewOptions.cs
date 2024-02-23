using System.Reflection;

namespace UraniumUI.Options;
public class AutoFormViewOptions
{
    public Dictionary<Type, EditorForType> EditorMapping { get; } = new();

    public delegate View EditorForType(PropertyInfo property, object source);
}
