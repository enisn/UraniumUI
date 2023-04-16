using DotNurse.Injector.Attributes;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Collections.ObjectModel;
using System.Text;
using System.Xml.Linq;

namespace UraniumUI.StyleBuilder.StyleManager;

[RegisterAs(typeof(ColorStyleManager))]
public partial class ColorStyleManager : ReactiveObject, IDisposable
{
    [Reactive] public string Path { get; set; }

    public XDocument XmlDoc { get; private set; }

    [Reactive] public ObservableDictionary<string, ReactiveColor> Colors { get; protected set; }

    public async Task LoadAsync(string path)
    {
        await Task.Yield();
        XmlDoc = XDocument.Load(path);
        var ns = XmlDoc.Root.GetDefaultNamespace();
        var xns = XmlDoc.Root.GetNamespaceOfPrefix("x");
        Colors = new ObservableDictionary<string, ReactiveColor>(
                XmlDoc
                    .Descendants(ns + "Color")
                    .ToDictionary(
                        x => x.Attribute(xns + "Key").Value,
                        x => (ReactiveColor) Color.FromRgba(x.Value))
            );

        Path = path;
    }

    public async Task SaveAsync(string path)
    {
        ApplyChangesFromPalette();

        using (TextWriter sw = new StreamWriter(path ?? Path, false, Encoding.UTF8)) //Set encoding
        {
            await XmlDoc.SaveAsync(sw, SaveOptions.OmitDuplicateNamespaces, CancellationToken.None);
        }
    }

    public async Task SaveAsAsync()
    {
        ApplyChangesFromPalette();

        using (var stream = new MemoryStream())
        {
            XmlDoc.Save(stream);

            var result = await CommunityToolkit.Maui.Storage.FileSaver.Default.SaveAsync(
                initialPath: System.IO.Path.GetDirectoryName(Path),
                "Colors.xaml", stream, CancellationToken.None);

            if (result.IsSuccessful)
            {
                this.Path = result.FilePath;
            }
        }
    }

    public async Task CreateNewAsync()
    {
        Path = null;
        XmlDoc = new XDocument();
        var _dict = new Dictionary<string, ReactiveColor>();
        foreach (var key in MaterialPalette)
        {
            if(App.Current.Resources.TryGetValue(key, out object valueObj) && valueObj is Color color)
            {
                _dict[key] = color;
            }
        }

        Colors = new ObservableDictionary<string, ReactiveColor>(_dict);
    }

    protected virtual void ApplyChangesFromPalette()
    {
        var ns = XmlDoc.Root.GetDefaultNamespace();
        var xns = XmlDoc.Root.GetNamespaceOfPrefix("x");

        foreach (var colorElement in XmlDoc.Descendants(ns + "Color"))
        {
            if (Colors.TryGetValue(colorElement.Attribute(xns + "Key").Value, out var value))
            {
                colorElement.Value = value.Color.ToHex();
            }
        }
    }

    public void Dispose()
    {
        XmlDoc.RemoveNodes();
        XmlDoc = null;
        Path = null;
    }

    public static readonly string[] MaterialPalette = new[]
    {
        "Primary",
        "OnPrimary",
        "PrimaryContainer",
        "OnPrimaryContainer",
        "Secondary",
        "OnSecondary",
        "SecondaryContainer",
        "OnSecondaryContainer",
        "Tertiary",
        "OnTertiary",
        "TertiaryContainer",
        "OnTertiaryContainer",
        "Surface",
        "OnSurface",
        "SurfaceVariant",
        "OnSurfaceVariant",
        "Background",
        "OnBackground",
        "Error",
        "OnError",
        "ErrorContainer",
        "OnErrorContainer",
        "Outline",
        "OutlineVariant",
        "Shadow",
        "PrimaryDark",
        "OnPrimaryDark",
        "PrimaryContainerDark",
        "OnPrimaryContainerDark",
        "SecondaryDark",
        "OnSecondaryDark",
        "SecondaryContainerDark",
        "OnSecondaryContainerDark",
        "TertiaryDark",
        "OnTertiaryDark",
        "TertiaryContainerDark",
        "OnTertiaryContainerDark",
        "SurfaceDark",
        "OnSurfaceDark",
        "SurfaceVariantDark",
        "OnSurfaceVariantDark",
        "BackgroundDark",
        "OnBackgroundDark",
        "ErrorDark",
        "OnErrorDark",
        "ErrorContainerDark",
        "OnErrorContainerDark",
        "OutlineDark",
        "OutlineVariantDark",
        "ShadowDark",
    };
}
