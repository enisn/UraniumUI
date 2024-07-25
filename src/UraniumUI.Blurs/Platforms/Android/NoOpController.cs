using Android.Graphics;
using Android.Graphics.Drawables;
using Color = Android.Graphics.Color;

namespace UraniumUI.Blurs;
public class NoOpController : IBlurController
{
    public bool Draw(Canvas canvas)
    {
        return true;
    }

    public void Destroy()
    {
    }

    public IBlurViewFacade SetBlurAutoUpdate(bool enabled)
    {
        return this;
    }

    public IBlurViewFacade SetBlurEnabled(bool enabled)
    {
        return this;
    }

    public IBlurViewFacade SetBlurRadius(float radius)
    {
        return this;
    }

    public IBlurViewFacade SetFrameClearDrawable(Drawable frameClearDrawable)
    {
        return this;
    }

    public IBlurViewFacade SetOverlayColor(Color overlayColor)
    {
        return this;
    }

    public void UpdateBlurViewSize()
    {
    }
}
