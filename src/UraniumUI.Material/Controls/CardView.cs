namespace UraniumUI.Material.Controls;

public partial class CardView : Frame
{
    public const int AcrylicElevation = 20;

    public static readonly BindableProperty MaterialThemeProperty = BindableProperty.Create(
        nameof(MaterialTheme),
        typeof(Theme),
        typeof(CardView),
        defaultValueCreator: _ => globalTheme);

    public static readonly BindableProperty MaterialBlurStyleProperty = BindableProperty.Create(
        nameof(MaterialBlurStyle),
        typeof(BlurStyle),
        typeof(CardView),
        defaultValue: DefaultBlurStyle);

    public static readonly BindableProperty LightThemeBackgroundColorProperty = BindableProperty.Create(
        nameof(LightThemeBackgroundColor),
        typeof(Color),
        typeof(CardView),
        defaultValueCreator: _ => DefaultLightThemeBackgroundColor);

    public static readonly BindableProperty AcrylicGlowColorProperty = BindableProperty.Create(
        nameof(AcrylicGlowColor),
        typeof(Color),
        typeof(CardView),
        defaultValueCreator: _ => DefaultAcrylicGlowColor);

    public static readonly BindableProperty ElevationProperty = BindableProperty.Create(
        nameof(Elevation),
        typeof(int),
        typeof(CardView),
        defaultValue: DefaultElevation);

    private const Theme DefaultTheme = Theme.Light;

    private const int DefaultElevation = 2;

    private const BlurStyle DefaultBlurStyle = BlurStyle.Light;

    private static readonly Color DefaultLightThemeBackgroundColor = Colors.White;

    private static readonly Color DefaultAcrylicGlowColor = Colors.White;

    // https://material.io/design/color/dark-theme.html#properties
    private static readonly Color[] DarkColors = new[]
    {
            Color.FromArgb("121212"), // 00dp
            Color.FromArgb("1D1D1D"),
            Color.FromArgb("212121"),
            Color.FromArgb("242424"),
            Color.FromArgb("272727"), // 04dp
            Color.FromArgb("272727"),
            Color.FromArgb("2C2C2C"), // 06dp
            Color.FromArgb("2C2C2C"),
            Color.FromArgb("2D2D2D"), // 08dp
            Color.FromArgb("2D2D2D"),
            Color.FromArgb("2D2D2D"),
            Color.FromArgb("2D2D2D"),
            Color.FromArgb("323232"), // 12dp
            Color.FromArgb("323232"),
            Color.FromArgb("323232"),
            Color.FromArgb("323232"),
            Color.FromArgb("353535"), // 16dp
            Color.FromArgb("353535"),
            Color.FromArgb("353535"),
            Color.FromArgb("353535"),
            Color.FromArgb("353535"),
            Color.FromArgb("353535"),
            Color.FromArgb("353535"),
            Color.FromArgb("353535"),
            Color.FromArgb("373737"), // 24dp
        };

    private static Theme globalTheme = DefaultTheme;

    public CardView()
    {
        HasShadow = false;

        ThemeChanged += OnThemeChanged;
    }

    public static event EventHandler ThemeChanged;

    public enum Theme
    {
        Light = 0,
        Dark,
        Acrylic,
        AcrylicBlur,
    }

    public enum BlurStyle
    {
        Light = 0,
        ExtraLight,
        Dark,
    }

    public bool IsShadowCompatible => MaterialTheme == Theme.Acrylic || (MaterialTheme == Theme.Light && Elevation > 0);

    public Theme MaterialTheme
    {
        get => (Theme)GetValue(MaterialThemeProperty);
        set => SetValue(MaterialThemeProperty, value);
    }

    public Color LightThemeBackgroundColor
    {
        get => (Color)GetValue(LightThemeBackgroundColorProperty);
        set => SetValue(LightThemeBackgroundColorProperty, value);
    }

    public Color AcrylicGlowColor
    {
        get => (Color)GetValue(AcrylicGlowColorProperty);
        set => SetValue(AcrylicGlowColorProperty, value);
    }

    public BlurStyle MaterialBlurStyle
    {
        get => (BlurStyle)GetValue(MaterialBlurStyleProperty);
        set => SetValue(MaterialBlurStyleProperty, value);
    }

    public int Elevation
    {
        get => (int)GetValue(ElevationProperty);
        set => SetValue(ElevationProperty, value);
    }

    public static void ChangeGlobalTheme(Theme newTheme)
    {
        var previousTheme = globalTheme;
        globalTheme = newTheme;

        if (previousTheme != globalTheme)
        {
            ThemeChanged?.Invoke(null, new EventArgs());
        }
    }

    public void Unsubscribe()
    {
        ThemeChanged -= OnThemeChanged;
    }

    public Color ElevationToColor()
    {
        if (MaterialTheme == Theme.Light)
        {
            return Colors.Transparent;
        }

        if (Elevation < 0)
        {
            return Colors.Transparent;
        }

        int index = Elevation > 24 ? 24 : Elevation;
        return DarkColors[index];
    }

    private void OnThemeChanged(object sender, EventArgs eventArgs)
    {
        MaterialTheme = globalTheme;
    }
}
