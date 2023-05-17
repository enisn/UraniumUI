#if IOS || MACCATALYST
using System.ComponentModel;
using Microsoft.Maui.Controls.Platform;
using Microsoft.Maui.Platform;
using UIKit;

namespace UraniumUI.Blurs;
public class BlurPlatformEffect : PlatformEffect
{
    public BlurEffect VirtualEffect { get; private set; }

    protected UIVisualEffectView blurView;

    protected override void OnAttached()
    {
        var platformView = this.Control;

        if (Element.Effects.FirstOrDefault(x => x.ResolveId == this.ResolveId) is BlurEffect _effect)
        {
            VirtualEffect = _effect;
            _effect.UpdateEffectCommand = new Command(UpdateEffect);
        }

        Control.BackgroundColor = UIColor.Clear;

        blurView = new UIVisualEffectView();
        blurView.TranslatesAutoresizingMaskIntoConstraints = false;

        UpdateEffect();

        platformView.InsertSubview(blurView, 0);

        NSLayoutConstraint.ActivateConstraints(new[] {
            blurView.TopAnchor.ConstraintEqualTo(platformView.TopAnchor),
            blurView.LeadingAnchor.ConstraintEqualTo(platformView.LeadingAnchor),
            blurView.HeightAnchor.ConstraintEqualTo(platformView.HeightAnchor),
            blurView.WidthAnchor.ConstraintEqualTo(platformView.WidthAnchor)
        });
    }

    protected override void OnDetached()
    {
        Control.Subviews.FirstOrDefault(x => x is UIVisualEffectView)?.RemoveFromSuperview();
    }

    protected override void OnElementPropertyChanged(PropertyChangedEventArgs args)
    {
        base.OnElementPropertyChanged(args);
        if (args.PropertyName == nameof(View.BackgroundColorProperty.PropertyName))
        {
            Control.BackgroundColor = (this.Element as View).BackgroundColor.WithAlpha(.2f).ToPlatform();
        }
    }

    protected void UpdateEffect()
    {
        if (VirtualEffect?.AccentColor != null && VirtualEffect.AccentColor.IsNotDefault())
        {
            if (this.Element is View view)
            {
                Control.BackgroundColor = VirtualEffect.AccentColor.WithAlpha(VirtualEffect.AccentOpacity).ToPlatform();
            }
        }

        blurView.Effect = VirtualEffect?.Mode == BlurMode.Dark ?
            UIBlurEffect.FromStyle(UIBlurEffectStyle.Dark) :
            UIBlurEffect.FromStyle(UIBlurEffectStyle.Light);
    }
}

#endif