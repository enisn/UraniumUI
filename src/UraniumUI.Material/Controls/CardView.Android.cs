#if ANDROID

namespace UraniumUI.Material.Controls;
public partial class CardView
{
    public static readonly BindableProperty AndroidBlurRootElementProperty = BindableProperty.Create(
            nameof(AndroidBlurRootElement),
            typeof(Layout),
            typeof(CardView));

    public static readonly BindableProperty AndroidBlurOverlayColorProperty = BindableProperty.Create(
        nameof(AndroidBlurOverlayColor),
        typeof(Color),
        typeof(CardView),
        defaultValueCreator: _ => Colors.Transparent);

    public static readonly BindableProperty AndroidBlurRadiusProperty = BindableProperty.Create(
        nameof(AndroidBlurRadius),
        typeof(float),
        typeof(CardView),
        defaultValue: 0f);

    /// <summary>
    /// Android only: the root element must be an ancestor of the MaterialFrame.
    /// Blur computation is very costly on Android since it needs to process all the view hierarchy from the
    /// root element to be blurred (most of the time the element displaying the underlying image) to the blur frame.
    /// The shorter the path, the better the performance. If no root element is set, the activity decor view is used.
    /// </summary>
    public Layout AndroidBlurRootElement
    {
        get => (Layout)GetValue(AndroidBlurRootElementProperty);
        set => SetValue(AndroidBlurRootElementProperty, value);
    }

    /// <summary>
    /// Android only.
    /// Changes the overlay color over the blur (should be a transparent color, obviously).
    /// If not set, the different blur style styles take over.
    /// </summary>
    public Color AndroidBlurOverlayColor
    {
        get => (Color)GetValue(AndroidBlurOverlayColorProperty);
        set => SetValue(AndroidBlurOverlayColorProperty, value);
    }

    /// <summary>
    /// Android only.
    /// Changes the blur radius on Android.
    /// If set, it takes precedence over MaterialBlurStyle.
    /// If not set, the different blur style styles take over.
    /// </summary>
    public float AndroidBlurRadius
    {
        get => (float)GetValue(AndroidBlurRadiusProperty);
        set => SetValue(AndroidBlurRadiusProperty, value);
    }
}

#endif