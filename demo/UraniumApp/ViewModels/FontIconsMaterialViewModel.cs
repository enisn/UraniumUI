using DotNurse.Injector.Attributes;
using System.Reflection;
using UraniumUI;
using UraniumUI.Icons.MaterialIcons;

namespace UraniumApp.ViewModels;

[RegisterAs(typeof(FontIconsMaterialViewModel), ServiceLifetime.Singleton)]
public class FontIconsMaterialViewModel : UraniumBindableObject
{
    private IEnumerable<FontImageItemViewModel> items;
    public IEnumerable<FontImageItemViewModel> Items
    {
        get
        {
            if (string.IsNullOrEmpty(SearchText))
            {
                return items;
            }

            return items.Where(x => x.Alias.Contains(SearchText, StringComparison.InvariantCultureIgnoreCase));
        }
        set => SetProperty(ref items, value);
    }

    private FontImageItemViewModel selectedItem;

    public FontImageItemViewModel SelectedItem { get => selectedItem; set => SetProperty(ref selectedItem, value); }

    public List<FontImageTypeViewModel> Types { get; } = new List<FontImageTypeViewModel>
    {
        new FontImageTypeViewModel{ Display = "MaterialRegular", Type = typeof(MaterialRegular) },
        new FontImageTypeViewModel{ Display = "MaterialOutlined", Type = typeof(MaterialRound) },
        new FontImageTypeViewModel{ Display = "MaterialRound", Type = typeof(MaterialOutlined) },
        new FontImageTypeViewModel{ Display = "MaterialSharp", Type = typeof(MaterialSharp) },
        new FontImageTypeViewModel{ Display = "MaterialTwoTone", Type = typeof(MaterialTwoTone) },
    };

    private FontImageTypeViewModel selectedType;
    public FontImageTypeViewModel SelectedType { get => selectedType; set => SetProperty(ref selectedType, value, doAfter: OnSelectedTypeChanged); }

    private string searchText;
    public string SearchText { get => searchText; set => SetProperty(ref searchText, value, doAfter: (_) => OnPropertyChanged(nameof(Items))); }

    public FontIconsMaterialViewModel()
    {
        SelectedType = Types.First();
    }

    private void OnSelectedTypeChanged(FontImageTypeViewModel obj)
    {
        Items = SelectedType.Type.GetFields(BindingFlags.Static | BindingFlags.Public)
            .Select(s => new FontImageItemViewModel
            {
                Glyph = s.GetValue(null) as string,
                Alias = s.Name
            }).ToList();
        SelectedItem = Items.First();
    }
}

public class FontImageItemViewModel
{
    public string Glyph { get; set; }
    public string Alias { get; set; }
}

public class FontImageTypeViewModel
{
    public string Display { get; set; }
    public Type Type { get; set; }

    public override string ToString() => Display;
}
