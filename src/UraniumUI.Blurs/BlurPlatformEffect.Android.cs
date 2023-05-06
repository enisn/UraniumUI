#if ANDROID
using Android.Content;
using Android.Graphics.Drawables;
using Android.Views;
using Android.Widget;
using Microsoft.Maui.Controls.Compatibility.Platform.Android;
using Microsoft.Maui.Controls.Platform;
using Microsoft.Maui.Platform;
using UraniumUI.Blurs.Platforms.Android;

namespace UraniumUI.Blurs;
public class BlurPlatformEffect : PlatformEffect
{
    public Context Context => Control?.Context;

    private RealtimeBlurView _realtimeBlurView;
    private GradientDrawable _mainDrawable;

    public BlurEffect VirtualEffect { get; private set; }

    protected override void OnAttached()
    {

        if (Element.Effects.FirstOrDefault(x => x.ResolveId == this.ResolveId) is BlurEffect blurEffect)
        {
            VirtualEffect = blurEffect;
            blurEffect.UpdateEffectCommand = new Command(() =>
            {
                _realtimeBlurView.SetOverlayColor(GetColor(), true);
                AlignBlurView();
            });
        }

        UpdateEffect();
        if (Element is Microsoft.Maui.Controls.View view)
        {
            view.SizeChanged += BlurPlatformEffect_SizeChanged;
            view.ParentChanged += View_ParentChanged;
        }
    }

    private void View_ParentChanged(object sender, EventArgs e)
    {
        AlignBlurView();
    }

    protected override void OnDetached()
    {
        if (Element is Microsoft.Maui.Controls.View view)
        {
            view.SizeChanged -= BlurPlatformEffect_SizeChanged;
        }

        //TODO: Release drawable
    }

    private void BlurPlatformEffect_SizeChanged(object sender, EventArgs e)
    {
        AlignBlurView();
    }

    protected void UpdateEffect()
    {
        if (Control is ViewGroup viewGroup)
        {
            if (_mainDrawable == null)
            {
                _mainDrawable = new GradientDrawable();
                _mainDrawable.SetColor(Colors.Transparent.ToAndroid());
                Control.Background = _mainDrawable;
            }

            if (_realtimeBlurView == null)
            {
                _realtimeBlurView = new RealtimeBlurView(Context, Element.AutomationId);
            }

            _realtimeBlurView.SetBlurRadius(Context.ToPixels(48), true);

            _realtimeBlurView.SetOverlayColor(GetColor(), true);

            _realtimeBlurView?.SetRootView(Control.Parent as global::Android.Views.View);

            _realtimeBlurView.SetDownsampleFactor(1);

            if (viewGroup.ChildCount > 0 && ReferenceEquals(viewGroup.GetChildAt(0), _realtimeBlurView))
            {
                // Already added
                return;
            }

            //InternalLogger.Info(FormsId, "Renderer::EnableBlur() => adding pre draw listener");
            viewGroup.AddView(
                _realtimeBlurView,
                0,
                new FrameLayout.LayoutParams(
                    ViewGroup.LayoutParams.FillParent,
                    ViewGroup.LayoutParams.FillParent,
                    GravityFlags.NoGravity));

            AlignBlurView();
        }
        else
        {
            // Not supported for Standalone Views.
        }
    }

    protected Android.Graphics.Color GetColor()
    {
        if (VirtualEffect?.AccentColor != null && VirtualEffect.AccentColor.IsNotDefault())
        {
            return VirtualEffect.AccentColor.WithAlpha(VirtualEffect.AccentOpacity).ToAndroid();
        }

        return VirtualEffect?.Mode == BlurMode.Dark
            ? Colors.Black.WithAlpha(VirtualEffect.AccentOpacity).ToAndroid()
            : Colors.White.WithAlpha(VirtualEffect.AccentOpacity).ToAndroid();
    }

    private void AlignBlurView()
    {
        var PlatformView = Control;

        if (PlatformView.MeasuredWidth == 0 || PlatformView.MeasuredHeight == 0 || _realtimeBlurView == null)
        {
            return;
        }

        int width = PlatformView.MeasuredWidth;
        int height = PlatformView.MeasuredHeight;
        _realtimeBlurView.Measure(width, height);
        _realtimeBlurView.Layout(0, 0, width, height);
    }
}

#endif