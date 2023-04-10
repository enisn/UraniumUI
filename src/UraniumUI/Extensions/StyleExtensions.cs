namespace UraniumUI.Extensions;
public static class StyleExtensions
{
    public static void OverrideBy(this Style source, Style overrider)
    {
        foreach (var overriderSetter in overrider.Setters)
        {
            var existingSourceSetter = source.Setters.FirstOrDefault(x => x.Property.PropertyName == overriderSetter.Property.PropertyName);

            if (existingSourceSetter != null)
            {
                existingSourceSetter.Value = overriderSetter.Value;
            }
            else
            {
                source.Setters.Add(overriderSetter);
            }
        }
    }

    public static void BaseOn(this Style source, Style basedOn)
    {
        foreach (var setter in basedOn.Setters)
        {
            if (source.Setters.Any(x => x.Property.PropertyName == setter.Property.PropertyName))
            {
                continue;
            }

            source.Setters.Add(setter);
        }
    }
}
