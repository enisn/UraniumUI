using UraniumUI.Controls;

namespace UraniumUI.Material.Controls;
public class DropdownField : InputField
{
    public Dropdown DropdownView => Content as Dropdown;

    public override View Content { get; set; } = new Dropdown
    {
        VerticalOptions = LayoutOptions.Center,
        Margin = new Thickness(15, 0),
        StyleClass = new List<string> { "InputField.Dropdown" }
    };

    public DropdownField()
    {
    }
}
