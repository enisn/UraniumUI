using Microsoft.Maui.Controls.Shapes;
using System.ComponentModel;
using UraniumUI.Extensions;
using UraniumUI.Resources;
using UraniumUI.ViewExtensions;

namespace UraniumUI.Material.Controls;

[ContentProperty(nameof(Content))]
public partial class InputField : ContentView
{
    internal const double FirstDash = 6;
    internal const double MaxCornerRadius = 24;
    public virtual new View Content { get => (View)GetValue(ContentProperty); set => SetValue(ContentProperty, value); }

    public static readonly new BindableProperty ContentProperty = BindableProperty.Create(
        nameof(Content),
        typeof(View),
        typeof(InputField),
        propertyChanged: (bindable, oldValue, newValue) =>
        {
            if (bindable is not InputField inputField)
            {
                return;
            }

            if (oldValue is not null)
            {
                inputField.ReleaseEvents();
            }

            if (newValue is not null)
            {
                inputField.RegisterForEvents();
            }

            inputField.OnPropertyChanged(nameof(Content));
        }, defaultBindingMode: BindingMode.TwoWay);

    protected Label labelTitle => this.FindByViewQueryIdInVisualTreeDescendants<Label>("TitleLabel");

    protected Border border => this.FindByViewQueryIdInVisualTreeDescendants<Border>("Border");

    protected Grid rootGrid => this.FindByViewQueryIdInVisualTreeDescendants<Grid>("RootGrid");

    protected Grid innerGrid => this.FindByViewQueryIdInVisualTreeDescendants<Grid>("InnerGrid");

    protected Lazy<Image> imageIcon = new Lazy<Image>(() =>
    {
        return new Image
        {
            StyleClass = new[] { "InputField.Icon" },
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            WidthRequest = 20,
            HeightRequest = 20,
            Margin = new Thickness(10, 0, 0, 0),
        };
    });

    protected HorizontalStackLayout endIconsContainer => this.FindByViewQueryIdInVisualTreeDescendants<HorizontalStackLayout>("EndIconsContainer");

    public IList<IView> Attachments => endIconsContainer.Children;

    private Color LastFontimageColor;

    private bool hasValue;

    private static readonly ControlTemplate inputFieldControlTemplate = new ControlTemplate(() =>
    {
        var @this = new Grid
        {
            Padding = new Thickness(0, 5, 0, 0),
        };
        @this.SetId("RootGrid");
        @this.SetBinding(Grid.BindingContextProperty, new Binding(".", source: new RelativeBindingSource(RelativeBindingSourceMode.TemplatedParent)));

        @this.AddRowDefinition(new RowDefinition(GridLength.Auto));
        @this.AddRowDefinition(new RowDefinition(GridLength.Auto));

        var roundRect = new RoundRectangle();
        roundRect.CornerRadius = (double)InputField.CornerRadiusProperty.DefaultValue;
        roundRect.SetBinding(RoundRectangle.StrokeProperty, new Binding(nameof(InputField.BorderColor)));
        roundRect.SetBinding(RoundRectangle.StrokeThicknessProperty, new Binding(nameof(InputField.BorderThickness)));
        roundRect.SetBinding(RoundRectangle.BackgroundProperty, new Binding(nameof(InputField.InputBackground)));
        roundRect.SetBinding(RoundRectangle.BackgroundColorProperty, new Binding(nameof(InputField.InputBackgroundColor)));

        var border = new Border
        {
            StyleClass = new[] { "InputField.Border" },
            StrokeThickness = 1,
            StrokeDashOffset = 0,
            BackgroundColor = Colors.Transparent,
            StrokeShape = roundRect,
            ZIndex = 0,
        };
        border.SetId("Border");

        @this.Add(border);

        var labelTitle = new Label()
        {
            StyleClass = new[] { "InputField.Title" },
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Start,
            InputTransparent = true,
            Margin = 15,
            ZIndex = 1000,
        };



        labelTitle.SetBinding(Label.TextColorProperty, new Binding(nameof(TitleColor)));
        labelTitle.SetId("TitleLabel");
        labelTitle.Scale = 1;
        labelTitle.SetBinding(Label.TextProperty, new Binding(nameof(Title)));
        labelTitle.SetBinding(Label.FontSizeProperty, new Binding(nameof(TitleFontSize)));
        labelTitle.SetBinding(Label.FontAttributesProperty, new Binding(nameof(FontAttributes)));
        labelTitle.SetBinding(Label.FontFamilyProperty, new Binding(nameof(FontFamily)));
        labelTitle.SetBinding(Label.FontSizeProperty, new Binding(nameof(FontSize)));
        labelTitle.SetBinding(Label.FontAutoScalingEnabledProperty, new Binding(nameof(FontAutoScalingEnabled)));

        @this.Add(labelTitle);

        var innerGrid = new Grid();
        innerGrid.SetId("InnerGrid");

        border.Content = innerGrid;
        innerGrid.AddColumnDefinition(new ColumnDefinition(GridLength.Auto));
        innerGrid.AddColumnDefinition(new ColumnDefinition(GridLength.Star));
        innerGrid.AddColumnDefinition(new ColumnDefinition(GridLength.Auto));
        innerGrid.AddRowDefinition(new RowDefinition(GridLength.Star));

        var contentHolder = new ContentView();
        contentHolder.SetBinding(ContentView.ContentProperty, new Binding(nameof(InputField.Content), source: new RelativeBindingSource(RelativeBindingSourceMode.TemplatedParent)));

        innerGrid.Add(contentHolder, column: 1);

        var endIconsContainer = new HorizontalStackLayout
        {
            StyleClass = new[] { "InputField.Attachments" },
        };

        endIconsContainer.SetId("EndIconsContainer");

        innerGrid.Add(endIconsContainer, column: 2);

        return @this;
    });

    public InputField()
    {
        this.ControlTemplate = inputFieldControlTemplate;

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

#if !WINDOWS
    protected override void OnHandlerChanged()
    {
        base.OnHandlerChanged();

#if ANDROID
        Loaded += OnLoaded;
#endif
#if MACCATALYST
        if (OperatingSystem.IsIOSVersionAtLeast(15) && Content.Handler.PlatformView is UIKit.UITextField textview)
        {
            textview.FocusEffect = null;
        }
#endif

        Content.Focused += OnFocusChanged;
        Content.Unfocused += OnFocusChanged;

        if (Handler is null)
        {
            Content.Focused -= OnFocusChanged;
            Content.Unfocused -= OnFocusChanged;
#if ANDROID
            Loaded -= OnLoaded;
#endif
        }
    }

    protected virtual void OnFocusChanged(object sender, FocusEventArgs args)
    {
        (this.rootGrid as IGridLayout).IsFocused = args.IsFocused;
    }
#endif

#if ANDROID
    // Android icon loading fix.
    protected virtual void OnLoaded(object sender, EventArgs e)
    {
        AlignIconColor();
    }

    void AlignIconColor()
    {
        if (Icon is not FontImageSource fontImageSource || LastFontimageColor.IsNullOrTransparent())
        {
            return;
        }

        fontImageSource.Color = null;

        Dispatcher.Dispatch(() =>
        {
            fontImageSource.Color = LastFontimageColor;
        });
    }
#endif

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
        if (labelTitle is null)
        {
            return;
        }

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

        border.Padding = 0;
        border.Stroke = BorderColor;
        border.StrokeThickness = BorderThickness;
        border.Background = InputBackground;
        border.BackgroundColor = InputBackgroundColor;
        border.StrokeDashOffset = 0;
        border.StrokeShape = new RoundRectangle
        {
            CornerRadius = CornerRadius
        };
#endif

        border.StrokeDashArray = new DoubleCollection { calculatedFirstDash * 0.9 / BorderThickness, space / BorderThickness, perimeter, 0 };

        UpdateState();
    }

    protected virtual void UpdateState()
    {
        if (Content is null)
        {
            return;
        }

        if (border.StrokeDashArray == null || border.StrokeDashArray.Count == 0 || labelTitle.Width <= 0)
        {
            return;
        }
        using (border.Batch())
        using (labelTitle.Batch())
        {
            if (HasValue || Content.IsFocused)
            {
                var x = CornerRadius.Clamp(10, MaxCornerRadius) - 10;

                UpdateOffset(0.01);

                labelTitle.AnchorX = 0;

                if (HasValue)
                {
                    labelTitle.TranslationX = x;
                    labelTitle.TranslationY = -25;
                    labelTitle.Scale = .8;
                }
                else
                {
                    labelTitle.TranslateToSafely(x, -25, 90, Easing.BounceOut);
                    labelTitle.ScaleToSafely(.8, 90);
                }

#if ANDROID
                if (this.IsRtl())
                {
                    labelTitle.AnchorX = .5;
                }
#endif
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

                labelTitle.AnchorX = 0;
                labelTitle.TranslateToSafely(x, 0, 90, Easing.BounceOut);
                labelTitle.ScaleToSafely(1, 90);
            }
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
            LastFontimageColor = fontImageSource.Color?.WithAlpha(1); // To create a new instance.
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

        if (!innerGrid.Contains(imageIcon.Value))
        {
            innerGrid.Add(imageIcon.Value, column: 0);
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
        string.Empty);

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
        ColorResource.GetColor("OnBackground", "OnBackgroundDark", Colors.Gray)
        );

    public Color BorderColor { get => (Color)GetValue(BorderColorProperty); set => SetValue(BorderColorProperty, value); }

    public static readonly BindableProperty BorderColorProperty = BindableProperty.Create(
        nameof(BorderColor),
        typeof(Color),
        typeof(InputField),
        ColorResource.GetColor("OnBackground", "OnBackgroundDark", Colors.Gray));

    public double BorderThickness { get => (double)GetValue(BorderThicknessProperty); set => SetValue(BorderThicknessProperty, value); }

    public static readonly BindableProperty BorderThicknessProperty = BindableProperty.Create(
        nameof(BorderThickness),
        typeof(double),
        typeof(InputField),
        1.0);

    public Color InputBackgroundColor { get => (Color)GetValue(InputBackgroundColorProperty); set => SetValue(InputBackgroundColorProperty, value); }

    public static readonly BindableProperty InputBackgroundColorProperty = BindableProperty.Create(
        nameof(InputBackgroundColor),
        typeof(Color),
        typeof(InputField),
        Colors.Transparent);

    public Brush InputBackground { get => (Brush)GetValue(InputBackgroundProperty); set => SetValue(InputBackgroundProperty, value); }

    public static readonly BindableProperty InputBackgroundProperty = BindableProperty.Create(
        nameof(InputBackground),
        typeof(Brush),
        typeof(InputField),
        Brush.Transparent);

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
        defaultValue: Label.FontSizeProperty.DefaultValue
        );

    public FontAttributes FontAttributes { get => (FontAttributes)GetValue(FontAttributesProperty); set => SetValue(FontAttributesProperty, value); }

    public static readonly BindableProperty FontAttributesProperty = BindableProperty.Create(
       nameof(FontAttributes), typeof(FontAttributes), typeof(InputField),
       defaultValue: Label.FontAttributesProperty.DefaultValue);

    public string FontFamily { get => (string)GetValue(FontFamilyProperty); set => SetValue(FontFamilyProperty, value); }

    public static readonly BindableProperty FontFamilyProperty = BindableProperty.Create(
         nameof(FontFamily), typeof(string), typeof(InputField),
         defaultValue: Label.FontFamilyProperty.DefaultValue);


    [TypeConverter(typeof(FontSizeConverter))]
    public double FontSize { get => (double)GetValue(FontSizeProperty); set => SetValue(FontSizeProperty, value); }

    public static readonly BindableProperty FontSizeProperty = BindableProperty.Create(
        nameof(FontSize), typeof(double), typeof(PickerField), Picker.FontSizeProperty.DefaultValue,
        propertyChanged: (bindable, oldValue, newValue) => (bindable as PickerField).PickerView.FontSize = (double)newValue);

    public bool FontAutoScalingEnabled { get => (bool)GetValue(FontAutoScalingEnabledProperty); set => SetValue(FontAutoScalingEnabledProperty, value); }

    public static readonly BindableProperty FontAutoScalingEnabledProperty = BindableProperty.Create(
        nameof(FontAutoScalingEnabled), typeof(bool), typeof(PickerField), Picker.FontAutoScalingEnabledProperty.DefaultValue,
        propertyChanged: (bindable, oldValue, newValue) => (bindable as PickerField).PickerView.FontAutoScalingEnabled = (bool)newValue);

    public string ContentAutomationId { get => (string)GetValue(ContentAutomationIdProperty); set => SetValue(ContentAutomationIdProperty, value); }

    public static readonly BindableProperty ContentAutomationIdProperty = BindableProperty.Create(
        nameof(ContentAutomationId),
        typeof(string),
        typeof(InputField),
        null,
        propertyChanged: (bindable, oldValue, newValue) =>
        {
            if (bindable is InputField inputField && inputField.Content != null)
            {
                inputField.Content.AutomationId = newValue as string;
            }
        });
    #endregion
}
