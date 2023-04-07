using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using UraniumUI.Extensions;

namespace UraniumUI.Material.Resources;

public partial class StyleResource : ResourceDictionary, Microsoft.Maui.Controls.Internals.IResourceDictionary
{
    private ResourceDictionary basedOn;
    private ResourceDictionary colorsOverride;

    public StyleResource()
    {
        InitializeComponent();
        Overrides.CollectionChanged += (s, e) =>
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                foreach (var item in e.NewItems)
                {
                    ApplyOverriddenBy(item as ResourceDictionary);
				}
			}
		};

		(this as Microsoft.Maui.Controls.Internals.IResourceDictionary).ValuesChanged += StyleResource_ValuesChanged;
    }

	private void StyleResource_ValuesChanged(object sender, Microsoft.Maui.Controls.Internals.ResourcesChangedEventArgs e)
	{
        Console.WriteLine(  ".....");
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

    public ObservableCollection<ResourceDictionary> Overrides { get; set; } = new();

    public ResourceDictionary ColorsOverride
    {
        get => colorsOverride;
        set
        {
            colorsOverride = value;
            ApplyColorOverride();
        }
    }

    protected virtual void ApplyOverriddenBy(ResourceDictionary overriddenBy)
    {
        var thisStyleDict = this.MergedDictionaries.Skip(1).First();

        foreach (var key in thisStyleDict.Keys)
        {
            if (overriddenBy.TryGetValue(key, out object value) && value is Style overriderStyle)
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
