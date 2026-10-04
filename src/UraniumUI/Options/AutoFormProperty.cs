using System.ComponentModel.DataAnnotations;
using System.Reflection;
using UraniumUI.Extensions;

namespace UraniumUI.Options;

public sealed class AutoFormProperty
{
    private readonly IReadOnlyList<Attribute> attributes;

    public AutoFormProperty(
        string name,
        Type propertyType,
        string displayName = null,
        bool isNullable = false,
        IEnumerable<Attribute> attributes = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        Name = name;
        PropertyType = propertyType ?? throw new ArgumentNullException(nameof(propertyType));
        DisplayName = displayName;
        IsNullable = isNullable;
        this.attributes = attributes?.ToArray() ?? Array.Empty<Attribute>();
    }

    private AutoFormProperty(PropertyInfo propertyInfo)
        : this(
            propertyInfo.Name,
            propertyInfo.PropertyType,
            isNullable: propertyInfo.PropertyType.IsNullable(),
            attributes: propertyInfo.GetCustomAttributes().OfType<Attribute>())
    {
        PropertyInfo = propertyInfo;
    }

    public string Name { get; }

    public Type PropertyType { get; }

    public string DisplayName { get; }

    public bool IsNullable { get; }

    public IReadOnlyList<Attribute> Attributes => attributes;

    public PropertyInfo PropertyInfo { get; }

    public static AutoFormProperty FromPropertyInfo(PropertyInfo propertyInfo)
    {
        ArgumentNullException.ThrowIfNull(propertyInfo);

        return new AutoFormProperty(propertyInfo);
    }

    public TAttribute GetCustomAttribute<TAttribute>() where TAttribute : Attribute
    {
        return attributes.OfType<TAttribute>().FirstOrDefault();
    }

    public IEnumerable<TAttribute> GetCustomAttributes<TAttribute>() where TAttribute : Attribute
    {
        return attributes.OfType<TAttribute>();
    }

    public string GetDefaultDisplayName()
    {
        if (!string.IsNullOrWhiteSpace(DisplayName))
        {
            return DisplayName;
        }

        var displayAttribute = GetCustomAttribute<DisplayAttribute>();
        if (displayAttribute != null)
        {
            return displayAttribute.GetName() ?? Name;
        }

        return Name;
    }
}
