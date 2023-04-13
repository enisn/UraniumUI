using DotNurse.Injector.Attributes;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace UraniumUI.StyleBuilder.StyleManager;

[RegisterAs(typeof(ColorStyleManager))]
public partial class ColorStyleManager : ReactiveObject
{
    protected Xml.ResourceDictionary XmlNode { get; set; }

    [Reactive] public ColorPalette Palette { get; protected set; }

    [Reactive] public string Path { get; set; }

    public bool IsLoaded => XmlNode != null;

    public async Task LoadAsync(string path)
    {
        var serializer = new XmlSerializer(typeof(Xml.ResourceDictionary));
        using (StringReader reader = new StringReader(await File.ReadAllTextAsync(path)))
        {
            XmlNode = (Xml.ResourceDictionary)serializer.Deserialize(reader);
        }

        Path = path;

        Palette = new ColorPalette(XmlNode);
    }

    public async Task SaveAsync(string path)
    {
        using (TextWriter sw = new StreamWriter(path ?? Path, false, Encoding.UTF8)) //Set encoding
        {
            var serializer = new XmlSerializer(typeof(Xml.ResourceDictionary));
            serializer.Serialize(sw, XmlNode);

            await sw.WriteAsync(XamlCompileDecleration);
        }
    }

    public async Task SaveAsAsync()
    {
        using (var stream = new MemoryStream())
        {
            var serializer = new XmlSerializer(typeof(Xml.ResourceDictionary));
            serializer.Serialize(stream, XmlNode);

            await CommunityToolkit.Maui.Storage.FileSaver.Default.SaveAsync(
                initialPath: System.IO.Path.GetDirectoryName(Path),
                "Colors.xaml", stream, CancellationToken.None);
        }
    }

    public async Task CreateNewAsync()
    {
        Path = null;
        Palette = new ColorPalette(withDefaultValues: true);
        XmlNode = new Xml.ResourceDictionary()
        {
            Colors = Palette.GetType()
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(x => x.PropertyType == typeof(Color))
            .Select(x => new Xml.Color
            {
                Key = x.Name,
                Text = (x.GetValue(Palette) as Color).ToHex(),
            }).ToList()
        };
    }

    const string XamlCompileDecleration = "\n<?xaml-comp compile=\"true\" ?>";
}
