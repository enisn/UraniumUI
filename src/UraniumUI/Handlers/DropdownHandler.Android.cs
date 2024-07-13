#if ANDROID
using Android.Views;
using Google.Android.Material.Button;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using UraniumUI.Controls;
using UraniumUI.Platforms.Android;

namespace UraniumUI.Handlers;
public partial class DropdownHandler : ButtonHandler
{
    public DropdownHandler(IPropertyMapper mapper, CommandMapper commandMapper = null) : base(DropdownPropertyMapper, commandMapper)
    {

    }

    protected override MaterialButton CreatePlatformView()
    {
        var button = base.CreatePlatformView();
        button.Text = VirtualViewDropdown?.SelectedItem?.ToString();

        button.TextAlignment = Android.Views.TextAlignment.TextStart;
        return button;
    }

    private void Button_Click(object sender, EventArgs e)
    {
        var activity = Microsoft.Maui.ApplicationModel.Platform.CurrentActivity;

        var popupMenu = new Android.Widget.PopupMenu(activity, PlatformView, GetGravityFlags(VirtualViewDropdown.HorizontalTextAlignment));

        if (VirtualViewDropdown.ItemsSource is not null)
        {
            foreach (var item in VirtualViewDropdown.ItemsSource)
            {
                var menuItem = popupMenu.Menu.Add(new Java.Lang.String(item.ToString()));

                menuItem.SetOnMenuItemClickListener(new MenuItemOnMenuItemClickListener((menuitem) =>
                {
                    VirtualViewDropdown.SelectedItem = item;
                }));
            }
        }

        popupMenu.Show();
    }

    private GravityFlags GetGravityFlags(Microsoft.Maui.TextAlignment textAlignment)
    {
        return textAlignment switch
        {
            Microsoft.Maui.TextAlignment.Start => GravityFlags.Start,
            Microsoft.Maui.TextAlignment.Center => GravityFlags.Center,
            Microsoft.Maui.TextAlignment.End => GravityFlags.End,
            _ => GravityFlags.Start
        };
    }

    protected override void ConnectHandler(MaterialButton platformView)
    {
        base.ConnectHandler(platformView);
        platformView.Click += Button_Click;
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
        if (dropdown.SelectedItem is null)
        {
            handler.PlatformView.Text = dropdown.Placeholder;
            handler.PlatformView.SetTextColor(dropdown.PlaceholderColor.ToPlatform());
        }
        else
        {
            handler.PlatformView.Text = dropdown.SelectedItem?.ToString();
            handler.PlatformView.SetTextColor(dropdown.TextColor?.ToPlatform() ?? Colors.Black.ToPlatform());
        }
    }

    public static void MapPlaceholder(DropdownHandler handler, Dropdown dropdown)
    {
        if (dropdown.SelectedItem is null)
        {
            handler.PlatformView.Text = dropdown.Placeholder;
        }
    }

    public static void MapPlaceholderColor(DropdownHandler handler, Dropdown dropdown)
    {
        if (dropdown.SelectedItem is null)
        {
            handler.PlatformView.SetTextColor(dropdown.PlaceholderColor.ToPlatform());
        }
    }

    public static void MapHorizontalTextAlignment(DropdownHandler handler, Dropdown dropdown)
    {
        handler.PlatformView.TextAlignment = dropdown.HorizontalTextAlignment switch
        {
            Microsoft.Maui.TextAlignment.Start => Android.Views.TextAlignment.TextStart,
            Microsoft.Maui.TextAlignment.Center => Android.Views.TextAlignment.Center,
            Microsoft.Maui.TextAlignment.End => Android.Views.TextAlignment.TextEnd,
            _ => Android.Views.TextAlignment.TextStart
        };
    }
}
#endif