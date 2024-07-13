#if WINDOWS
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
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
        dropdownButton.BorderThickness = new Microsoft.UI.Xaml.Thickness(0);
        dropdownButton.Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Transparent);
        dropdownButton.Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Red);

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
                menuItem.CommandParameter = item;
                flyout.Items.Add(menuItem);
            }
            dropdownButton.Flyout = flyout;

            dropdown.ItemsSourceCollectionChangedCallback = (e) =>
            {
                switch (e.Action)
                {
                    case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                        {
                            foreach (var item in e.NewItems)
                            {
                                flyout.Items.Add(new Microsoft.UI.Xaml.Controls.MenuFlyoutItem() { Text = item.ToString(), Command = new Command(() => dropdown.SelectedItem = item) });
                            }
                        }
                       break;
                    case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                        {
                            foreach (var item in e.OldItems)
                            {
                                var itemToRemove = flyout.Items.FirstOrDefault(x => x is Microsoft.UI.Xaml.Controls.MenuFlyoutItem menuItem && menuItem.CommandParameter == item);
                                if (itemToRemove is not null)
                                {
                                    flyout.Items.Remove(itemToRemove);
                                }
                            }
                        }
                        break;
                }
            };
        }
    }

    public static void MapItemsSource(DropdownHandler handler, Dropdown dropdown)
    {
        SetItemSource(dropdown, handler.PlatformView as Microsoft.UI.Xaml.Controls.DropDownButton);
    }

    public static void MapSelectedItem(DropdownHandler handler, Dropdown dropdown)
    {
        if (handler.PlatformView is not Microsoft.UI.Xaml.Controls.DropDownButton dropdownButton)
        {
            return;
        }

        if (dropdown.SelectedItem is null)
        {
            dropdownButton.Content = dropdown.Placeholder;
            dropdownButton.Foreground = dropdown.PlaceholderColor.ToPlatform();
        }
        else
        {
            dropdownButton.Content = dropdown.SelectedItem.ToString();
            dropdownButton.Foreground = dropdown.TextColor?.ToPlatform() ?? Colors.Black.ToPlatform();
        }
    }

    public static void MapPlaceholder(DropdownHandler handler, Dropdown dropdown)
    {
        if (handler.PlatformView is not Microsoft.UI.Xaml.Controls.DropDownButton dropdownButton)
        {
            return;
        }

        if (dropdown.SelectedItem is null)
        {
            dropdownButton.Content = dropdown.Placeholder;
        }
    }

    public static void MapPlaceholderColor(DropdownHandler handler, Dropdown dropdown)
    {
        if (handler.PlatformView is not Microsoft.UI.Xaml.Controls.DropDownButton dropdownButton)
        {
            return;
        }

        if (dropdown.SelectedItem is null)
        {
            dropdownButton.Foreground = dropdown.PlaceholderColor.ToPlatform();
        }
    }

    public static void MapHorizontalTextAlignment(DropdownHandler handler, Dropdown dropdown)
    {
        if (handler.PlatformView is not Microsoft.UI.Xaml.Controls.DropDownButton dropdownButton)
        {
            return;
        }

        dropdownButton.HorizontalContentAlignment = dropdown.HorizontalTextAlignment.ToPlatformHorizontalAlignment();
    }
}
#endif