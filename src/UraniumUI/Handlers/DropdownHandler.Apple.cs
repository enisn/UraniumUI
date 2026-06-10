#if IOS || MACCATALYST
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using System.Collections.Specialized;
using UIKit;
using UraniumUI.Controls;
using UraniumUI.Extensions;
using UraniumUI.Resources;

namespace UraniumUI.Handlers;

public partial class DropdownHandler : ButtonHandler
{
    public DropdownHandler(IPropertyMapper mapper, CommandMapper commandMapper = null) : base(DropdownPropertyMapper, commandMapper)
    {

    }

    protected override UIKit.UIButton CreatePlatformView()
    {
        var button = base.CreatePlatformView();

        SetItemsSource(VirtualViewDropdown, button);

        button.ShowsMenuAsPrimaryAction = true;
        button.TintColor = ColorResource.GetColor("Primary", "PrimaryDark", Colors.Azure).ToPlatform();
        if (UIDevice.CurrentDevice.CheckSystemVersion(15, 0))
        {
            var configuration = UIButtonConfiguration.PlainButtonConfiguration;
            button.Configuration = configuration;
        }


        return button;
    }

    protected override void ConnectHandler(UIButton platformView)
    {
        base.ConnectHandler(platformView);
        ArrangeText();
    }

    private static void SetItemsSource(Dropdown dropdown, UIKit.UIButton button)
    {
        // Always reconstruct the menu so that it is also cleared when ItemsSource becomes null.
        ReconstructMenu(dropdown, button);

        if (dropdown.ItemsSource is not null)
        {
            dropdown.ItemsSourceCollectionChangedCallback = (e) =>
            {
                ReconstructMenu(dropdown, button);
            };
        }
        else
        {
            // Clear any previous callback when there is no ItemsSource.
            dropdown.ItemsSourceCollectionChangedCallback = null;
        }
    }

    private static void ReconstructMenu(Dropdown dropdown, UIKit.UIButton button)
    {
        if (dropdown.ItemsSource is null)
        {
            button.Menu = null;
            return;
        }

        var items = new UIKit.UIMenuElement[dropdown.ItemsSource.Count];
        var selectedIndex = dropdown.ItemsSource.IndexOf(dropdown.SelectedItem);
        for (int i = 0; i < dropdown.ItemsSource.Count; i++)
        {
            var item = dropdown.ItemsSource[i];
            var action = UIKit.UIAction.Create(GetTextForItem(dropdown, item), null, null, _ => { dropdown.SelectedItem = item; });
            action.State = i == selectedIndex ? UIMenuElementState.On : UIMenuElementState.Off;
            items[i] = action;
        }
        button.Menu = UIKit.UIMenu.Create(items);
    }

    public static void MapItemsSource(DropdownHandler handler, Dropdown dropdown)
    {
        SetItemsSource(dropdown, handler.PlatformView);
        handler.ArrangeText();
    }

    public static void MapSelectedItem(DropdownHandler handler, Dropdown dropdown)
    {
       handler.ArrangeText();
    }

    internal void ArrangeText()
    {
        var selectedIndex = VirtualViewDropdown.ItemsSource?.IndexOf(VirtualViewDropdown.SelectedItem) ?? -1;

        if (UIDevice.CurrentDevice.CheckSystemVersion(15, 0))
        {
            PlatformView.ChangesSelectionAsPrimaryAction = selectedIndex != -1;
        }

        if (PlatformView.Menu is not null)
        {
            for (int i = 0; i < PlatformView.Menu.Children.Length; i++)
            {
                var menuItem = PlatformView.Menu.Children[i];

                if (menuItem is UIAction action)
                {
                    action.State = i == selectedIndex ? UIMenuElementState.On : UIMenuElementState.Off;
                }
            }
        }

        if (VirtualViewDropdown.SelectedItem is null)
        {
            VirtualViewDropdown.Text = VirtualViewDropdown.Placeholder;
            PlatformView.SetTitleColor(VirtualViewDropdown.PlaceholderColor.ToPlatform(), UIControlState.Normal);
        }
        else
        {
            VirtualViewDropdown.Text = GetTextForItem(VirtualViewDropdown, VirtualViewDropdown.SelectedItem);
            PlatformView.SetTitleColor(VirtualViewDropdown.TextColor?.ToPlatform() ?? Colors.Black.ToPlatform(), UIControlState.Normal);
        }

        PlatformView.TintColor = ColorResource.GetColor("Primary", "PrimaryDark", Colors.Azure).ToPlatform();
    }

    public static void MapPlaceholder(DropdownHandler handler, Dropdown dropdown)
    {
        if (dropdown.SelectedItem is null)
        {
            handler.PlatformView.SetTitle(dropdown.Placeholder, UIControlState.Normal);
        }
    }

    public static void MapPlaceholderColor(DropdownHandler handler, Dropdown dropdown)
    {
        if (dropdown.SelectedItem is null)
        {
            handler.PlatformView.SetTitleColor(dropdown.PlaceholderColor.ToPlatform(), UIControlState.Normal);
        }
    }

    public static void MapHorizontalTextAlignment(DropdownHandler handler, Dropdown dropdown)
    {
        handler.PlatformView.HorizontalAlignment = dropdown.HorizontalTextAlignment switch
        {
            TextAlignment.Start => UIControlContentHorizontalAlignment.Left,
            TextAlignment.Center => UIControlContentHorizontalAlignment.Center,
            TextAlignment.End => UIControlContentHorizontalAlignment.Right,
            _ => UIControlContentHorizontalAlignment.Left
        };
    }

    public static void MapTextColor(DropdownHandler handler, Dropdown dropdown)
    {
        handler.ArrangeText();
    }

    public static void MapItemDisplayBinding(DropdownHandler handler, Dropdown dropdown)
    {
        ReconstructMenu(dropdown, handler.PlatformView);
        handler.ArrangeText();
    }

    private static string GetTextForItem(Dropdown dropdown, object item)
    {
        if (dropdown.ItemDisplayBinding is not null)
        {
            return dropdown.ItemDisplayBinding.GetValueOnce<string>(item);
        }
        return item?.ToString();
    }
}
#endif
