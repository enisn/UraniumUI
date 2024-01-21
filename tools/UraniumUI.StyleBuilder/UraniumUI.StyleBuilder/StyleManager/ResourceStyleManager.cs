using CommunityToolkit.Maui.Core.Extensions;
using DotNurse.Injector.Attributes;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Collections.ObjectModel;
using System.Xml.Linq;

namespace UraniumUI.StyleBuilder.StyleManager;

[RegisterAs(typeof(ResourceStyleManager))]
public class ResourceStyleManager : ReactiveObject, IDisposable
{
    [Reactive] public string Path { get; set; }
    public XDocument XmlDoc { get; private set; }

    [Reactive] public ObservableCollection<ReactiveStyle> Styles { get; set; }

    public async Task LoadAsync(string path)
    {
        await Task.Yield();
        XmlDoc = XDocument.Load(path);

        InitializeFromXmlDoc();

        Path = path;
    }

    protected virtual void InitializeFromXmlDoc()
    {
        var dict = new ResourceDictionary();

        global::Microsoft.Maui.Controls.Xaml.Extensions.LoadFromXaml(dict, XmlDoc.ToString());

        var _styles = dict.Values.Union(dict.MergedDictionaries.SelectMany(x => x.Values))
            .Where(x => x is Style || x is List<Style>)
            .Select(x =>
            {
                if (x is Style style)
                {
                    return ReactiveStyle.FromStyle((Style)x);
                }
                if (x is List<Style> listOfStyles)
                {
                    return ReactiveStyle.FromStyle(listOfStyles.FirstOrDefault());
                }
                return null;
            })
            .ToObservableCollection();

        Styles = _styles;
    }

    public void Dispose()
    {

    }
}
