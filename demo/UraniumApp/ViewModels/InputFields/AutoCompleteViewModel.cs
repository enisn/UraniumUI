using DotNurse.Injector.Attributes;
using DynamicData;
using ReactiveUI;
using System.Collections;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Xml.Linq;
using UraniumUI.Dialogs;
using UraniumUI.Material.Controls;
using UraniumUI.Resources;

namespace UraniumApp.ViewModels.InputFields;

[RegisterAs(typeof(AutoCompleteViewModel))]
public class AutoCompleteViewModel : SingleControlEditingViewModel<AutoCompleteTextField>
{
    protected override AutoCompleteTextField InitializeControl()
    {
        return new AutoCompleteTextField
        {
            Title = "Fruit",
            ItemsSource = new ObservableCollection<string>
            {
                "Apple",
                "Banana",
                "Apricot"
            }
        };
    }

    protected override void InitializeDefaultValues(Dictionary<string, object> values)
    {
        values.Add(nameof(TextField.TextColor), ColorResource.GetColor("OnBackground", "OnBackgroundDark"));
        values.Add(nameof(TextField.AccentColor), ColorResource.GetColor("Primary", "PrimaryDark"));
        values.Add(nameof(TextField.BorderColor), ColorResource.GetColor("OnBackground", "OnBackgroundDark"));
        values.Add(nameof(TextField.TitleColor), ColorResource.GetColor("OnBackground", "OnBackgroundDark"));
    }

    protected override string InitialXDocumentCode => """<ContentPage xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml" xmlns:material="http://schemas.enisn-projects.io/dotnet/maui/uraniumui/material"><material:AutoCompleteTextField/></ContentPage>""";

    public ICommand AddItemCommand { get; }
    public ICommand RemoveItemCommand { get; }

    public AutoCompleteViewModel(IDialogService dialogService)
    {
        AddItemCommand = new Command(async () =>
        {
            var text = await dialogService.DisplayTextPromptAsync("New Item", "ItemsSource item text");
            if (!string.IsNullOrEmpty(text))
            {
                Control.ItemsSource.Add(text);
                GenerateSourceCode(text);
            }
        });

        RemoveItemCommand = new Command((item) =>
        {
            if (item is string text)
            {
                Control.ItemsSource.Remove(text);
                GenerateSourceCode(text);
            }
        });
    }

    protected override void PostGenerateSourceCode(XElement control)
    {
        var contentPage = SourceCode.Descendants().First();
        var material = contentPage.GetNamespaceOfPrefix("material");
        var x = contentPage.GetNamespaceOfPrefix("x");
        var autoCompleteTextField = contentPage.Descendants(material + "AutoCompleteTextField").First();

        var itemsSource = new XElement(material + "AutoCompleteTextField.ItemsSource");

        autoCompleteTextField.RemoveNodes();

        autoCompleteTextField.Add(itemsSource);

        foreach (var item in Control.ItemsSource)
        {
            itemsSource.Add(new XElement(x + "String", item));
        }
    }

    protected override string FormatValue(object value)
    {
        if (value is IList)
        {
            return null;
        }

        return base.FormatValue(value);
    }
}
