using Android.Views;

namespace UraniumUI.Blurs;

public class OnPreDrawListener : Java.Lang.Object, ViewTreeObserver.IOnPreDrawListener
{
    Action actionOnPreDraw;

    public OnPreDrawListener(Action actionOnPreDraw)
    {
        this.actionOnPreDraw = actionOnPreDraw;
    }

    public bool OnPreDraw()
    {
        actionOnPreDraw?.Invoke();
        return true;
    }
}
