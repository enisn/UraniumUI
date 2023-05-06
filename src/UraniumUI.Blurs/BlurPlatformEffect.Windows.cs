#if WINDOWS

using Microsoft.Maui.Platform;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.Maui.Controls.Platform;

namespace UraniumUI.Blurs;
public class BlurPlatformEffect : PlatformEffect
{
    public BlurEffect VirtualEffect { get; private set; }
    protected override void OnAttached()
    {
        if (Element.Effects.FirstOrDefault(x => x.ResolveId == this.ResolveId) is BlurEffect blurEffect)
        {
            VirtualEffect = blurEffect;
            blurEffect.UpdateEffectCommand = new Command(UpdateEffect);
        }

        UpdateEffect();
    }

    protected void UpdateEffect()
    {
        if (Control is Control control)
        {
            control.Background = GetBrush();
        }

        if (Control is Panel panel)
        {
            panel.Background = GetBrush();
        }
    }

    protected AcrylicBrush GetBrush()
    {
        if (VirtualEffect?.AccentColor != null && VirtualEffect.AccentColor.IsNotDefault())
        {
            return new AcrylicBrush
            {
                TintColor = VirtualEffect.AccentColor.ToWindowsColor(),
                TintOpacity =  VirtualEffect.AccentOpacity,
                TintLuminosityOpacity = .4
            };
        }

        return new AcrylicBrush
        {
            TintColor = VirtualEffect?.Mode == BlurMode.Dark ? Colors.Black.ToWindowsColor() : Colors.DimGray.ToWindowsColor(),
            TintOpacity = VirtualEffect.AccentOpacity,
            TintLuminosityOpacity = .4
        };
    }

    protected override void OnDetached()
    {
        if (Control is Control control)
        {
            control.Background = null;
        }

        if (Control is Panel panel)
        {
            panel.Background = null;
        }
    }
}

#endif