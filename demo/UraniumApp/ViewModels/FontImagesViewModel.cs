using DotNurse.Injector.Attributes;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UraniumUI.Icons.FontAwesome;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UraniumUI.Icons.MaterialIcons;
using UraniumUI.Icons.SegoeFluent;
using System.Reactive.Linq;
using System.Xml.Linq;
using DynamicData.Binding;

namespace UraniumApp.ViewModels;

[RegisterAs(typeof(FontImagesViewModel))]
public class FontImagesViewModel : ReactiveObject
{
    public FontImageViewModel[] Items { get; }
    public FontImagesViewModel()
    {
        Items = new FontImageViewModel[]
        {
            new FontImageViewModel("FontAwesome")
            {
                FontFamilyPrefix = "FA",
                Types = new[]
                {
                    typeof(Solid),
                    typeof(Regular)
                },
                SelectedType = typeof(Solid),
            },
            new FontImageViewModel("MaterialIcons")
            {
                Types = new[]
                {
                    typeof(MaterialRegular),
                    typeof(MaterialOutlined),
                    typeof(MaterialRound),
                    typeof(MaterialSharp),
                    typeof(MaterialTwoTone),
                },
                SelectedType = typeof(MaterialRegular),
            },
            new FontImageViewModel("Fluent")
            {
                Types = new[]
                {
                    typeof(Fluent),
                },
                SelectedType = typeof(Fluent),
            }
        };
    }
}

public class FontImageViewModel : ReactiveObject
{

    public FontImageViewModel(string name)
    {
        this.Name = name;
        this.WhenAnyValue(x => x.SelectedType)
            .Subscribe(LoadIconsForSelectedType);
        SourceCode = XDocument.Parse("""<ContentPage xmlns:uranium="http://schemas.enisn-projects.io/dotnet/maui/uraniumui"><Image /></ContentPage>""");

        this.WhenAnyPropertyChanged(nameof(SelectedType), nameof(SelectedIcon))
            .Subscribe(GenerateSourceCode);
    }
    public string XamlSourceCode => SourceCode.ToString();
    protected XDocument SourceCode { get; }

    public Type[] Types { get; set; }

    [Reactive] public string Name { get; set; }

    [Reactive] public string SearchText { get; set; }

    private IEnumerable<FontImageItemViewModel> icons;
    public IEnumerable<FontImageItemViewModel> Icons
    {
        get
        {
            if (string.IsNullOrEmpty(SearchText))
            {
                return icons;
            }

            return icons.Where(x => x.Alias.Contains(SearchText, StringComparison.InvariantCultureIgnoreCase));
        }

        protected set => this.RaiseAndSetIfChanged(ref icons, value);
    }

    [Reactive] public FontImageItemViewModel SelectedIcon { get; set; }

    [Reactive] public Type SelectedType { get; set; }

    public string FontFamilyPrefix { get; set; }

    private void LoadIconsForSelectedType(Type type)
    {
        if (type == null)
        {
            Icons = null;
            return;
        }

        Icons = type.GetFields(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public)
            .Select(s => new FontImageItemViewModel
            {
                Glyph = s.GetValue(null) as string,
                Alias = s.Name,
                FontFamily = FontFamilyPrefix + type.Name
            });

        SelectedIcon = Icons.FirstOrDefault();

        this.WhenAnyValue(x => x.SearchText)
            .Throttle(TimeSpan.FromMilliseconds(250))
            .Subscribe(_ => this.RaisePropertyChanged(nameof(Icons)));
    }

    protected virtual void GenerateSourceCode(object parameter = null)
    { 
        var contentPage = SourceCode.Descendants().First();
        var uranium = contentPage.GetNamespaceOfPrefix("uranium");
        var image = contentPage.Descendants("Image").First();

        var sourceValue = SelectedIcon is null ? null :
            $"{{FontImageSource Glyph={{x:Static uranium:{SelectedType.Name}.{SelectedIcon.Alias}}}, FontFamily={SelectedIcon.FontFamily}}}";

        image.SetAttributeValue("Source", sourceValue);

        this.RaisePropertyChanged(nameof(XamlSourceCode));
    }

    public override string ToString() => Name;
}

public class FontImageItemViewModel
{
    public string Glyph { get; set; }
    public string Alias { get; set; }
    public string FontFamily { get; set; }
}