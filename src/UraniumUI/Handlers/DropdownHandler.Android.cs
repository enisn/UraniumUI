#if ANDROID
using Android.Views;
using Google.Android.Material.Button;
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