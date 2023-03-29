namespace UraniumUI.Material.Resources;

public partial class ColorResource : ResourceDictionary
{
    private ResourceDictionary basedOn;
    private ResourceDictionary overriddenBy;

    public ColorResource()
    {
        InitializeComponent();
    }

    public ResourceDictionary OverriddenBy
    {
        get => overriddenBy;
        set
        {
            overriddenBy = value;
            if (value != null)
            {
                ApplyOverriddenBy();
            }
        }
    }

    private void ApplyOverriddenBy()
    {
        foreach (var key in OverriddenBy.Keys)
        {
            if (OverriddenBy.TryGetValue(key, out object value) && value is Color overridenColor)
            {
                if (this[key] is Color)
                {
                    this[key] = overridenColor;
                }
            }
        }
    }
}
