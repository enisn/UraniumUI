using DotNurse.Injector.Attributes;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Linq;
using UraniumApp.Inputs.ColorPicking;
using UraniumUI.Resources;

namespace UraniumApp.ViewModels;

[RegisterAs(typeof(ChipsViewModel))]
public class ChipsViewModel : ReactiveObject
{
    protected IColorPicker ColorPicker { get; }

    public string XamlSourceCode => SourceCode.ToString();

    protected XDocument SourceCode { get; }

    [Reactive] public Color BackgroundColor { get; set; }

    [Reactive] public Color TextColor { get; set; }

    public ICommand PickBackgroundColorCommand { get; }

    public ICommand PickTextColorCommand { get; }

    [Reactive] public string Text { get; set; } = "Hello, World!";

    public ChipsViewModel(IColorPicker colorPicker)
    {
        ColorPicker = colorPicker;
        BackgroundColor = ColorResource.GetColor("Primary", "PrimaryDark", Colors.Purple);
        TextColor = ColorResource.GetColor("OnPrimary", "OnPrimaryDark", Colors.White);

        PickBackgroundColorCommand = new Command(
                       async () => await ColorPicker.PickCollorForAsync(this, nameof(BackgroundColor)));

        PickTextColorCommand = new Command(
                                  async () => await ColorPicker.PickCollorForAsync(this, nameof(TextColor)));
        SourceCode = XDocument.Parse($"""<ContentPage xmlns:material="http://schemas.enisn-projects.io/dotnet/maui/uraniumui/material">  <material:Chip Text="{Text}"/> </ContentPage>""");

        this.WhenAnyValue(x => x.Text)
            .Subscribe(_ => GenerateSourceCode());
        
        this.WhenAnyValue(x => x.BackgroundColor)
            .Subscribe(_ => GenerateSourceCode());

        this.WhenAnyValue(x => x.TextColor)
            .Subscribe(_ => GenerateSourceCode());
    }

    protected void GenerateSourceCode()
    {
        var contentPage = SourceCode.Descendants().First();

        var material = contentPage.GetNamespaceOfPrefix("material");

        var chip = contentPage.Descendants().First();

        chip.SetAttributeValue(nameof(Text), Text);

        chip.SetAttributeValue(nameof(BackgroundColor), BackgroundColor == ColorResource.GetColor("Primary", "PrimaryDark", Colors.Purple)
            ? null : BackgroundColor.ToArgbHex());

        chip.SetAttributeValue(nameof(TextColor),
            TextColor == ColorResource.GetColor("OnPrimary", "OnPrimaryDark", Colors.White)
            ? null
            : TextColor?.ToArgbHex());

        this.RaisePropertyChanged(nameof(XamlSourceCode));
    }
}
