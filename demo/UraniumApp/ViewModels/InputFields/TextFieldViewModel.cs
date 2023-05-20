using DotNurse.Injector.Attributes;
using UraniumApp.Inputs.ColorPicking;
using UraniumUI.Material.Controls;
using UraniumUI.Resources;

namespace UraniumApp.ViewModels.InputFields;

[RegisterAs(typeof(TextFieldViewModel))]
public class TextFieldViewModel : SingleControlEditingViewModel<TextField>
{
    protected override TextField InitializeControl()
    {
        return new TextField
        {
            Title = "Title"
        };
    }

    protected override string InitialXDocumentCode => """<ContentPage xmlns:material="http://schemas.enisn-projects.io/dotnet/maui/uraniumui/material"><material:TextField/></ContentPage>""";

    protected override void InitializeDefaultValues(Dictionary<string, object> values)
    {
        values.Add(nameof(TextField.TextColor), ColorResource.GetColor("OnBackground", "OnBackgroundDark"));
        values.Add(nameof(TextField.AccentColor), ColorResource.GetColor("Primary", "PrimaryDark"));
        values.Add(nameof(TextField.BorderColor), ColorResource.GetColor("OnBackground", "OnBackgroundDark"));
        values.Add(nameof(TextField.TitleColor), ColorResource.GetColor("OnBackground", "OnBackgroundDark"));
    }
}
