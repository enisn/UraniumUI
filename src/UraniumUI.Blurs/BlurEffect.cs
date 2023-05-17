using System.Windows.Input;

namespace UraniumUI.Blurs;
public class BlurEffect : RoutingEffect
{
    private BlurMode mode;
    private Color accentColor;
    private float accentOpacity = .2f;

    public BlurMode Mode { get => mode; set { mode = value; UpdateEffectCommand?.Execute(this); } }

    public Color AccentColor { get => accentColor; set { accentColor = value; UpdateEffectCommand?.Execute(this); } }

    public float AccentOpacity { get => accentOpacity; set { accentOpacity = value; UpdateEffectCommand?.Execute(this); } }

    internal ICommand UpdateEffectCommand { get; set; }

    public BlurEffect()
    {
        mode = Application.Current?.RequestedTheme == AppTheme.Dark ? BlurMode.Dark : BlurMode.Light;
    }
}

public enum BlurMode
{
    Light,
    Dark,
}
