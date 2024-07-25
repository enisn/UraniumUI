#if ANDROID
using Android.Content;
using Android.Graphics.Drawables;
using Android.Views;
using Android.Widget;
using Microsoft.Maui.Controls.Compatibility.Platform.Android;
using Microsoft.Maui.Controls.Platform;
using Microsoft.Maui.Platform;
using Color = Android.Graphics.Color;

namespace UraniumUI.Blurs;
public class BlurPlatformEffect : PlatformEffect
{
    public Context Context => Control?.Context;

    private BlurView _blurView;
    private GradientDrawable _mainDrawable;

    public BlurEffect VirtualEffect { get; private set; }

    protected override void OnAttached()
    {
        if (Element.Effects.FirstOrDefault(x => x.ResolveId == this.ResolveId) is BlurEffect blurEffect)
        {
            VirtualEffect = blurEffect;
            blurEffect.UpdateEffectCommand = new Command(() =>
            {
                _blurView.SetBackgroundColor(GetColor());
                //AlignBlurView();
            });
        }

        if (Element is Microsoft.Maui.Controls.View view)
        {
            view.SizeChanged += BlurPlatformEffect_SizeChanged;
            view.ParentChanged += View_ParentChanged;
        }

        UpdateEffect();
    }

    protected override void OnDetached()
    {
        if (Element is Microsoft.Maui.Controls.View view)
        {
            view.SizeChanged -= BlurPlatformEffect_SizeChanged;
            view.ParentChanged -= View_ParentChanged;
        }

        //TODO: Release drawable
    }

    private void BlurPlatformEffect_SizeChanged(object sender, EventArgs e)
    {
        AlignBlurView();
    }

    private void View_ParentChanged(object sender, EventArgs e)
    {
        UpdateEffect();
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

            if (_blurView == null)
            {
                _blurView = new BlurView(Context);

                //var child = viewGroup.GetChildAt(0) ?? new Android.Views.View(Context);
                //child.RemoveFromParent();
                //_blurView.AddView(child);

                while (viewGroup.GetChildAt(0) != null)
                {
                    var child = viewGroup.GetChildAt(0);
                    child.RemoveFromParent();
                    _blurView.AddView(child);
                }

                viewGroup.AddView(_blurView, 0, new FrameLayout.LayoutParams(
                        ViewGroup.LayoutParams.FillParent,
                        ViewGroup.LayoutParams.FillParent,
                        GravityFlags.NoGravity));

                _blurView.SetOverlayColor(Color.Transparent);
                AlignBlurView();
            }

            var decorView = Microsoft.Maui.ApplicationModel.Platform.CurrentActivity.Window.DecorView;
            var root = decorView.FindViewById(global::Android.Resource.Id.Content) as global::Android.Views.ViewGroup;
            var windowBackground = decorView.Background;

            _blurView.SetBackgroundColor(GetColor());

            _blurView
               .SetupWith(root as global::Android.Views.ViewGroup, new RenderScriptBlur(Context))
               .SetFrameClearDrawable(windowBackground) // Optional
               .SetBlurRadius(24f);
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

        //if (PlatformView.MeasuredWidth == 0 || PlatformView.MeasuredHeight == 0 || _blurView == null)
        //{
        //    return;
        //}       
        if (_blurView == null)
        {
            return;
        }

        int width = PlatformView.MeasuredWidth;
        int height = PlatformView.MeasuredHeight;
        _blurView.Measure(width, height);
        _blurView.Layout(0, 0, width, height);
    }
}

#endif