using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Reflection;

namespace UraniumUI.StyleBuilder.StyleManager;

public class ColorPalette : ReactiveObject
{
    public ColorPalette(bool withDefaultValues = true)
    {
        if (withDefaultValues)
        {
            foreach (var property in GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                if (App.Current.Resources.TryGetValue(property.Name, out object valueObj) && valueObj is Color color)
                {
                    property.SetValue(this, color);
                }
            }
        }
    }

    public ColorPalette(Xml.ResourceDictionary dict)
    {
        foreach (var property in GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
        {
            var colorNode = dict.Colors.FirstOrDefault(x => x.Key == property.Name);
            if (colorNode != null)
            {
                property.SetValue(this, Color.FromArgb(colorNode.Text));
            }
        }
    }

    [Reactive] public Color Primary { get; set; }
    [Reactive] public Color OnPrimary { get; set; }
    [Reactive] public Color PrimaryContainer { get; set; }
    [Reactive] public Color OnPrimaryContainer { get; set; }
    [Reactive] public Color Secondary { get; set; }
    [Reactive] public Color OnSecondary { get; set; }
    [Reactive] public Color SecondaryContainer { get; set; }
    [Reactive] public Color OnSecondaryContainer { get; set; }
    [Reactive] public Color Tertiary { get; set; }
    [Reactive] public Color OnTertiary { get; set; }
    [Reactive] public Color TertiaryContainer { get; set; }
    [Reactive] public Color OnTertiaryContainer { get; set; }
    [Reactive] public Color Surface { get; set; }
    [Reactive] public Color OnSurface { get; set; }
    [Reactive] public Color SurfaceVariant { get; set; }
    [Reactive] public Color OnSurfaceVariant { get; set; }
    [Reactive] public Color Background { get; set; }
    [Reactive] public Color OnBackground { get; set; }
    [Reactive] public Color Error { get; set; }
    [Reactive] public Color OnError { get; set; }
    [Reactive] public Color ErrorContainer { get; set; }
    [Reactive] public Color OnErrorContainer { get; set; }
    [Reactive] public Color Outline { get; set; }
    [Reactive] public Color OutlineVariant { get; set; }
    [Reactive] public Color Shadow { get; set; }

    [Reactive] public Color PrimaryDark { get; set; }
    [Reactive] public Color OnPrimaryDark { get; set; }
    [Reactive] public Color PrimaryContainerDark { get; set; }
    [Reactive] public Color OnPrimaryContainerDark { get; set; }
    [Reactive] public Color SecondaryDark { get; set; }
    [Reactive] public Color OnSecondaryDark { get; set; }
    [Reactive] public Color SecondaryContainerDark { get; set; }
    [Reactive] public Color OnSecondaryContainerDark { get; set; }
    [Reactive] public Color TertiaryDark { get; set; }
    [Reactive] public Color OnTertiaryDark { get; set; }
    [Reactive] public Color TertiaryContainerDark { get; set; }
    [Reactive] public Color OnTertiaryContainerDark { get; set; }
    [Reactive] public Color SurfaceDark { get; set; }
    [Reactive] public Color OnSurfaceDark { get; set; }
    [Reactive] public Color SurfaceVariantDark { get; set; }
    [Reactive] public Color OnSurfaceVariantDark { get; set; }
    [Reactive] public Color BackgroundDark { get; set; }
    [Reactive] public Color OnBackgroundDark { get; set; }
    [Reactive] public Color ErrorDark { get; set; }
    [Reactive] public Color OnErrorDark { get; set; }
    [Reactive] public Color ErrorContainerDark { get; set; }
    [Reactive] public Color OnErrorContainerDark { get; set; }
    [Reactive] public Color OutlineDark { get; set; }
    [Reactive] public Color OutlineVariantDark { get; set; }
    [Reactive] public Color ShadowDark { get; set; }
}
