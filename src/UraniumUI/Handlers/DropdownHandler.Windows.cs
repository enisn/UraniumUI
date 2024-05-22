#if WINDOWS
using Microsoft.Maui.Handlers;
using UraniumUI.Controls;

namespace UraniumUI.Handlers;
public partial class DropdownHandler : ButtonHandler
{
    public DropdownHandler(IPropertyMapper mapper, CommandMapper commandMapper = null) : base(DropdownPropertyMapper, commandMapper)
    {

    }

    protected override Microsoft.UI.Xaml.Controls.DropDownButton CreatePlatformView()
    {
        var dropdownButton = new Microsoft.UI.Xaml.Controls.DropDownButton();

        SetItemSource(VirtualViewDropdown, dropdownButton);
        return dropdownButton;
    }

    private static void SetItemSource(Dropdown dropdown, Microsoft.UI.Xaml.Controls.DropDownButton dropdownButton)
    {
        if (dropdown.ItemsSource is not null)
        {
            var flyout = new Microsoft.UI.Xaml.Controls.MenuFlyout();
            flyout.Placement = Microsoft.UI.Xaml.Controls.Primitives.FlyoutPlacementMode.Bottom;
            flyout.Items.Clear();

            // TODO: For customization. (only possible on windows :( )
            //// Create a Style for the MenuFlyoutPresenter
            //var style = new Microsoft.UI.Xaml.Style(typeof(Microsoft.UI.Xaml.Controls.MenuFlyoutPresenter));
            //style.Setters.Add(new Microsoft.UI.Xaml.Setter(Microsoft.UI.Xaml.Controls.Control.BackgroundProperty, new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Red)));
            //flyout.MenuFlyoutPresenterStyle = style;

            foreach (var item in dropdown.ItemsSource)
            {
                var menuItem = new Microsoft.UI.Xaml.Controls.MenuFlyoutItem();
                menuItem.Text = item.ToString();
                menuItem.Command = new Command(() => dropdown.SelectedItem = item);
                flyout.Items.Add(menuItem);
            }
            dropdownButton.Flyout = flyout;
        }
    }

    public static void MapItemsSource(DropdownHandler handler, Dropdown dropdown)
    {
        SetItemSource(dropdown, handler.PlatformView as Microsoft.UI.Xaml.Controls.DropDownButton);
    }

    public static void MapSelectedItem(DropdownHandler handler, Dropdown dropdown)
    {
        if (handler.PlatformView is Microsoft.UI.Xaml.Controls.DropDownButton dropdownButton)
        {
            dropdownButton.Content = dropdown.SelectedItem?.ToString();
        }
    }
}
# endif