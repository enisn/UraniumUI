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
}