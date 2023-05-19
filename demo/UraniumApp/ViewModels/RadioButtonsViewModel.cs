using DotNurse.Injector.Attributes;
using UraniumUI.Resources;
using InputKit.Shared;
using InputKit.Shared.Helpers;
using Microsoft.Maui.Controls.Shapes;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Xml.Linq;
using UraniumApp.Inputs.ColorPicking;
using InputKit.Shared.Controls;
using UraniumApp.Inputs.GeometryPicking;

namespace UraniumApp.ViewModels;

[RegisterAs(typeof(RadioButtonsViewModel))]
public class RadioButtonsViewModel : ReactiveObject
{
    public IColorPicker ColorPicker { get; }

    public IGeometryPicker GeometryPicker { get; }

    [Reactive] public int SelectedIndex { get; set; } = -1;

    public int[] SelectedIndexList => RadioButtonItems.Select((s, i) => i).Prepend(-1).ToArray();

    public string XamlSourceCode => SourceCode.ToString();

    protected XDocument SourceCode { get; }

    public ObservableCollection<SingleRadioButtonViewModel> RadioButtonItems { get; } = new();

    public ICommand AddNewRadioButtonItemCommand { get; }

    public ICommand RemoveRadioButtonItemCommand { get; }

    public RadioButtonsViewModel(IColorPicker colorPicker, IGeometryPicker geometryPicker)
    {
        ColorPicker = colorPicker;
        GeometryPicker = geometryPicker;

        SourceCode = XDocument.Parse($"""<ContentPage xmlns:material="http://schemas.enisn-projects.io/dotnet/maui/uraniumui/material"><material:RadioButtonGroupView></material:RadioButtonGroupView></ContentPage>""");

        var generateSourceCodeCommand = new Command(GenerateSourceCode);
        RadioButtonItems.Add(new SingleRadioButtonViewModel(this, generateSourceCodeCommand) { Text = "Option 1" });
        RadioButtonItems.Add(new SingleRadioButtonViewModel(this, generateSourceCodeCommand) { Text = "Option 2" });

        AddNewRadioButtonItemCommand = new Command(() => RadioButtonItems.Add(
            new SingleRadioButtonViewModel(this, generateSourceCodeCommand) { Text = "New Option" }));

        RemoveRadioButtonItemCommand = new Command(item =>
            RadioButtonItems.Remove(item as SingleRadioButtonViewModel));

        RadioButtonItems.CollectionChanged += (s, e) => this.RaisePropertyChanged(nameof(SelectedIndexList));

        GenerateSourceCode();

        this.WhenAnyValue(x => x.SelectedIndex).Subscribe(_ => GenerateSourceCode());
    }

    protected void GenerateSourceCode(object parameter = null)
    {
        var contentPage = SourceCode.Descendants().First();

        var material = contentPage.GetNamespaceOfPrefix("material");

        var rbGroupView = contentPage.Descendants().First();

        rbGroupView.SetAttributeValue(nameof(SelectedIndex), SelectedIndex);

        rbGroupView.RemoveNodes();

        foreach (var item in RadioButtonItems)
        {
            var radioButton = new XElement(material + "RadioButton");

            item.SetXmlAttributes(radioButton);

            rbGroupView.Add(radioButton);
        }

        this.RaisePropertyChanged(nameof(XamlSourceCode));
    }
}

public class SingleRadioButtonViewModel : ReactiveObject
{
    protected ICommand ChangedCommand { get; }

    [Reactive] public string Text { get; set; }

    private Color defaultColor = ColorResource.GetColor("Primary", "PrimaryDark");
    [Reactive] public Color Color { get; set; }

    [Reactive] public bool IsChecked { get; set; }

    [Reactive] public LabelPosition LabelPosition { get; set; }

    public LabelPosition[] LabelPositionList => new[] { LabelPosition.After, LabelPosition.Before };

    [Reactive] public bool IsDisabled { get; set; }

    private string defaultSelectedIconPath = PredefinedShapes.Paths.Dot;
    [Reactive] public string SelectedIconPath { get; set; }

    public Geometry SelectedIconGeometry => GeometryConverter.FromPath(SelectedIconPath);

    public ICommand PickColorCommand { get; }

    public ICommand PickSelectedIconGeometryCommand { get; }

    public RadioButtonsViewModel Parent { get; }

    public SingleRadioButtonViewModel(RadioButtonsViewModel parent, ICommand changedCommand)
    {
        ChangedCommand = changedCommand;
        Color = defaultColor;
        SelectedIconPath = PredefinedShapes.Paths.Dot;

        PickColorCommand = new Command(() => Parent.ColorPicker.PickCollorForAsync(this, nameof(Color)));
        PickSelectedIconGeometryCommand = new Command(async () =>
        {
            var newPath = await Parent.GeometryPicker.PickGeometryForAsync();
            if (newPath != null)
            {
                SelectedIconPath = newPath;
            }
        });

        this.WhenAnyValue(x => x.Text).Subscribe(InvokeChangedCommand);
        this.WhenAnyValue(x => x.Color).Subscribe(InvokeChangedCommand);
        this.WhenAnyValue(x => x.IsChecked).Subscribe(InvokeChangedCommand);
        this.WhenAnyValue(x => x.LabelPosition).Subscribe(InvokeChangedCommand);
        this.WhenAnyValue(x => x.IsDisabled).Subscribe(InvokeChangedCommand);
        this.WhenAnyValue(x => x.SelectedIconPath).Subscribe(x =>
        {
            InvokeChangedCommand(x);
            this.RaisePropertyChanged(nameof(SelectedIconGeometry));
        });
        Parent = parent;
    }

    private void InvokeChangedCommand(object param) => ChangedCommand?.Execute(param = null);
    private void InvokeChangedCommand(bool param) => ChangedCommand?.Execute(param);
    private void InvokeChangedCommand(LabelPosition param) => ChangedCommand?.Execute(param);

    public void SetXmlAttributes(XElement radioButton)
    {
        radioButton.SetAttributeValue(nameof(Text), Text);
        radioButton.SetAttributeValue(nameof(Color), Color == defaultColor ? null : Color.ToArgbHex());
        radioButton.SetAttributeValue(nameof(IsChecked), !IsChecked ? null : IsChecked.ToString());
        radioButton.SetAttributeValue(nameof(LabelPosition), LabelPosition == LabelPosition.After ? null : LabelPosition);
        radioButton.SetAttributeValue(nameof(IsDisabled), !IsDisabled ? null : IsDisabled.ToString());
        radioButton.SetAttributeValue(nameof(SelectedIconGeometry), SelectedIconPath == defaultSelectedIconPath ? null : SelectedIconPath);
    }
}
