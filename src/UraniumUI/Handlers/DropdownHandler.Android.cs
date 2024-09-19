#if ANDROID
using Android.Views;
using Google.Android.Material.Button;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using UraniumUI.Controls;
using UraniumUI.Extensions;
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

        button.Text = GetTextForItem(VirtualViewDropdown, VirtualViewDropdown?.SelectedItem);

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
                var menuItem = popupMenu.Menu.Add(new Java.Lang.String(GetTextForItem(VirtualViewDropdown, item)));

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
        ArrangeText();
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
        handler.ArrangeText();
    }

    protected void ArrangeText()
    {
        if (VirtualViewDropdown.SelectedItem is null)
        {
            VirtualViewDropdown.Text = VirtualViewDropdown.Placeholder;
            PlatformView.SetTextColor(VirtualViewDropdown.PlaceholderColor.ToPlatform());
        }
        else
        {
            VirtualViewDropdown.Text = VirtualViewDropdown.SelectedItem?.ToString();
            PlatformView.SetTextColor(VirtualViewDropdown.TextColor?.ToPlatform() ?? Colors.Black.ToPlatform());
        }
    }

    public static void MapPlaceholder(DropdownHandler handler, Dropdown dropdown)
    {
        if (dropdown.SelectedItem is null)
        {
            dropdown.Text = dropdown.Placeholder;
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

    public static void MapTextColor(DropdownHandler handler, Dropdown dropdown)
    {
        handler.ArrangeText();
    }

    private string GetTextForItem(Dropdown dropdown, object item)
    {
        if (dropdown?.ItemDisplayBinding is not null)
        {
            return dropdown.ItemDisplayBinding.GetValueOnce<string>(item);
        }
        return item?.ToString();
    }

    public static void MapItemDisplayBinding(DropdownHandler handler, Dropdown dropdown)
    {
        // Do nothing on Android.
    }
}
#endif