using DotNurse.Injector.Attributes;
using DynamicData.Binding;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Windows.Input;
using System.Xml.Linq;
using UraniumApp.Inputs.ColorPicking;
using UraniumUI.Blurs;

namespace UraniumApp.ViewModels.Blurs;

[RegisterAs(typeof(BlursPreviewViewModel))]
public class BlursPreviewViewModel : ReactiveObject
{
    [Reactive] public string Title { get; set; }

    [Reactive] public BlurMode BlurMode { get; set; }

    [Reactive] public Color AccentColor { get; set; }

    [Reactive] public float AccentOpacity { get; set; } = .4f;

    public string XamlSourceCode => SourceCode.ToString();
    protected XDocument SourceCode { get; }

    public string[] BackgroundImageSourceList => new[]
    {
        "https://images.unsplash.com/photo-1470058869958-2a77ade41c02?ixlib=rb-4.0.3&ixid=MnwxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8&auto=format&fit=crop&w=1170&q=80",
        "https://images.unsplash.com/photo-1482686115713-0fbcaced6e28?ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D&auto=format&fit=crop&w=1467&q=80",
        "https://images.unsplash.com/photo-1479030160180-b1860951d696?ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D&auto=format&fit=crop&w=1470&q=80",
        "https://images.unsplash.com/photo-1421930866250-aa0594cea05c?ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D&auto=format&fit=crop&w=1476&q=80",
    };

    [Reactive] public string BackgroundImageSource { get; set; }

    public BlurMode[] BlurModeList => new[]
    {
        BlurMode.Light, BlurMode.Dark,
    };
    public ICommand PickAccentColorCommand { get; set; }

    private readonly IColorPicker colorPicker;
    public BlursPreviewViewModel(IColorPicker colorPicker)
    {
        Title = "Blurs Preview";
        BackgroundImageSource = BackgroundImageSourceList[0];
        BlurMode = BlurModeList[0];

        SourceCode = XDocument.Parse($"""<ContentPage xmlns:uranium="http://schemas.enisn-projects.io/dotnet/maui/uraniumui"><VerticalStackLayout></VerticalStackLayout></ContentPage>""");
        this.colorPicker = colorPicker;

        PickAccentColorCommand = ReactiveCommand.CreateFromTask(() =>
            colorPicker.PickCollorForAsync(this, nameof(AccentColor)));

        this.WhenAnyPropertyChanged(nameof(BlurMode), nameof(AccentColor), nameof(AccentOpacity))
            .Subscribe(GenerateSourceCode);

        GenerateSourceCode();
    }

    protected virtual void GenerateSourceCode(object parameter = null)
    {
        var contentPage = SourceCode.Descendants().First();

        var uranium = contentPage.GetNamespaceOfPrefix("uranium");

        var verticalStackLayout = contentPage.Element("VerticalStackLayout");

        verticalStackLayout.RemoveNodes();

        var effects = new XElement("Effects");
        verticalStackLayout.Add(effects);

        var blurEffect = new XElement(uranium + "BlurEffect");

        effects.Add(blurEffect);

        blurEffect.SetAttributeValue(nameof(BlurMode), BlurMode.ToString());
        blurEffect.SetAttributeValue(nameof(AccentColor), AccentColor?.ToArgbHex());
        blurEffect.SetAttributeValue(nameof(AccentOpacity), AccentOpacity == 0.4f ? null : AccentOpacity);

        this.RaisePropertyChanged(nameof(XamlSourceCode));
    }
}
