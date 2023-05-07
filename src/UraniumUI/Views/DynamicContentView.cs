namespace UraniumUI.Views;
public class DynamicContentView : ContentView
{
    public static readonly BindableProperty ValueProperty = BindableProperty.Create("Value", typeof(object), typeof(object), propertyChanged: (bo,ov,nv)=>(bo as DynamicContentView).OnValueChanged());

    public object Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    public List<ValueCondition> Conditions { get; set; } = new();

    protected virtual void OnValueChanged()
    {
        if (Conditions ==  null || !Conditions.Any())
        {
            Content = null;
            return;
        }

        foreach (var condition in Conditions)
        {
            if (condition.HasMet(Value))
            {
                Content = condition.GetContent();

                break;
            }
        }
    }
}

[ContentProperty(nameof(ContentTemplate))]
public class ValueCondition : BindableObject
{
    public object Equal { get; set; }

    public object GreaterThan { get; set; }

    public object LesserThan { get; set; }

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

        if (LesserThan != null && value is IComparable comparable2 && comparable2.CompareTo(LesserThan) < 0)
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
