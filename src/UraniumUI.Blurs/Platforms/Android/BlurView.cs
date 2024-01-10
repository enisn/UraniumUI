using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.Annotations;
using Color = Android.Graphics.Color;

namespace UraniumUI.Blurs;
public class BlurView : FrameLayout
{
    private static string TAG = typeof(BlurView).Name;

    IBlurController blurController = new NoOpController();

    private Color overlayColor;

    public BlurView(Context context) : base(context)
    {
    }

    public BlurView(Context context, IAttributeSet attrs) : base(context, attrs)
    {
    }

    public BlurView(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
    {
    }

    public override void Draw(Canvas canvas)
    {
        var shouldDraw = blurController.Draw(canvas);
        if (shouldDraw)
        {
            base.Draw(canvas);
        }
    }

    protected override void OnSizeChanged(int w, int h, int oldw, int oldh)
    {
        base.OnSizeChanged(w, h, oldw, oldh);
        blurController.UpdateBlurViewSize();
    }

    protected override void OnAttachedToWindow()
    {
        base.OnAttachedToWindow();
        if (!IsHardwareAccelerated)
        {
            Log.Error(TAG, "BlurView can't be used in not hardware-accelerated window!");
        }
        else
        {
            blurController.SetBlurAutoUpdate(true);
        }
    }

    /**
   * @param rootView  root to start blur from.
   *                  Can be Activity's root content layout (android.R.id.content)
   *                  or (preferably) some of your layouts. The lower amount of Views are in the root, the better for performance.
   * @param algorithm sets the blur algorithm
   * @return {@link BlurView} to setup needed params.
   */
    public IBlurViewFacade SetupWith(ViewGroup rootView, IBlurAlgorithm algorithm)
    {
        this.blurController.Destroy();
        var blurController = new PreDrawBlurController(this, rootView, overlayColor, algorithm);
        this.blurController = blurController;

        return blurController;
    }

    /**
    * @param rootView root to start blur from.
    *                 Can be Activity's root content layout (android.R.id.content)
    *                 or (preferably) some of your layouts. The lower amount of Views are in the root, the better for performance.
    *                 <p>
    *                 BlurAlgorithm is automatically picked based on the API version.
    *                 It uses RenderEffectBlur on API 31+, and RenderScriptBlur on older versions.
    * @return {@link BlurView} to setup needed params.
    */

    [RequiresApi(Value = (int)BuildVersionCodes.JellyBeanMr1)]
    public IBlurViewFacade SetupWith(ViewGroup rootView)
    {
        return SetupWith(rootView, GetBlurAlgorithm());
    }

    /**
  * @see BlurViewFacade#setBlurRadius(float)
  */
    public IBlurViewFacade SetBlurRadius(float radius)
    {
        return blurController.SetBlurRadius(radius);
    }

    /**
    * @see BlurViewFacade#setOverlayColor(int)
    */
    public IBlurViewFacade SetOverlayColor(Color overlayColor)
    {
        this.overlayColor = overlayColor;
        return blurController.SetOverlayColor(overlayColor);
    }

    /**
     * @see BlurViewFacade#setBlurAutoUpdate(boolean)
     */
    public IBlurViewFacade setBlurAutoUpdate(bool enabled)
    {
        return blurController.SetBlurAutoUpdate(enabled);
    }

    /**
     * @see BlurViewFacade#setBlurEnabled(boolean)
     */
    public IBlurViewFacade setBlurEnabled(bool enabled)
    {
        return blurController.SetBlurEnabled(enabled);
    }

    [RequiresApi(Value = (int)BuildVersionCodes.JellyBeanMr1)]
    private IBlurAlgorithm GetBlurAlgorithm()
    {
        IBlurAlgorithm algorithm;
        if (Build.VERSION.SdkInt >= BuildVersionCodes.S)
        {
            algorithm = new RenderEffectBlur();
        }
        else
        {
            algorithm = new RenderScriptBlur(Context);
        }
        return algorithm;
    }
}
