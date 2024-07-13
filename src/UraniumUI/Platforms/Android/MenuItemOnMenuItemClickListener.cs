using Android.Views;

namespace UraniumUI.Platforms.Android;

public class MenuItemOnMenuItemClickListener : Java.Lang.Object, IMenuItemOnMenuItemClickListener
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