using DotNurse.Injector.Attributes;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace UraniumUI.StyleBuilder.StyleManager;

[RegisterAs(typeof(ColorStyleManager))]
public partial class ColorStyleManager : ReactiveObject, IDisposable
{
    [Reactive] public ColorPalette Palette { get; protected set; }

    [Reactive] public string Path { get; set; }

    public XDocument XmlDoc { get; private set; }

    public async Task LoadAsync(string path)
    {
        await Task.Yield();
        XmlDoc = XDocument.Load(path);
        var ns = XmlDoc.Root.GetDefaultNamespace();
        var xns = XmlDoc.Root.GetNamespaceOfPrefix("x");
        var dict = XmlDoc.Descendants(ns + "Color")
            .ToDictionary(x => x.Attribute(xns + "Key").Value, x => x.Value);

        Path = path;

        Palette = new ColorPalette(dict);
    }

    public async Task SaveAsync(string path)
    {
        var ns = XmlDoc.Root.GetDefaultNamespace();
        var xns = XmlDoc.Root.GetNamespaceOfPrefix("x");

        var colorDictionary = this.Palette.ToDictionary();

        foreach (var colorElement in XmlDoc.Descendants(ns + "Color"))
        {
            if (colorDictionary.TryGetValue(colorElement.Attribute(xns + "Key").Value, out var value))
            {
                colorElement.Value = value;
            }
        }

        using (TextWriter sw = new StreamWriter(path ?? Path, false, Encoding.UTF8)) //Set encoding
        {
            await XmlDoc.SaveAsync(sw, SaveOptions.OmitDuplicateNamespaces, CancellationToken.None);
        }
    }

    public async Task SaveAsAsync()
    {
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
        Palette = new ColorPalette(withDefaultValues: true);
        XmlDoc = new XDocument();
    }

    public void Dispose()
    {
        XmlDoc.RemoveNodes();
        XmlDoc = null;
        Palette = null;
        Path = null;
    }
}
