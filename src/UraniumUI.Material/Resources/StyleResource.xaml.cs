using UraniumUI.Extensions;

namespace UraniumUI.Material.Resources;

public partial class StyleResource : ResourceDictionary
{
    private ResourceDictionary basedOn;
    private ResourceDictionary overriddenBy;
    private ResourceDictionary colorsOverride;

    public StyleResource()
    {
        InitializeComponent();
    }

    public ResourceDictionary BasedOn
    {
        get => basedOn;
        set
        {
            basedOn = value;
            if (value != null)
            {
                ApplyBasedOn();
            }
        }
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
    public ResourceDictionary ColorsOverride
    {
        get => colorsOverride;
        set
        {
            colorsOverride = value;
            ApplyColorOverride();
        }
    }

    protected virtual void ApplyOverriddenBy()
    {
        var thisStyleDict = this.MergedDictionaries.Skip(1).First();

        foreach (var key in thisStyleDict.Keys)
        {
            if (OverriddenBy.TryGetValue(key, out object value) && value is Style overriderStyle)
            {
                if (thisStyleDict[key] is Style thisStyle)
                {
                    thisStyle.OverrideBy(overriderStyle);
                }
            }
        }
    }

    protected virtual void ApplyColorOverride()
    {
        var thisColorDict = this.MergedDictionaries.First();

        foreach (var overrideKey in ColorsOverride.Keys)
        {
            if (thisColorDict.TryGetValue(overrideKey, out object value) && value is Color thisColor)
            {
                thisColorDict[overrideKey] = ColorsOverride[overrideKey];
            }
        }

    }

    protected virtual void ApplyBasedOn()
    {
        var styleDict = this.MergedDictionaries.Skip(1).First();

        foreach (var key in styleDict.Keys)
        {
            if (BasedOn.TryGetValue(key, out object value) && value is Style baseStyle)
            {
                if (styleDict[key] is Style thisStyle)
                {
                    thisStyle.BaseOn(baseStyle);
                }
            }
        }
    }
}
