using DotNurse.Injector.Attributes;
using UraniumUI.Material.Controls;
using UraniumUI.Resources;

namespace UraniumApp.ViewModels.InputFields;

[RegisterAs(typeof(TimePickerFieldViewModel))]
public class TimePickerFieldViewModel : SingleControlEditingViewModel<TimePickerField>
{
    protected override TimePickerField InitializeControl()
    {
        return new TimePickerField
        {
            Title = "Pick a time"
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
