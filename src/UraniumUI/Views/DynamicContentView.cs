using System.Collections.ObjectModel;
using System.ComponentModel;
using UraniumUI.Converters;

namespace UraniumUI.Views;
public class DynamicContentView : ContentView
{
    public static readonly BindableProperty ValueProperty = BindableProperty.Create("Value", typeof(object), typeof(object), propertyChanged: (bo, ov, nv) => (bo as DynamicContentView).OnValueChanged());

    [TypeConverter(typeof(DynamicContentValueTypeConverter))]
    public object Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    public ObservableCollection<ValueCondition> Conditions { get; set; } = new();

    public DynamicContentView()
    {
        Conditions.CollectionChanged += (o, e) => OnValueChanged();
    }

    protected virtual void OnValueChanged()
    {
        if (Conditions == null || !Conditions.Any())
        {
            Content = null;
            return;
        }

        foreach (var condition in Conditions)
        {
            if (condition.HasMet(Value))
            {
                var newContent = condition.GetContent();
                if (Content != newContent)
                {
                    Content = newContent;
                }

                break;
            }
        }
    }
}

[ContentProperty(nameof(ContentTemplate))]
public class ValueCondition : BindableObject
{
    [TypeConverter(typeof(DynamicContentValueTypeConverter))]
    public object Equal { get; set; }

    [TypeConverter(typeof(DynamicContentValueTypeConverter))]
    public object GreaterThan { get; set; }

    [TypeConverter(typeof(DynamicContentValueTypeConverter))]
    public object LessThan { get; set; }

    [TypeConverter(typeof(DynamicContentValueTypeConverter))]
    public object Not { get; set; }

    public DataTemplate ContentTemplate { get; set; }

    internal View Content { get; private set; }

    public bool HasMet(object value)
    {
        if (Equal != null && value.Equals(Equal))
        {
            return true;
        }

        if (GreaterThan != null && value is IComparable comparable && comparable.CompareTo(GreaterThan) > 0)
        {
            return true;
        }

        if (LessThan != null && value is IComparable comparable2 && comparable2.CompareTo(LessThan) < 0)
        {
            return true;
        }

        if (Not != null && !value.Equals(Not))
        {
            return true;
        }

        return false;
    }

    public View GetContent()
    {
        if (Content == null)
        {
            Content = ContentTemplate.CreateContent() as View;
        }

        return Content;
    }
}
