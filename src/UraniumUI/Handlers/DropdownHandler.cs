using Microsoft.Maui.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UraniumUI.Controls;

namespace UraniumUI.Handlers;
public partial class DropdownHandler
{
    public static IPropertyMapper<Dropdown, DropdownHandler> DropdownPropertyMapper =>
        new PropertyMapper<Dropdown, DropdownHandler>(ViewHandler.ViewMapper)
        {
            [nameof(Dropdown.ItemsSource)] = MapItemsSource,
            [nameof(Dropdown.SelectedItem)] = MapSelectedItem,
        };

    public DropdownHandler() : base(DropdownPropertyMapper)
    {

    }
}

#if ANDROID
public partial class DropdownHandler : ViewHandler<Dropdown, AndroidX.AppCompat.Widget.AppCompatButton>
{
    public DropdownHandler(IPropertyMapper mapper, CommandMapper commandMapper = null) : base(DropdownPropertyMapper, commandMapper)
    {

    }

    protected override AndroidX.AppCompat.Widget.AppCompatButton CreatePlatformView()
    {
        var button = new AndroidX.AppCompat.Widget.AppCompatButton(Context);
        button.Text = VirtualView?.SelectedItem?.ToString();
        button.Click += Button_Click;
        return button;
    }

    private void Button_Click(object sender, EventArgs e)
    {
        var activity = Microsoft.Maui.ApplicationModel.Platform.CurrentActivity;
        var popupMenu = new Android.Widget.PopupMenu(activity, PlatformView);

        if (VirtualView.ItemsSource is not null)
        {
            foreach (var item in VirtualView.ItemsSource)
            {
                popupMenu.Menu.Add(new Java.Lang.String(item.ToString()));
            }
        }

        popupMenu.Show();
    }

    protected override void DisconnectHandler(AndroidX.AppCompat.Widget.AppCompatButton platformView)
    {
        base.DisconnectHandler(platformView);
        platformView.Click -= Button_Click;
    }

    public static void MapItemsSource(DropdownHandler handler, Dropdown dropdown)
    {

    }
    public static void MapSelectedItem(DropdownHandler handler, Dropdown dropdown)
    {

    }
}
#endif

#if IOS || MACCATALYST
public partial class DropdownHandler : ViewHandler<Dropdown, UIKit.UIButton>
{
    public DropdownHandler(IPropertyMapper mapper, CommandMapper commandMapper = null) : base(DropdownPropertyMapper, commandMapper)
    {

    }

    protected override UIKit.UIButton CreatePlatformView()
    {
        var button = new UIKit.UIButton();

        if (VirtualView.ItemsSource is not null)
        {
            var items = new UIKit.UIMenuElement[VirtualView.ItemsSource.Count];
            for (int i = 0; i < VirtualView.ItemsSource.Count; i++)
            {
                items[i] = UIKit.UIAction.Create(VirtualView.ItemsSource[i].ToString().ToString(), null, VirtualView.ItemsSource[i].ToString(), _ => { Console.WriteLine("Selected"); });
            }

            button.SetTitle(VirtualView?.SelectedItem?.ToString() ?? "Tap to choose", UIKit.UIControlState.Normal);
            button.Menu = UIKit.UIMenu.Create(items);
            button.ShowsMenuAsPrimaryAction = true;
            button.ChangesSelectionAsPrimaryAction = true;
        }

        return button;
    }

    public static void MapItemsSource(DropdownHandler handler, Dropdown dropdown)
    {

    }
    public static void MapSelectedItem(DropdownHandler handler, Dropdown dropdown)
    {

    }
}
#endif

#if WINDOWS

public partial class DropdownHandler : ViewHandler<Dropdown, Microsoft.UI.Xaml.Controls.DropDownButton>
{
    public DropdownHandler(IPropertyMapper mapper, CommandMapper commandMapper = null) : base(DropdownPropertyMapper, commandMapper)
    {

    }

    protected override Microsoft.UI.Xaml.Controls.DropDownButton CreatePlatformView()
    {
        var dropdownButton = new Microsoft.UI.Xaml.Controls.DropDownButton();
        var flyout = new Microsoft.UI.Xaml.Controls.MenuFlyout();
        flyout.Placement = Microsoft.UI.Xaml.Controls.Primitives.FlyoutPlacementMode.Bottom;

        if (VirtualView.ItemsSource is not null)
        {
            flyout.Items.Clear();
            foreach (var item in VirtualView.ItemsSource)
            {
                var menuItem = new Microsoft.UI.Xaml.Controls.MenuFlyoutItem();
                menuItem.Text = item.ToString();
                flyout.Items.Add(menuItem);
            }
        }

        dropdownButton.Flyout = flyout;
        return dropdownButton;
    }

    public static void MapItemsSource(DropdownHandler handler, Dropdown dropdown)
    {

    }
    public static void MapSelectedItem(DropdownHandler handler, Dropdown dropdown)
    {

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