using DotNurse.Injector.Attributes;
using UraniumUI.Material.Controls;
using UraniumUI.Resources;

namespace UraniumApp.ViewModels.InputFields;

[RegisterAs(typeof(DatePickerFieldViewModel))]
public class DatePickerFieldViewModel : SingleControlEditingViewModel<DatePickerField>
{
    protected override DatePickerField InitializeControl()
    {
        return new DatePickerField
        {
            Title = "Pick a date"
        };
    }

    protected override void InitializeDefaultValues(Dictionary<string, object> values)
    {
        values.Add(nameof(TextField.TextColor), ColorResource.GetColor("OnBackground", "OnBackgroundDark"));
        values.Add(nameof(TextField.AccentColor), ColorResource.GetColor("Primary", "PrimaryDark"));
        values.Add(nameof(TextField.BorderColor), ColorResource.GetColor("OnBackground", "OnBackgroundDark"));
        values.Add(nameof(TextField.TitleColor), ColorResource.GetColor("OnBackground", "OnBackgroundDark"));
    }
}