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
using DynamicData;

namespace UraniumApp.ViewModels;

[RegisterAs(typeof(FontImagesViewModel))]
public class FontImagesViewModel : ReactiveObject
{
    public FontImageViewModel[] Items { get; }
    public FontImagesViewModel()
    {
        Items = new FontImageViewModel[]
        {
            new FontImageViewModel("FontAwesome", new[]
                {
                    typeof(Solid),
                    typeof(Regular)
                })
            {
                FontFamilyPrefix = "FA",
            },
            new FontImageViewModel("MaterialIcons", new[]
                {
                    typeof(MaterialRegular),
                    typeof(MaterialOutlined),
                    typeof(MaterialRound),
                    typeof(MaterialSharp),
                    typeof(MaterialTwoTone),
                }),
            new FontImageViewModel("Fluent", typeof(Fluent))
        };
    }
}

public class FontImageViewModel : ReactiveObject
{
    public FontImageViewModel(string name, params Type[] types)
    {
        this.Name = name;
        this.Types = types;

        foreach (var type in types)
        {
            var icons = type.GetFields(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public)
            .Select(s => new FontImageItemViewModel
            {
                DeclaredType = type,
                Glyph = s.GetValue(null) as string,
                Alias = s.Name,
                FontFamily = FontFamilyPrefix + type.Name
            });

            IconsSourceList.AddRange(icons);
        }
        var pageRequest = this
            .WhenAnyValue(vm => vm.PageNumber)
            .Select(number => new PageRequest(1, number * 10));

        IconsSourceList.Connect()
            .Filter(this.WhenAnyValue(vm => vm.SelectedType)
                .Select(MakeTypeFilter))
            .Filter(
                this.WhenAnyValue(vm => vm.SearchText)
                .Throttle(TimeSpan.FromMilliseconds(500))
                .Select(MakeSearchFilter))
            .Page(pageRequest)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out icons)
            .Subscribe();

        SourceCode = XDocument.Parse("""<ContentPage xmlns:uranium="http://schemas.enisn-projects.io/dotnet/maui/uraniumui"><Image /></ContentPage>""");

        this.WhenAnyPropertyChanged(nameof(SelectedType), nameof(SelectedIcon))
            .Subscribe(GenerateSourceCode);

        this.SelectedType = Types.FirstOrDefault();
    }
    public string XamlSourceCode => SourceCode.ToString();
    protected XDocument SourceCode { get; }

    public Type[] Types { get; }

    [Reactive] public string Name { get; set; }

    [Reactive] public string SearchText { get; set; }

    public SourceList<FontImageItemViewModel> IconsSourceList { get; } = new();

    private ReadOnlyObservableCollection<FontImageItemViewModel> icons;
    public ReadOnlyObservableCollection<FontImageItemViewModel> Icons => icons;

    [Reactive] public FontImageItemViewModel SelectedIcon { get; set; }

    [Reactive] public Type SelectedType { get; set; }

    [Reactive] public int PageNumber { get; set; } = 1;

    public string FontFamilyPrefix { get; set; }

    private Func<FontImageItemViewModel, bool> MakeSearchFilter(string term)
    {
        return x => string.IsNullOrEmpty(term) || x.Alias.Contains(term, StringComparison.InvariantCultureIgnoreCase);
    }

    private Func<FontImageItemViewModel, bool> MakeTypeFilter(Type type)
    {
        return x => x.DeclaredType == type;
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
    public Type DeclaredType { get; set; }
    public string Glyph { get; set; }
    public string Alias { get; set; }
    public string FontFamily { get; set; }
}