using Android.Graphics.Drawables;
using Color = Android.Graphics.Color;

namespace UraniumUI.Blurs;

public interface IBlurViewFacade
{
    IBlurViewFacade SetBlurEnabled(bool enabled);

    IBlurViewFacade SetBlurAutoUpdate(bool enabled);

    IBlurViewFacade SetFrameClearDrawable(Drawable frameClearDrawable);

    IBlurViewFacade SetBlurRadius(float radius);

    IBlurViewFacade SetOverlayColor(Color overlayColor);
}
