#if WINDOWS
using Microsoft.Maui.Animations;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using Microsoft.UI.Xaml.Controls;
using System.ComponentModel;
using UraniumUI.Controls;
using UraniumUI.Extensions;

namespace UraniumUI.Handlers;
public partial class DropdownHandler : ButtonHandler
{
    public DropdownHandler(IPropertyMapper mapper, CommandMapper commandMapper = null) : base(DropdownPropertyMapper, commandMapper)
    {

    }

    protected override Microsoft.UI.Xaml.Controls.DropDownButton CreatePlatformView()
    {
        var dropdownButton = new Microsoft.UI.Xaml.Controls.DropDownButton();
        dropdownButton.HorizontalAlignment = Microsoft.UI.Xaml.HorizontalAlignment.Stretch;
        dropdownButton.BorderThickness = new Microsoft.UI.Xaml.Thickness(0);
        dropdownButton.Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Transparent);
        dropdownButton.Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Transparent);

        SetItemSource(VirtualViewDropdown, dropdownButton);

        return dropdownButton;
    }

    protected override void ConnectHandler(Microsoft.UI.Xaml.Controls.Button platformView)
    {
        base.ConnectHandler(platformView);
        ArrangeText();
    }

    private static void SetItemSource(Dropdown dropdown, Microsoft.UI.Xaml.Controls.DropDownButton dropdownButton)
    {
        if (dropdown.ItemsSource is not null)
        {
            var flyout = new Microsoft.UI.Xaml.Controls.MenuFlyout();
            flyout.Placement = GetPlacement(dropdown.HorizontalTextAlignment);
            flyout.Items.Clear();

            // TODO: For customization. (only possible on windows :( )
            //// Create a Style for the MenuFlyoutPresenter
            //var style = new Microsoft.UI.Xaml.Style(typeof(Microsoft.UI.Xaml.Controls.MenuFlyoutPresenter));
            //style.Setters.Add(new Microsoft.UI.Xaml.Setter(Microsoft.UI.Xaml.Controls.Control.BackgroundProperty, new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Red)));
            //flyout.MenuFlyoutPresenterStyle = style;

            foreach (var item in dropdown.ItemsSource)
            {
                var menuItem = new Microsoft.UI.Xaml.Controls.MenuFlyoutItem();

                SetFlyoutItemText(dropdown, menuItem, item);

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

    protected static Microsoft.UI.Xaml.Controls.Primitives.FlyoutPlacementMode GetPlacement(Microsoft.Maui.TextAlignment alignment)
    {
        return alignment switch
        {
            Microsoft.Maui.TextAlignment.Start => Microsoft.UI.Xaml.Controls.Primitives.FlyoutPlacementMode.BottomEdgeAlignedLeft,
            Microsoft.Maui.TextAlignment.Center => Microsoft.UI.Xaml.Controls.Primitives.FlyoutPlacementMode.Bottom,
            Microsoft.Maui.TextAlignment.End => Microsoft.UI.Xaml.Controls.Primitives.FlyoutPlacementMode.BottomEdgeAlignedRight,
            _ => Microsoft.UI.Xaml.Controls.Primitives.FlyoutPlacementMode.BottomEdgeAlignedLeft
        };
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

        handler.ArrangeText();
    }

    protected void ArrangeText()
    {
        var selectedIndex = VirtualViewDropdown.ItemsSource?.IndexOf(VirtualViewDropdown.SelectedItem) ?? -1;
        if (PlatformView.Flyout is Microsoft.UI.Xaml.Controls.MenuFlyout flyout)
        {
            for (int i = 0; i < flyout.Items.Count; i++)
            {
                var menuItem = flyout.Items[i];
                if (menuItem is Microsoft.UI.Xaml.Controls.MenuFlyoutItem menuFlyoutItem)
                {
                    menuFlyoutItem.Icon = i == selectedIndex ? new FontIcon() { Glyph="\uE73E" } : null;
                }
            }
        }

        if (VirtualViewDropdown.SelectedItem is null)
        {
            PlatformView.Content = VirtualViewDropdown.Placeholder;
            PlatformView.Foreground = VirtualViewDropdown.PlaceholderColor.ToPlatform();
        }
        else
        {
            PlatformView.Content = VirtualViewDropdown.SelectedItem.ToString();
            PlatformView.Foreground = VirtualViewDropdown.TextColor?.ToPlatform() ?? Colors.Black.ToPlatform();
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

        if (dropdownButton.Flyout is Microsoft.UI.Xaml.Controls.MenuFlyout flyout)
        {
            flyout.Placement = GetPlacement(dropdown.HorizontalTextAlignment);
        }
    }

    public static void MapTextColor(DropdownHandler handler, Dropdown dropdown)
    {
        handler.ArrangeText();
    }

    public static void MapItemDisplayBinding(DropdownHandler handler, Dropdown dropdown)
    {
        if (handler.PlatformView.Flyout is Microsoft.UI.Xaml.Controls.MenuFlyout flyout)
        {
            for (int i = 0; i < flyout.Items.Count; i++)
            {
                if (flyout.Items[i] is Microsoft.UI.Xaml.Controls.MenuFlyoutItem menuItem)
                {
                    SetFlyoutItemText(dropdown, menuItem, dropdown.ItemsSource[i]);
                }
            }
        }
    }

    private static void SetFlyoutItemText(Dropdown dropdown, Microsoft.UI.Xaml.Controls.MenuFlyoutItem menuItem, object item)
    {
        if (dropdown.ItemDisplayBinding != null)
        {
            menuItem.Text = dropdown.ItemDisplayBinding.GetValueOnce<string>(item);
        }
        else
        {
            menuItem.Text = item.ToString();
        }
    }
}
#endif