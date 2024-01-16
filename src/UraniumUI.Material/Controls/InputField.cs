using Microsoft.Maui.Controls.Shapes;
using UraniumUI.Resources;
using UraniumUI.Extensions;
using System.Collections;

namespace UraniumUI.Material.Controls;

[ContentProperty(nameof(Content))]
public partial class InputField : Grid
{
    internal const double FirstDash = 6;
    internal const double MaxCornerRadius = 24;
    private View content;
    public virtual View Content
    {
        get => content;
        set
        {
            if (content is not null)
            {
                ReleaseEvents();
            }
            content = value;
            border.Content = content;

            if (value != null)
            {
                RegisterForEvents();
            }
        }
    }

    protected Label labelTitle = new Label()
    {
        Text = "Title",
        HorizontalOptions = LayoutOptions.Start,
        VerticalOptions = LayoutOptions.Start,
        InputTransparent = true,
        Margin = 15,
    };

    protected Border border = new Border
    {
        Padding = 0,
        StrokeThickness = 1,
        StrokeDashOffset = 0,
        BackgroundColor = Colors.Transparent,
    };

    protected Grid rootGrid = new Grid();

    protected Lazy<Image> imageIcon = new Lazy<Image>(() =>
    {
        return new Image
        {
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            WidthRequest = 20,
            HeightRequest = 20,
            Margin = new Thickness(10, 0, 0, 0),
        };
    });

    protected HorizontalStackLayout endIconsContainer = new HorizontalStackLayout();

    public IList<IView> Attachments => endIconsContainer.Children;

    private Color LastFontimageColor;

    private bool hasValue;

    public InputField()
    {
        this.Padding = new Thickness(0, 5, 0, 0);
        border.StrokeShape = new RoundRectangle
        {
            CornerRadius = this.CornerRadius,
            Stroke = this.BorderColor,
            StrokeThickness = this.BorderThickness,
            Background = this.InputBackground,
            BackgroundColor = this.InputBackgroundColor,
        };

        RegisterForEvents();

        border.ZIndex = 0;
        labelTitle.ZIndex = 1000;

        this.Add(border);
        this.Add(labelTitle);

        border.Content = rootGrid;

        rootGrid.AddColumnDefinition(new ColumnDefinition(GridLength.Auto));
        rootGrid.AddColumnDefinition(new ColumnDefinition(GridLength.Star));
        rootGrid.AddColumnDefinition(new ColumnDefinition(GridLength.Auto));
        rootGrid.AddRowDefinition(new RowDefinition(GridLength.Star));

        if (Content != null)
        {
            rootGrid.Add(Content, column: 1);
        }

        rootGrid.Add(endIconsContainer, column: 2);

        labelTitle.Scale = 1;
        labelTitle.SetBinding(Label.TextProperty, new Binding(nameof(Title), source: this));

        InitializeValidation();
    }

    public virtual bool HasValue
    {
        get => hasValue;
        set
        {
            hasValue = value;
            UpdateState();
        }
    }

    protected override void OnHandlerChanging(HandlerChangingEventArgs args)
    {
        base.OnHandlerChanging(args);

        if (args.NewHandler is null)
        {
            ReleaseEvents();
        }
    }

    protected override async void OnSizeAllocated(double width, double height)
    {
        try
        {
            base.OnSizeAllocated(width, height);
            await Task.Delay(100);
            InitializeBorder();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in {nameof(InputField)} - OnSizeAllocated: {ex}");
        }
    }

    protected override void OnHandlerChanged()
    {
        base.OnHandlerChanged();

        Content.Focused += OnFocusChanged;
        Content.Unfocused += OnFocusChanged;

        if (Handler is null)
        {
            Content.Focused -= OnFocusChanged;
            Content.Unfocused -= OnFocusChanged;
        }
    }

    protected virtual void OnFocusChanged(object sender, FocusEventArgs args)
    {
#if !WINDOWS
        (this as IGridLayout).IsFocused = args.IsFocused;
#endif
    }

    // TODO: Remove this member hiding after android unfocus fixed.
    public new void Unfocus()
    {
        base.Unfocus();
#if ANDROID
        var view = Content.Handler.PlatformView as Android.Views.View;

        view?.ClearFocus();
#endif
    }

    private void InitializeBorder()
    {
        var perimeter = (this.Width + this.Height) * 2;
        var calculatedFirstDash = FirstDash + CornerRadius.Clamp(FirstDash, double.MaxValue);

        var space = (labelTitle.Width + calculatedFirstDash) * .8;
        if (labelTitle.Width <= 0)
            space = 0;

#if ANDROID
        if (this.IsRtl())
        {
            calculatedFirstDash += this.Width - labelTitle.Width;
        }
#endif

#if WINDOWS
        if (space <= 0 || perimeter <= 0)
        {
            return;
        }

        border.Content = null;
        this.Remove(border);
        border = new Border
        {
            Padding = 0,
            Stroke = BorderColor,
            StrokeThickness = BorderThickness,
            Background = InputBackground,
            BackgroundColor = InputBackgroundColor,
            StrokeDashOffset = 0,
            StrokeShape = new RoundRectangle
            {
                CornerRadius = CornerRadius
            },
            Content = rootGrid
        };
#endif

        border.StrokeDashArray = new DoubleCollection { calculatedFirstDash * 0.9, space, perimeter, 0 };

#if WINDOWS
        this.Add(border);
#endif

        UpdateState();
        border.StrokeThickness = BorderThickness;
    }

    protected virtual void UpdateState()
    {
        if (border.StrokeDashArray == null || border.StrokeDashArray.Count == 0 || labelTitle.Width <= 0)
        {
            return;
        }

        if (HasValue || Content.IsFocused)
        {
            var x = CornerRadius.Clamp(10, MaxCornerRadius) - 10;

            UpdateOffset(0.01);
            labelTitle.TranslateTo(x, -25, 90, Easing.BounceOut);
            labelTitle.AnchorX = 0;


#if ANDROID
            if (this.IsRtl())
            {
                labelTitle.AnchorX = .5;
            }
#endif

            labelTitle.ScaleTo(.8, 90);
        }
        else
        {
            var offsetToGo = border.StrokeDashArray[0] + border.StrokeDashArray[1] + FirstDash;
            UpdateOffset(offsetToGo);

            labelTitle.CancelAnimations();

            var x = imageIcon.IsValueCreated ? imageIcon.Value.Width : 0;

#if ANDROID
            if (this.IsRtl())
            {
                x = imageIcon.IsValueCreated ? -imageIcon.Value.Width : 0;
            }
#endif

            labelTitle.TranslateTo(x, 0, 90, Easing.BounceOut);

            labelTitle.AnchorX = 0;
            labelTitle.ScaleTo(1, 90);
        }
    }

    protected virtual void UpdateOffset(double value)
    {
        border.StrokeDashOffset = value;
    }

    protected virtual void RegisterForEvents()
    {
        if (Content != null)
        {
            Content.Focused -= Content_Focused;
            Content.Focused += Content_Focused;
            Content.Unfocused -= Content_Unfocused;
            Content.Unfocused += Content_Unfocused;
            SizeChanged -= InputField_SizeChanged;
            SizeChanged += InputField_SizeChanged;
        }
    }

    protected virtual void ReleaseEvents()
    {
        Content.Focused -= Content_Focused;
        Content.Unfocused -= Content_Unfocused;
        SizeChanged -= InputField_SizeChanged;
    }

    private void Content_Unfocused(object sender, FocusEventArgs e)
    {
        border.Stroke = BorderColor;
        labelTitle.TextColor = TitleColor;
        UpdateState();

        if (Icon is FontImageSource fontImageSource)
        {
            fontImageSource.Color = LastFontimageColor;
        }
    }

    private void Content_Focused(object sender, FocusEventArgs e)
    {
        border.Stroke = AccentColor;
        labelTitle.TextColor = AccentColor;
        UpdateState();

        if (Icon is FontImageSource fontImageSource && fontImageSource.Color != AccentColor)
        {
            LastFontimageColor = fontImageSource.Color?.WithAlpha(1); // To createnew instance.
            fontImageSource.Color = AccentColor;
        }
    }

    private void InputField_SizeChanged(object sender, EventArgs e)
    {
        InitializeBorder();
    }

    protected virtual void OnIconChanged()
    {
        imageIcon.Value.Source = Icon;

        if (Icon is FontImageSource font && font.Color.IsNullOrTransparent())
        {
            // TODO: Add IconColor bindable property.??? What if it's not FontImage?
            font.SetAppThemeColor(
                FontImageSource.ColorProperty,
                ColorResource.GetColor("OnBackground", Colors.Gray),
                ColorResource.GetColor("OnBackgroundDark", Colors.Gray));
        }

        if (!rootGrid.Contains(imageIcon.Value))
        {
            rootGrid.Add(imageIcon.Value, column: 0);
        }

        var leftMargin = Icon != null ? 5 : 10;
        this.Content.Margin = new Thickness(leftMargin, 0, 0, 0);

    }
    protected virtual void OnCornerRadiusChanged()
    {
        if (CornerRadius > MaxCornerRadius)
        {
            CornerRadius = MaxCornerRadius;
            return;
        }

        if (border.StrokeShape is RoundRectangle roundRectangle)
        {
            roundRectangle.CornerRadius = CornerRadius;
#if WINDOWS
            InitializeBorder();
#endif
        }
    }

    #region BindableProperties
    public string Title { get => (string)GetValue(TitleProperty); set => SetValue(TitleProperty, value); }

    public static readonly BindableProperty TitleProperty = BindableProperty.Create(
        nameof(Title),
        typeof(string),
        typeof(InputField),
        string.Empty,
        propertyChanged: (bindable, oldValue, newValue) =>
        {
            var textField = (bindable as InputField);
            textField.labelTitle.Text = (string)newValue;
            textField.InitializeBorder();
        });

    public Color AccentColor { get => (Color)GetValue(AccentColorProperty); set => SetValue(AccentColorProperty, value); }

    public static readonly BindableProperty AccentColorProperty = BindableProperty.Create(
        nameof(AccentColor),
        typeof(Color),
        typeof(InputField),
        ColorResource.GetColor("Primary", "PrimaryDark", Colors.Purple));

    public Color TitleColor { get => (Color)GetValue(TitleColorProperty); set => SetValue(TitleColorProperty, value); }

    public static readonly BindableProperty TitleColorProperty = BindableProperty.Create(
        nameof(TitleColor),
        typeof(Color),
        typeof(InputField),
        ColorResource.GetColor("OnBackground", "OnBackgroundDark", Colors.Gray),
        propertyChanged: (bindable, oldValue, newValue) => (bindable as InputField).labelTitle.TextColor = (Color)newValue
        );

    public Color BorderColor { get => (Color)GetValue(BorderColorProperty); set => SetValue(BorderColorProperty, value); }

    public static readonly BindableProperty BorderColorProperty = BindableProperty.Create(
        nameof(BorderColor),
        typeof(Color),
        typeof(InputField),
        ColorResource.GetColor("OnBackground", "OnBackgroundDark", Colors.Gray),
        propertyChanged: (bindable, oldValue, newValue) => (bindable as InputField).border.Stroke = (Color)newValue);

    public double BorderThickness { get => (double)GetValue(BorderThicknessProperty); set => SetValue(BorderThicknessProperty, value); }

    public static readonly BindableProperty BorderThicknessProperty = BindableProperty.Create(
        nameof(BorderThickness),
        typeof(double),
        typeof(InputField),
        1.0,
        propertyChanged: (bindable, oldValue, newValue) => (bindable as InputField).border.StrokeThickness = (double)newValue);

    public Color InputBackgroundColor { get => (Color)GetValue(InputBackgroundColorProperty); set => SetValue(InputBackgroundColorProperty, value); }

    public static readonly BindableProperty InputBackgroundColorProperty = BindableProperty.Create(
        nameof(InputBackgroundColor),
        typeof(Color),
        typeof(InputField),
        Colors.Transparent,
        propertyChanged: (bindable, oldValue, newValue) => (bindable as InputField).border.BackgroundColor = (Color)newValue);

    public Brush InputBackground { get => (Brush)GetValue(InputBackgroundProperty); set => SetValue(InputBackgroundProperty, value); }

    public static readonly BindableProperty InputBackgroundProperty = BindableProperty.Create(
        nameof(InputBackground),
        typeof(Brush),
        typeof(InputField),
        Brush.Transparent,
        propertyChanged: (bindable, oldValue, newValue) => (bindable as InputField).border.Background = (Brush)newValue);

    public ImageSource Icon { get => (ImageSource)GetValue(IconProperty); set => SetValue(IconProperty, value); }

    public static readonly BindableProperty IconProperty = BindableProperty.Create(
        nameof(Icon),
        typeof(ImageSource),
        typeof(InputField),
        propertyChanged: (bindable, oldValue, newValue) => (bindable as InputField).OnIconChanged());

    public double CornerRadius { get => (double)GetValue(CornerRadiusProperty); set => SetValue(CornerRadiusProperty, value); }

    public static readonly BindableProperty CornerRadiusProperty = BindableProperty.Create(
        nameof(CornerRadius),
        typeof(double),
        typeof(InputField),
        defaultValue: 8.0,
        propertyChanged: (bindable, oldValue, newValue) => (bindable as InputField).OnCornerRadiusChanged());

    [System.ComponentModel.TypeConverter(typeof(FontSizeConverter))]
    public double TitleFontSize { get => (double)GetValue(TitleFontSizeProperty); set => SetValue(TitleFontSizeProperty, value); }

    public static readonly BindableProperty TitleFontSizeProperty = BindableProperty.Create(
        nameof(TitleFontSize),
        typeof(double),
        typeof(InputField),
        defaultValue: Label.FontSizeProperty.DefaultValue,
        propertyChanged: (bindable, oldValue, newValue) => (bindable as InputField).labelTitle.FontSize = (double)newValue
        );
    #endregion
}
