using DotNurse.Injector.Attributes;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Windows.Input;
using System.Xml.Linq;
using UraniumApp.Inputs.ColorPicking;
using UraniumUI.Resources;

namespace UraniumApp.ViewModels.InputFields;

[RegisterAs(typeof(TextFieldViewModel))]
public class TextFieldViewModel : InputFieldViewModel
{
    public string XamlSourceCode => SourceCode.ToString();

    protected XDocument SourceCode { get; }

    [Reactive] public string Text { get; set; }

    [Reactive] public string Title { get; set; } = "Title";

    protected Color defaultTextColor = ColorResource.GetColor("OnBackground", "OnBackgroundDark", Colors.Gray);
    [Reactive] public Color TextColor { get; set; }

    public ICommand PickTextColorCommand { get; }

    public TextFieldViewModel(IColorPicker colorPicker) : base(colorPicker)
    {
        SourceCode = XDocument.Parse($"""<ContentPage xmlns:material="http://schemas.enisn-projects.io/dotnet/maui/uraniumui/material"><material:TextField/></ContentPage>""");

        TextColor = defaultTextColor;

        PickTextColorCommand = new Command(async _ =>
            await ColorPicker.PickCollorForAsync(this, nameof(TextColor)));

        this.WhenAnyValue(x => x.Text).Subscribe(GenerateSourceCode);
        this.WhenAnyValue(x => x.Title).Subscribe(GenerateSourceCode);
        this.WhenAnyValue(x => x.TextColor).Subscribe(GenerateSourceCode);
        this.WhenAnyValue(x => x.AccentColor).Subscribe(GenerateSourceCode);

        GenerateSourceCode();
    }

    protected void GenerateSourceCode(object parameter = null)
    {
        var contentPage = SourceCode.Descendants().First();

        var material = contentPage.GetNamespaceOfPrefix("material");

        var textField = contentPage.Descendants(material + "TextField").First();

        textField.SetAttributeValue(nameof(Text), Text);
        textField.SetAttributeValue(nameof(Title), Title);

        textField.SetAttributeValue(nameof(TextColor), TextColor == defaultTextColor ? null : TextColor?.ToArgbHex());
        textField.SetAttributeValue(nameof(AccentColor), AccentColor == defaultAccentColor ? null : AccentColor?.ToArgbHex());

        this.RaisePropertyChanged(nameof(XamlSourceCode));
    }
}
