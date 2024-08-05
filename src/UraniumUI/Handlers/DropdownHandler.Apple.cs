#if IOS || MACCATALYST
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using System.Collections.Specialized;
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

        button.ShowsMenuAsPrimaryAction = true;
        if (UIDevice.CurrentDevice.CheckSystemVersion(15, 0))
        {
            button.ChangesSelectionAsPrimaryAction = true;
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
        if (dropdown.ItemsSource is not null)
        {
            var items = new UIKit.UIMenuElement[dropdown.ItemsSource.Count];
            var selectedIndex = dropdown.ItemsSource.IndexOf(dropdown.SelectedItem);
            for (int i = 0; i < dropdown.ItemsSource.Count; i++)
            {
                var item = dropdown.ItemsSource[i];
                var action = UIKit.UIAction.Create(dropdown.ItemsSource[i].ToString(), null, dropdown.ItemsSource[i].ToString(), _ => { dropdown.SelectedItem = item; });
                action.State = i == selectedIndex ? UIMenuElementState.On : UIMenuElementState.Off;
                items[i] = action;
            }
            button.Menu = UIKit.UIMenu.Create(items);

            dropdown.ItemsSourceCollectionChangedCallback = (e) =>
            {
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        {
                            Array.Resize(ref items, items.Length + e.NewItems.Count);
                            for (int i = 0; i < e.NewItems.Count; i++)
                            {
                                var item = e.NewItems[i];
                                var act = UIKit.UIAction.Create(e.NewItems[i].ToString(), null, e.NewItems[i].ToString(), _ => { dropdown.SelectedItem = item; });
                                items[items.Length - e.NewItems.Count + i] = act;
                            }
                        }
                        break;
                    case NotifyCollectionChangedAction.Remove:
                        {
                            for (int i = 0; i < e.OldItems.Count; i++)
                            {
                                items = items.Where(x => x.Title != e.OldItems[i].ToString()).ToArray();
                            }
                            button.Menu = UIKit.UIMenu.Create(items);
                        }
                        break;
                }
            };
        }
    }

    public static void MapItemsSource(DropdownHandler handler, Dropdown dropdown)
    {
        SetItemsSource(dropdown, handler.PlatformView);
    }

    public static void MapSelectedItem(DropdownHandler handler, Dropdown dropdown)
    {
       handler.ArrangeText();
    }

    internal void ArrangeText()
    {
        var selectedIndex = VirtualViewDropdown.ItemsSource?.IndexOf(VirtualViewDropdown.SelectedItem) ?? -1;

        for (int i = 0; i < PlatformView.Menu.Children.Length; i++)
        {
            var menuItem = PlatformView.Menu.Children[i];

            if (menuItem is UIAction action)
            {
                action.State = i == selectedIndex ? UIMenuElementState.On : UIMenuElementState.Off;
            }
        }

        if (VirtualViewDropdown.SelectedItem is null)
        {
            VirtualViewDropdown.Text = VirtualViewDropdown.Placeholder;
            PlatformView.SetTitleColor(VirtualViewDropdown.PlaceholderColor.ToPlatform(), UIControlState.Normal);
        }
        else
        {
            VirtualViewDropdown.Text = VirtualViewDropdown.SelectedItem?.ToString();
            PlatformView.SetTitleColor(VirtualViewDropdown.TextColor?.ToPlatform() ?? Colors.Black.ToPlatform(), UIControlState.Normal);
        }
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
}
#endif