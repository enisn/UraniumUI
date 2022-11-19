namespace UraniumUI.Theming;
public static class CascadingStyle
{
    public static readonly BindableProperty ResourcesProperty = BindableProperty.CreateAttached(
        "Resources",
        typeof(ResourceDictionary),
        typeof(CascadingStyle),
        null,
        propertyChanged: OnResourcesChanged);

    public static ResourceDictionary GetResources(BindableObject view)
    {
        return (ResourceDictionary)view.GetValue(ResourcesProperty);
    }

    public static void SetResources(BindableObject view, ResourceDictionary value)
    {
        view.SetValue(ResourcesProperty, value);
    }

    private static void OnResourcesChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is not View view)
        {
            return;
        }

        if (newValue is ResourceDictionary newResources)
        {
            foreach (var item in newResources)
            {
                view.Resources.Add(item.Key, item.Value);
            }
        }
    }

    public static readonly BindableProperty StyleClassProperty = BindableProperty.CreateAttached(
        "StyleClass",
        typeof(string),
        typeof(CascadingStyle),
        null,
        propertyChanged: OnStyleClassChanged);


    public static string GetStyleClass(BindableObject view)
    {
        return (string)view.GetValue(StyleClassProperty);
    }

    public static void SetStyleClass(BindableObject view, string value)
    {
        view.SetValue(StyleClassProperty, value);
    }

    private static void OnStyleClassChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is not View view)
        {
            return;
        }

        if (newValue is string newStyleClass)
        {
            var newStyleClasses = new ListStringTypeConverter().ConvertFrom(newStyleClass) as List<string>;
            if (view.StyleClass is null)
            {
                view.StyleClass = newStyleClasses;
                return;
            }
            
            foreach (var styleClass in newStyleClasses)
            {
                view.StyleClass.Add(styleClass);
            }
        }
    }
}