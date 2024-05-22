#if ANDROID
using Android.Graphics.Drawables;
using Android.Views;
using Android.Widget;
using Google.Android.Material.Button;

#endif

using Microsoft.Maui.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#if IOS || MACCATALYST
using UIKit;
#endif
using UraniumUI.Controls;

namespace UraniumUI.Handlers;
public partial class DropdownHandler
{
    public Dropdown VirtualViewDropdown => VirtualView as Dropdown;
    public static IPropertyMapper<Dropdown, DropdownHandler> DropdownPropertyMapper =>
        new PropertyMapper<Dropdown, DropdownHandler>(ButtonHandler.Mapper)
        {
            [nameof(Dropdown.ItemsSource)] = MapItemsSource,
            [nameof(Dropdown.SelectedItem)] = MapSelectedItem,
        };

    public DropdownHandler() : base(DropdownPropertyMapper)
    {

    }
}

#if ANDROID
public partial class DropdownHandler : ButtonHandler
{
    public DropdownHandler(IPropertyMapper mapper, CommandMapper commandMapper = null) : base(DropdownPropertyMapper, commandMapper)
    {

    }

    protected override MaterialButton CreatePlatformView()
    {
        var button = base.CreatePlatformView();
        button.Text = VirtualViewDropdown?.SelectedItem?.ToString();
        button.Click += Button_Click;
        return button;
    }

    private void Button_Click(object sender, EventArgs e)
    {
        var activity = Microsoft.Maui.ApplicationModel.Platform.CurrentActivity;

        var popupMenu = new Android.Widget.PopupMenu(activity, PlatformView, GravityFlags.Top);

        if (VirtualViewDropdown.ItemsSource is not null)
        {
            foreach (var item in VirtualViewDropdown.ItemsSource)
            {
                popupMenu.Menu.Add(new Java.Lang.String(item.ToString()));
            }
        }

        popupMenu.Show();
    }

    protected override void DisconnectHandler(MaterialButton platformView)
    {
        base.DisconnectHandler(platformView);
        platformView.Click -= Button_Click;
    }

    public static void MapItemsSource(DropdownHandler handler, Dropdown dropdown)
    {
        // Do nothing on Android.
    }

    public static void MapSelectedItem(DropdownHandler handler, Dropdown dropdown)
    {
        handler.PlatformView.Text = dropdown.SelectedItem?.ToString();
    }
}
#endif

#if IOS || MACCATALYST
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

#if WINDOWS

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

#endif

#if (NET8_0) && !ANDROID && !IOS && !MACCATALYST && !WINDOWS
public partial class DropdownHandler : ViewHandler<Dropdown, object>
{
	public DropdownHandler(IPropertyMapper mapper, CommandMapper commandMapper = null) : base(DropdownPropertyMapper, commandMapper)
    {

	}

    protected override object CreatePlatformView()
    {
        throw new NotImplementedException();
    }

    public static void MapItemsSource(DropdownHandler handler, Dropdown dropdown)
    {

    }
    public static void MapSelectedItem(DropdownHandler handler, Dropdown dropdown)
    {

    }
}

#endif