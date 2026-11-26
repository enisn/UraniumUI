using System.Reflection;
using System.Windows.Input;

namespace UraniumUI.Material.Tests.Controls;

/// <summary>
/// The bindable properties a field declares under the same name as its inner view's, which the field forwards to that view.
/// </summary>
internal static class ForwardedProperties
{
    public static IEnumerable<(BindableProperty Field, BindableProperty Inner)> Of<TField, TInner>() => typeof(TField)
        .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
        .Where(field => field.FieldType == typeof(BindableProperty))
        .Select(field => (Field: (BindableProperty)field.GetValue(null), Inner: typeof(TInner).GetField(field.Name, BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)?.GetValue(null) as BindableProperty))
        .Where(pair => pair.Inner is not null);

    /// <summary>The properties whose default differs from the inner view's, except <paramref name="intentional"/>.</summary>
    public static List<string> WithDivergingDefaults<TField, TInner>(params string[] intentional) => Of<TField, TInner>()
        .Where(pair => !intentional.Contains(pair.Field.PropertyName) && !Equals(pair.Field.DefaultValue, pair.Inner.DefaultValue))
        .Select(pair => $"{pair.Field.PropertyName}: {pair.Field.DefaultValue} instead of {pair.Inner.DefaultValue}")
        .ToList();

    /// <summary>The properties whose non-default value, set on <paramref name="field"/>, does not reach <paramref name="inner"/>.</summary>
    public static List<string> NotReaching<TField, TInner>(TField field, TInner inner)
        where TField : BindableObject
        where TInner : BindableObject
    {
        var notForwarded = new List<string>();
        foreach (var (fieldProperty, innerProperty) in Of<TField, TInner>())
        {
            var value = NonDefaultValue(fieldProperty);
            field.SetValue(fieldProperty, value);
            if (!Equals(inner.GetValue(innerProperty), value))
            {
                notForwarded.Add($"{fieldProperty.PropertyName}: {inner.GetValue(innerProperty)} instead of {value}");
            }
        }

        return notForwarded;
    }

    /// <summary>A value of the property's type that differs from its default; a new type fails here until it gets one.</summary>
    private static object NonDefaultValue(BindableProperty property) => property.DefaultValue switch
    {
        bool value => !value,
        int value => value == 7 ? 8 : 7,
        double value => value + 2,
        string value => value + "forwarded",
        Color color => color == Colors.Red ? Colors.Blue : Colors.Red,
        Keyboard => Keyboard.Numeric,
        Enum value => Enum.GetValues(value.GetType()).Cast<object>().First(other => !other.Equals(value)),
        null when property.ReturnType == typeof(string) => "forwarded",
        null when property.ReturnType == typeof(Color) => Colors.Red,
        null when property.ReturnType == typeof(ICommand) => new Command(() => { }),
        null when property.ReturnType == typeof(object) => new object(),
        _ => throw new NotSupportedException($"No non-default value for {property.PropertyName} of type {property.ReturnType.Name}."),
    };
}
