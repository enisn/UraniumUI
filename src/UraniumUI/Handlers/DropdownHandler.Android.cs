#if ANDROID
using Android.Views;
using Android.Widget;
using Google.Android.Material.Button;
using Java.Interop;
using Microsoft.Maui.Handlers;
using UraniumUI.Controls;

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
        return button;
    }

    private void Button_Click(object sender, EventArgs e)
    {
        var activity = Microsoft.Maui.ApplicationModel.Platform.CurrentActivity;

        var popupMenu = new Android.Widget.PopupMenu(activity, PlatformView, GravityFlags.Center);

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

    class MenuItemOnMenuItemClickListener : Java.Lang.Object, IMenuItemOnMenuItemClickListener
    {
        private Action<IMenuItem> _onMenuItemClick;

        public MenuItemOnMenuItemClickListener(Action<IMenuItem> onMenuItemClick)
        {
            _onMenuItemClick = onMenuItemClick;
        }

        public bool OnMenuItemClick(IMenuItem item)
        {
            _onMenuItemClick?.Invoke(item);
            return true;
        }
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
        handler.PlatformView.Text = dropdown.SelectedItem?.ToString();
    }
}
#endif