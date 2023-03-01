namespace UraniumUI.Material.Resources;

public partial class StyleResource : ResourceDictionary
{
    private ResourceDictionary basedOn;

    public StyleResource()
    {
        InitializeComponent();
    }

    public ResourceDictionary MergeWith
    {
        get => basedOn;
        set
        {
            basedOn = value; ApplyMerge();
        }
    }

    public MergeType MergeType { get; set; }

    protected virtual void ApplyMerge()
    {
        var styleDict = this.MergedDictionaries.Skip(1).First();

        foreach (var key in styleDict.Keys)
        {
            if (MergeWith.TryGetValue(key, out object value) && value is Style baseStyle)
            {
                if (styleDict[key] is Style thisStyle)
                {
                    MapStyle(baseStyle, thisStyle);
                }
            }
        }
    }

    protected virtual void MapStyle(Style source, Style destination)
    {
        if (MergeType == MergeType.DestionationPrefferred)
        {
            foreach (var setter in source.Setters)
            {
                if (destination.Setters.Any(x => x.Property.PropertyName == setter.Property.PropertyName))
                {
                    continue;
                }

                destination.Setters.Add(setter);
            }
        }
        else
        {
            foreach (var setter in source.Setters)
            {
                destination.Setters.Remove(
                    destination.Setters.FirstOrDefault(x => x.Property.PropertyName == setter.Property.PropertyName));
                destination.Setters.Add(setter);
            }
        }
    }
}

public enum MergeType
{
    DestionationPrefferred,
    SourcePrefferred,
}
