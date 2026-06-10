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
    internal const double EdgePadding = 10;                  // gap to the field border (leading Icon margin / trailing Attachments margin)
    internal const double AttachmentsSpacing = 8;            // gap between sibling attachments
    internal const double BuiltInAttachmentLeftPadding = 5;  // left tap-area extension on built-in attachments toward the text content

    private Label titleLabelPart;
    private Border borderPart;
    private Grid rootGridPart;
    private Grid innerGridPart;
    private HorizontalStackLayout endIconsContainerPart;
    private bool isTemplateApplied;

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

    protected Label labelTitle => titleLabelPart ??= FindTemplatePart<Label>("TitleLabel");

    protected Border border => borderPart ??= FindTemplatePart<Border>("Border");

    protected Grid rootGrid => rootGridPart ??= FindTemplatePart<Grid>("RootGrid");

    protected Grid innerGrid => innerGridPart ??= FindTemplatePart<Grid>("InnerGrid");

    protected Lazy<Image> imageIcon = new Lazy<Image>(() =>
    {
        var image = new Image
        {
            StyleClass = new[] { "InputField.Icon" },
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            WidthRequest = 20,
            HeightRequest = 20,
            Margin = new Thickness(EdgePadding, 0, 0, 0),
        };
        image.SetId("IconImage");

        return image;
    });

    protected HorizontalStackLayout endIconsContainer => endIconsContainerPart ??= FindTemplatePart<HorizontalStackLayout>("EndIconsContainer");

    public IList<IView> Attachments => endIconsContainer?.Children;

    private Color LastFontimageColor;

    private Thickness? originalContentMargin;

    private bool hasValue;

    // Leading-icon predicate. Invariant: imageIcon is materialized, in the grid, and visible iff this is true.
    protected bool HasIcon => Icon != null;

    private static Binding GetRelativeBinding(string path, BindingMode mode = BindingMode.Default) => new Binding(path, mode: mode, source: new RelativeBindingSource(RelativeBindingSourceMode.TemplatedParent));

    private static readonly ControlTemplate inputFieldControlTemplate = new ControlTemplate(() =>
    {
        var @this = new Grid
        {
            Padding = new Thickness(0, 5, 0, 0),
        };
        @this.SetId("RootGrid");

        @this.AddRowDefinition(new RowDefinition(GridLength.Auto));
        @this.AddRowDefinition(new RowDefinition(GridLength.Auto));

        var roundRect = new RoundRectangle();
        roundRect.CornerRadius = (double)InputField.CornerRadiusProperty.DefaultValue;

        var border = new Border
        {
            StyleClass = new[] { "InputField.Border" },
            StrokeShape = roundRect,
        };
        border.SetBinding(Border.StrokeProperty, GetRelativeBinding(nameof(InputField.BorderColor)));
        border.SetBinding(Border.StrokeThicknessProperty, GetRelativeBinding(nameof(InputField.BorderThickness)));
        border.SetBinding(Border.BackgroundProperty, GetRelativeBinding(nameof(InputField.InputBackground)));
        border.SetBinding(Border.BackgroundColorProperty, GetRelativeBinding(nameof(InputField.InputBackgroundColor), BindingMode.TwoWay));
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

        labelTitle.SetBinding(Label.TextColorProperty, GetRelativeBinding(nameof(TitleColor)));
        labelTitle.SetId("TitleLabel");
        labelTitle.Scale = 1;
        labelTitle.SetBinding(Label.TextProperty, GetRelativeBinding(nameof(Title)));
        labelTitle.SetBinding(Label.FontSizeProperty, GetRelativeBinding(nameof(TitleFontSize)));
        labelTitle.SetBinding(Label.FontAttributesProperty, GetRelativeBinding(nameof(FontAttributes)));
        labelTitle.SetBinding(Label.FontFamilyProperty, GetRelativeBinding(nameof(FontFamily)));
        labelTitle.SetBinding(Label.FontAutoScalingEnabledProperty, GetRelativeBinding(nameof(FontAutoScalingEnabled)));

        @this.Add(labelTitle);

        var innerGrid = new Grid();
        innerGrid.SetId("InnerGrid");

        border.Content = innerGrid;
        innerGrid.AddColumnDefinition(new ColumnDefinition(GridLength.Auto));
        innerGrid.AddColumnDefinition(new ColumnDefinition(GridLength.Star));
        innerGrid.AddColumnDefinition(new ColumnDefinition(GridLength.Auto));
        innerGrid.AddRowDefinition(new RowDefinition(GridLength.Star));

        var contentHolder = new ContentView();
        contentHolder.SetBinding(ContentView.ContentProperty, GetRelativeBinding(nameof(InputField.Content)));

        innerGrid.Add(contentHolder, column: 1);

        var endIconsContainer = new HorizontalStackLayout
        {
            StyleClass = new[] { "InputField.Attachments" },
            Margin = new Thickness(0, 0, EdgePadding, 0),
            Spacing = AttachmentsSpacing,
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

    private T FindTemplatePart<T>(string id)
        where T : VisualElement
    {
        if (!isTemplateApplied && Handler is null)
        {
            return null;
        }

        return this.FindByViewQueryIdInVisualTreeDescendants<T>(id);
    }

    private void ResetTemplateParts()
    {
        titleLabelPart = null;
        borderPart = null;
        rootGridPart = null;
        innerGridPart = null;
        endIconsContainerPart = null;
    }

    protected override void OnHandlerChanging(HandlerChangingEventArgs args)
    {
        base.OnHandlerChanging(args);

        if (args.NewHandler is null)
        {
            isTemplateApplied = false;
            ResetTemplateParts();
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
        if (rootGrid is IGridLayout gridLayout)
        {
            gridLayout.IsFocused = args.IsFocused;
        }
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
        var currentLabelTitle = labelTitle;
        var currentBorder = border;

        if (currentLabelTitle is null || currentBorder is null)
        {
            return;
        }

        var perimeter = (this.Width + this.Height) * 2;
        var calculatedFirstDash = FirstDash + CornerRadius.Clamp(FirstDash, double.MaxValue);

        var space = (currentLabelTitle.Width + calculatedFirstDash) * .8;
        if (currentLabelTitle.Width <= 0)
            space = 0;

#if ANDROID
        if (this.IsRtl())
        {
            calculatedFirstDash += this.Width - currentLabelTitle.Width;
        }
#endif

        currentBorder.StrokeDashArray = new DoubleCollection { calculatedFirstDash * 0.9 / BorderThickness, space / BorderThickness, perimeter, 0 };

        UpdateState();
    }

    protected virtual void UpdateState()
    {
        var currentBorder = border;
        var currentLabelTitle = labelTitle;

        if (Content is null)
        {
            return;
        }

        if (currentBorder?.StrokeDashArray == null || currentBorder.StrokeDashArray.Count == 0 || currentLabelTitle is null || currentLabelTitle.Width <= 0)
        {
            return;
        }

        using (currentBorder.Batch())
        using (currentLabelTitle.Batch())
        {
            if (HasValue || Content.IsFocused)
            {
                var x = CornerRadius.Clamp(10, MaxCornerRadius) - 10;

                UpdateOffset(0.01);

                currentLabelTitle.AnchorX = 0;

                currentLabelTitle.CancelAnimations();
                if (HasValue)
                {
                    currentLabelTitle.TranslationX = x;
                    currentLabelTitle.TranslationY = -25;
                    currentLabelTitle.Scale = .8;
                }
                else
                {
                    currentLabelTitle.TranslateToSafely(x, -25, 90, Easing.BounceOut);
                    currentLabelTitle.ScaleToSafely(.8, 90);
                }

#if ANDROID
                if (this.IsRtl())
                {
                    currentLabelTitle.AnchorX = .5;
                }
#endif
            }
            else
            {
                var offsetToGo = currentBorder.StrokeDashArray[0] + currentBorder.StrokeDashArray[1] + FirstDash;
                UpdateOffset(offsetToGo);

                currentLabelTitle.CancelAnimations();

                var x = HasIcon ? imageIcon.Value.Width : 0;

#if ANDROID
                if (this.IsRtl())
                {
                    x = HasIcon ? -imageIcon.Value.Width : 0;
                }
#endif

                currentLabelTitle.AnchorX = 0;
                currentLabelTitle.TranslateToSafely(x, 0, 90, Easing.BounceOut);
                currentLabelTitle.ScaleToSafely(1, 90);
            }
        }
    }

    protected virtual void UpdateOffset(double value)
    {
        if (border is not null)
        {
            border.StrokeDashOffset = value;
        }
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
        var currentBorder = border;
        var currentLabelTitle = labelTitle;

        currentBorder?.SetBinding(Border.StrokeProperty, GetRelativeBinding(nameof(BorderColor)));
        currentLabelTitle?.SetBinding(Label.TextColorProperty, GetRelativeBinding(nameof(TitleColor)));
        UpdateState();

        if (Icon is FontImageSource fontImageSource)
        {
            fontImageSource.Color = LastFontimageColor;
        }
    }

    private void Content_Focused(object sender, FocusEventArgs e)
    {
        if (border is not null)
        {
            border.Stroke = AccentColor;
        }

        if (labelTitle is not null)
        {
            labelTitle.TextColor = AccentColor;
        }

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

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        isTemplateApplied = true;

        ResetTemplateParts();

        if (Icon != null)
        {
            OnIconChanged();
        }

        OnCornerRadiusChanged();

        if (!string.IsNullOrEmpty(ContentAutomationId) && Content != null)
        {
            Content.AutomationId = ContentAutomationId;
        }
    }

    protected virtual void OnIconChanged()
    {
        if (this.Content != null && originalContentMargin == null)
        {
            originalContentMargin = this.Content.Margin;
        }

        if (HasIcon)
        {
            imageIcon.Value.Source = Icon;
            imageIcon.Value.IsVisible = true;

            if (Icon is FontImageSource font && font.Color.IsNullOrTransparent())
            {
                // TODO: Add IconColor bindable property.??? What if it's not FontImage?
                font.SetAppThemeColor(
                    FontImageSource.ColorProperty,
                    ColorResource.GetColor("OnBackground", Colors.Gray),
                    ColorResource.GetColor("OnBackgroundDark", Colors.Gray));
            }

            if (innerGrid != null && !innerGrid.Contains(imageIcon.Value))
            {
                innerGrid.Add(imageIcon.Value, column: 0);
            }

            this.Content.Margin = new Thickness(5, 0, 0, 0);
        }
        else
        {
            if (imageIcon.IsValueCreated)
            {
                // Collapse the leading Auto column when Icon is cleared (e.g. binding goes back to null).
                imageIcon.Value.Source = null;
                imageIcon.Value.IsVisible = false;
            }

            if (this.Content != null && originalContentMargin.HasValue)
            {
                this.Content.Margin = originalContentMargin.Value;
            }
        }

        // Re-run the title-position logic so the floating Label tracks the icon's new presence/absence.
        UpdateState();
    }

    protected virtual void OnCornerRadiusChanged()
    {
        if (CornerRadius > MaxCornerRadius)
        {
            CornerRadius = MaxCornerRadius;
            return;
        }

        if (border?.StrokeShape is RoundRectangle roundRectangle)
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
        propertyChanged: (bo, ov, nv) => (bo as InputField).InitializeBorder());

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
        null);

    public Brush InputBackground { get => (Brush)GetValue(InputBackgroundProperty); set => SetValue(InputBackgroundProperty, value); }

    public static readonly BindableProperty InputBackgroundProperty = BindableProperty.Create(
        nameof(InputBackground),
        typeof(Brush),
        typeof(InputField),
        null);

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
        nameof(FontSize), typeof(double), typeof(InputField), Picker.FontSizeProperty.DefaultValue);

    public bool FontAutoScalingEnabled { get => (bool)GetValue(FontAutoScalingEnabledProperty); set => SetValue(FontAutoScalingEnabledProperty, value); }

    public static readonly BindableProperty FontAutoScalingEnabledProperty = BindableProperty.Create(
        nameof(FontAutoScalingEnabled), typeof(bool), typeof(InputField), Picker.FontAutoScalingEnabledProperty.DefaultValue,
        propertyChanged: (bindable, oldValue, newValue) =>
        {
            var titleLabel = (bindable as InputField)?.labelTitle;
            if (titleLabel != null)
            {
                titleLabel.FontAutoScalingEnabled = (bool)newValue;
            }
        });

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
