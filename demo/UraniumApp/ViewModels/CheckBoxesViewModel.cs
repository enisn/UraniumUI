using DotNurse.Injector.Attributes;
using InputKit.Shared;
using InputKit.Shared.Controls;
using InputKit.Shared.Helpers;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Graphics.Text;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Linq;
using UraniumApp.Inputs.ColorPicking;
using UraniumApp.Inputs.GeometryPicking;
using UraniumUI.Extensions;
using UraniumUI.Pages;
using UraniumUI.Resources;
using static InputKit.Shared.Controls.CheckBox;

namespace UraniumApp.ViewModels;

[RegisterAs(typeof(CheckBoxesViewModel))]
public class CheckBoxesViewModel : ReactiveObject
{
    protected IColorPicker ColorPicker { get; }
    protected IGeometryPicker GeometryPicker { get; }

    public string XamlSourceCode => SourceCode.ToString();

    protected XDocument SourceCode { get; }

    [Reactive] public string Text { get; set; } = "Hello, World!";

    [Reactive] public bool IsChecked { get; set; }

    [Reactive] public CheckType Type { get; set; } = CheckType.Material;

    [Reactive] public LabelPosition LabelPosition { get; set; }

    [Reactive] public Color Color { get; set; } = ColorResource.GetColor("Primary", "PrimaryDark");
    [Reactive] public Color BorderColor { get; set; } = ColorResource.GetColor("Outline", "OutlineDark");
    [Reactive] public Color TextColor { get; set; } = ColorResource.GetColor("OnBackground", "OnBackgroundDark");
    [Reactive] public Color IconColor { get; set; } = Colors.Transparent;
    [Reactive] public string IconPath { get; set; } = PredefinedShapes.Paths.Check;
    public Geometry IconGeometry => GeometryConverter.FromPath(IconPath);

    public LabelPosition[] LabelPositionList { get; } = new[]
    {
        LabelPosition.After,
        LabelPosition.Before,
    };

    public CheckType[] TypeList { get; } = new[]
    {
        CheckType.Material,
        CheckType.Filled,
        CheckType.Regular
    };

    public ICommand OpenDocumentationCommand { get; }
    public ICommand PickColorCommand { get; }
    public ICommand PickBorderColorCommand { get; }
    public ICommand PickTextColorCommand { get; }
    public ICommand PickIconColorCommand { get; }
    public ICommand PickIconGeometryCommand { get; }

    public CheckBoxesViewModel(IColorPicker colorPicker, IGeometryPicker geometryPicker)
    {
        ColorPicker = colorPicker;

        SourceCode = XDocument.Parse($"""<ContentPage xmlns:material="http://schemas.enisn-projects.io/dotnet/maui/uraniumui/material">  <material:CheckBox Text="{Text}"/> </ContentPage>""");

        OpenDocumentationCommand = new Command(
            async () => await Browser.Default.OpenAsync("https://enisn-projects.io/docs/en/uranium/latest/themes/material/CheckBox"));

        PickColorCommand = new Command(async () =>
            await ColorPicker.PickCollorForAsync(this, nameof(Color)));

        PickColorCommand = new Command(async () =>
            await ColorPicker.PickCollorForAsync(this, nameof(Color)));

        PickBorderColorCommand = new Command(async () =>
            await ColorPicker.PickCollorForAsync(this, nameof(BorderColor)));

        PickTextColorCommand = new Command(async () =>
            await ColorPicker.PickCollorForAsync(this, nameof(TextColor)));

        PickIconColorCommand = new Command(async () =>
            await ColorPicker.PickCollorForAsync(this, nameof(IconColor)));

        PickIconGeometryCommand = new Command(async () =>
        {
            var newGeometry = await GeometryPicker.PickGeometryForAsync();
            if (newGeometry != null)
            {
                IconPath = newGeometry;
            }
        });

        this.WhenAnyValue(x => x.Text).Subscribe(GenerateSourceCode);
        this.WhenAnyValue(x => x.IsChecked).Subscribe(_ => GenerateSourceCode());
        this.WhenAnyValue(x => x.Type).Subscribe(_ => GenerateSourceCode());
        this.WhenAnyValue(x => x.LabelPosition).Subscribe(_ => GenerateSourceCode());
        this.WhenAnyValue(x => x.Color).Subscribe(_ => GenerateSourceCode());
        this.WhenAnyValue(x => x.BorderColor).Subscribe(_ => GenerateSourceCode());
        this.WhenAnyValue(x => x.TextColor).Subscribe(_ => GenerateSourceCode());
        this.WhenAnyValue(x => x.IconColor).Subscribe(_ => GenerateSourceCode());
        this.WhenAnyValue(x => x.IconPath).Subscribe(_ => 
        {
            GenerateSourceCode();
            this.RaisePropertyChanged(nameof(IconGeometry));
        });

        GeometryPicker = geometryPicker;
    }

    protected void GenerateSourceCode(object parameter = null)
    {
        var contentPage = SourceCode.Descendants().First();

        var material = contentPage.GetNamespaceOfPrefix("material");

        var checkBox = contentPage.Descendants().First();

        checkBox.SetAttributeValue(nameof(Text), Text);

        checkBox.SetAttributeValue(nameof(IsChecked), !IsChecked
            ? null : IsChecked.ToString());

        checkBox.SetAttributeValue(nameof(Type),
            Type == CheckType.Material
            ? null : Type.ToString());

        checkBox.SetAttributeValue(nameof(LabelPosition), LabelPosition == LabelPosition.After ? null : LabelPosition.ToString());

        checkBox.SetAttributeValue(nameof(Color), Color == ColorResource.GetColor("Primary", "PrimaryDark")
            ? null : Color?.ToArgbHex());

        checkBox.SetAttributeValue(nameof(BorderColor), BorderColor == ColorResource.GetColor("Outline", "OutlineDark")
            ? null : BorderColor?.ToArgbHex());

        checkBox.SetAttributeValue(nameof(TextColor), TextColor == ColorResource.GetColor("OnBackground", "OnBackgroundDark")
            ? null : TextColor?.ToArgbHex());

        checkBox.SetAttributeValue(nameof(IconColor), IconColor.IsNullOrTransparent() ? null : IconColor?.ToArgbHex());

        checkBox.SetAttributeValue(nameof(IconPath), IconPath == PredefinedShapes.Paths.Check ? null : IconPath?.ToString());

        this.RaisePropertyChanged(nameof(XamlSourceCode));
    }
}
