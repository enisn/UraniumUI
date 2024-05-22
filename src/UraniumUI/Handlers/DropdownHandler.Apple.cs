#if IOS || MACCATALYST
using Microsoft.Maui.Handlers;
using UIKit;
using UraniumUI.Controls;

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

        button.SetTitleColor(UIKit.UIColor.Label, UIControlState.Normal);

        button.ShowsMenuAsPrimaryAction = true;
        button.ChangesSelectionAsPrimaryAction = true;

        return button;
    }

    private static void SetItemsSource(Dropdown dropdown, UIKit.UIButton button)
    {
        if (dropdown.ItemsSource is not null)
        {
            var items = new UIKit.UIMenuElement[dropdown.ItemsSource.Count];
            var selectedIndex = dropdown.ItemsSource.IndexOf(dropdown.SelectedItem);
            for (int i = 0; i < dropdown.ItemsSource.Count; i++)
            {
                var item = dropdown.ItemsSource[i];
                var act = UIKit.UIAction.Create(dropdown.ItemsSource[i].ToString(), null, dropdown.ItemsSource[i].ToString(), _ => { dropdown.SelectedItem = item; });
                act.State = i == selectedIndex ? UIMenuElementState.On : UIMenuElementState.Off;
                items[i] = act;
            }
            button.Menu = UIKit.UIMenu.Create(items);
        }
    }

    public static void MapItemsSource(DropdownHandler handler, Dropdown dropdown)
    {
        SetItemsSource(dropdown, handler.PlatformView);
    }

    public static void MapSelectedItem(DropdownHandler handler, Dropdown dropdown)
    {
        var selectedIndex = dropdown.ItemsSource.IndexOf(dropdown.SelectedItem);

        for (int i = 0; i < handler.PlatformView.Menu.Children.Length; i++)
        {
            var menuItem = handler.PlatformView.Menu.Children[i];

            if (menuItem is UIAction action)
            {
                action.State = i == selectedIndex ? UIMenuElementState.On : UIMenuElementState.Off;
            }
        }

        handler.PlatformView.SetTitle(dropdown.SelectedItem?.ToString(), UIControlState.Normal);
    }
}
#endif