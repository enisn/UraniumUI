using Microsoft.Maui.Controls.Shapes;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using UraniumUI.Extensions;
using UraniumUI.Options;
using UraniumUI.Pages;
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
    private readonly List<IView> appliedAttachments = new();
    private INotifyCollectionChanged subscribedAttachments;
    private bool isTemplateApplied;
    private string generatedSemanticDescription;
    private string generatedSemanticHint;
    private string explicitSemanticHint;
    private string validationSemanticMessage;

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

            inputField.UpdateContentSemantics();
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

    public static readonly BindableProperty AttachmentsProperty = BindableProperty.Create(
        nameof(Attachments),
        typeof(object),
        typeof(InputField),
        defaultValueCreator: _ => new ObservableCollection<IView>(),
        validateValue: (_, value) => value is null || value is IList<IView> || value is IView || value is IEnumerable<IView>,
        propertyChanged: (bindable, oldValue, newValue) =>
        {
            if (bindable is not InputField inputField)
            {
                return;
            }

            inputField.UnsubscribeFromAttachments(oldValue);
            inputField.SubscribeToAttachments(newValue);
            inputField.ApplyAttachments();
            inputField.OnPropertyChanged(nameof(Attachments));
        },
        coerceValue: (_, value) => value switch
        {
            null => null,
            IList<IView> => value,
            IView attachment => new ObservableCollection<IView> { attachment },
            IEnumerable<IView> attachments => new ObservableCollection<IView>(attachments.Where(attachment => attachment is not null)),
            _ => value,
        });

    private IList<IView> BindableAttachments => GetValue(AttachmentsProperty) as IList<IView>;

    public IList<IView> Attachments { get => endIconsContainer?.Children ?? BindableAttachments; set => SetValue(AttachmentsProperty, value); }

    private Color LastFontimageColor;

    /// <summary>
    /// The font icon whose color this field owns, i.e. one that had no explicit color of its own
    /// and was given the theme-aware default. Tracked so focus changes can restore that default
    /// as an app theme binding rather than a color frozen at focus time.
    /// </summary>
    private FontImageSource themedIcon;

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
        AutomationProperties.SetIsInAccessibleTree(labelTitle, false);

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
        SubscribeToAttachments(BindableAttachments);

        InitializeValidation();
    }

    private void SubscribeToAttachments(object attachments)
    {
        if (attachments is not INotifyCollectionChanged collectionChanged || ReferenceEquals(collectionChanged, subscribedAttachments))
        {
            return;
        }

        collectionChanged.CollectionChanged += Attachments_CollectionChanged;
        subscribedAttachments = collectionChanged;
    }

    private void UnsubscribeFromAttachments(object attachments)
    {
        if (attachments is not INotifyCollectionChanged collectionChanged)
        {
            return;
        }

        collectionChanged.CollectionChanged -= Attachments_CollectionChanged;

        if (ReferenceEquals(collectionChanged, subscribedAttachments))
        {
            subscribedAttachments = null;
        }
    }

    private void Attachments_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        ApplyAttachments();
    }

    private void ApplyAttachments()
    {
        var currentEndIconsContainer = endIconsContainer;
        if (currentEndIconsContainer is null)
        {
            return;
        }

        foreach (var attachment in appliedAttachments.ToArray())
        {
            currentEndIconsContainer.Remove(attachment);
        }

        appliedAttachments.Clear();

        if (BindableAttachments is null)
        {
            return;
        }

        foreach (var attachment in BindableAttachments.Where(attachment => attachment is not null))
        {
            if (!currentEndIconsContainer.Contains(attachment))
            {
                currentEndIconsContainer.Add(attachment);
            }

            appliedAttachments.Add(attachment);
        }
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

        var isThemedIcon = ReferenceEquals(fontImageSource, themedIcon);

        fontImageSource.Color = null;

        Dispatcher.Dispatch(() =>
        {
            // Same reasoning as Content_Unfocused: an icon whose color this field owns has to get
            // its app theme binding back, not the plain color captured when it was focused.
            if (isThemedIcon)
            {
                ApplyDefaultIconColor(fontImageSource);
            }
            else
            {
                fontImageSource.Color = LastFontimageColor;
            }
        });
    }
#endif

    public new bool Focus()
    {
        return Content?.Focus() ?? base.Focus();
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
            // Restoring the captured color as a plain value would clear the app theme binding
            // that Content_Focused overwrote, freezing the icon at the theme it was focused in.
            if (ReferenceEquals(fontImageSource, themedIcon))
            {
                ApplyDefaultIconColor(fontImageSource);
            }
            else
            {
                fontImageSource.Color = LastFontimageColor;
            }
        }
    }

    /// <summary>
    /// Applies the theme-aware default color to a font icon as an app theme binding, so it keeps
    /// following light/dark changes for as long as the field owns that icon's color.
    /// </summary>
    protected virtual void ApplyDefaultIconColor(FontImageSource fontImageSource)
    {
        fontImageSource.SetAppThemeColor(
            FontImageSource.ColorProperty,
            ColorResource.GetColor("OnBackground", Colors.Gray),
            ColorResource.GetColor("OnBackgroundDark", Colors.Gray));
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

    /// <summary>
    /// Re-applies the accent color when a focused field's accent changes.
    /// </summary>
    protected virtual void OnAccentColorChanged()
    {
        if (Content?.IsFocused != true)
        {
            return;
        }

        if (border is not null)
        {
            border.Stroke = AccentColor;
        }

        if (labelTitle is not null)
        {
            labelTitle.TextColor = AccentColor;
        }

        if (Icon is FontImageSource fontImageSource)
        {
            fontImageSource.Color = AccentColor;
        }
    }

    protected virtual bool IsContentReadOnly => false;

    protected virtual void UpdateContentSemantics()
    {
        UpdateContentSemanticDescription();
        UpdateContentSemanticHint();
    }

    protected virtual void SetValidationSemanticMessage(string message)
    {
        validationSemanticMessage = NormalizeSemanticText(message);
        UpdateContentSemanticHint();
    }

    protected virtual void AnnounceValidationMessage(string message)
    {
        var normalizedMessage = NormalizeSemanticText(message);

        if (string.IsNullOrWhiteSpace(normalizedMessage))
        {
            return;
        }

        try
        {
            SemanticScreenReader.Announce(AccessibilityOptions.FormatValidationErrorHint(normalizedMessage));
        }
        catch (Exception)
        {
            // Unit tests and headless hosts can run without native screen-reader services.
        }
    }

    protected static void SetActionSemantics(VisualElement element, string description, string hint)
    {
        SemanticProperties.SetDescription(element, description);
        SemanticProperties.SetHint(element, hint);
    }

    protected UraniumUIAccessibilityOptions AccessibilityOptions => GetAccessibilityOptions();

    internal static UraniumUIAccessibilityOptions GetAccessibilityOptions() => AccessibilityOptionsProvider.Get();

    private void UpdateContentSemanticDescription()
    {
        if (Content is null)
        {
            return;
        }

        var description = GetTitleSemanticText();
        var currentDescription = SemanticProperties.GetDescription(Content);

        if (string.IsNullOrWhiteSpace(description))
        {
            if (currentDescription == generatedSemanticDescription)
            {
                SemanticProperties.SetDescription(Content, null);
            }

            generatedSemanticDescription = null;
            return;
        }

        if (string.IsNullOrEmpty(currentDescription) || currentDescription == generatedSemanticDescription)
        {
            SemanticProperties.SetDescription(Content, description);
            generatedSemanticDescription = description;
        }
    }

    private string GetTitleSemanticText()
    {
        if (!string.IsNullOrWhiteSpace(Title))
        {
            return Title;
        }

        if (TitleFormattedText is null)
        {
            return null;
        }

        var text = string.Concat(TitleFormattedText.Spans.Select(span => span.Text));

        return string.IsNullOrWhiteSpace(text) ? null : text;
    }

    private void UpdateContentSemanticHint()
    {
        if (Content is null)
        {
            return;
        }

        var currentHint = SemanticProperties.GetHint(Content);
        if (!string.IsNullOrEmpty(currentHint) && currentHint != generatedSemanticHint)
        {
            explicitSemanticHint = currentHint;
        }

        var generatedStateHints = GetGeneratedStateHints().ToArray();
        if (generatedStateHints.Length == 0)
        {
            if (currentHint == generatedSemanticHint)
            {
                SemanticProperties.SetHint(Content, explicitSemanticHint);
            }

            generatedSemanticHint = null;
            return;
        }

        var hint = string.Join(" ", new[] { explicitSemanticHint }.Concat(generatedStateHints).Where(value => !string.IsNullOrWhiteSpace(value)));

        if (string.IsNullOrEmpty(currentHint) || currentHint == generatedSemanticHint || !string.IsNullOrEmpty(explicitSemanticHint))
        {
            SemanticProperties.SetHint(Content, hint);
            generatedSemanticHint = hint;
        }
    }

    private IEnumerable<string> GetGeneratedStateHints()
    {
        if (!string.IsNullOrWhiteSpace(validationSemanticMessage))
        {
            yield return AccessibilityOptions.FormatValidationErrorHint(validationSemanticMessage);
        }

        if (IsContentReadOnly && !string.IsNullOrWhiteSpace(AccessibilityOptions.ReadOnlyHint))
        {
            yield return AccessibilityOptions.ReadOnlyHint;
        }

        if (!IsEnabled && !string.IsNullOrWhiteSpace(AccessibilityOptions.DisabledHint))
        {
            yield return AccessibilityOptions.DisabledHint;
        }
    }

    private static string NormalizeSemanticText(string text)
    {
        return string.IsNullOrWhiteSpace(text)
            ? null
            : string.Join(" ", text.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).Select(part => part.Trim()));
    }

    protected override void OnPropertyChanged(string propertyName = null)
    {
        base.OnPropertyChanged(propertyName);

        if (propertyName == IsEnabledProperty.PropertyName)
        {
            UpdateContentSemanticHint();
        }
    }

    private void InputField_SizeChanged(object sender, EventArgs e)
    {
        InitializeBorder();
    }

    private void ApplyTitleFormattedText()
    {
        var currentLabelTitle = labelTitle;
        if (currentLabelTitle is null)
        {
            return;
        }

        if (TitleFormattedText is null)
        {
            currentLabelTitle.FormattedText = null;
            currentLabelTitle.SetBinding(Label.TextProperty, GetRelativeBinding(nameof(Title)));
            return;
        }

        currentLabelTitle.RemoveBinding(Label.TextProperty);
        currentLabelTitle.Text = null;
        currentLabelTitle.FormattedText = TitleFormattedText;
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        isTemplateApplied = true;

        ResetTemplateParts();

        ApplyTitleFormattedText();
        ApplyAttachments();
        UpdateContentSemantics();

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

            // TODO: Add IconColor bindable property.??? What if it's not FontImage?
            // Re-theme an icon we already own even though its color is no longer transparent,
            // otherwise reassigning the same source would hand its color back to the caller.
            if (Icon is FontImageSource font && (font.Color.IsNullOrTransparent() || ReferenceEquals(font, themedIcon)))
            {
                themedIcon = font;
                ApplyDefaultIconColor(font);
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

    /// <summary>
    /// Creates the X-shaped clear icon Path used by the built-in clear attachments.
    /// The fill is wired through an AppTheme binding so it follows runtime light/dark
    /// theme switches instead of being captured once at creation time. Shape.Fill is a
    /// Brush-typed property, so SolidColorBrush instances are provided per theme.
    /// </summary>
    protected static Microsoft.Maui.Controls.Shapes.Path CreateClearIconPath(string styleClass)
    {
        var path = new Microsoft.Maui.Controls.Shapes.Path
        {
            Data = UraniumShapes.X,
        };

        if (!string.IsNullOrEmpty(styleClass))
        {
            path.StyleClass = new[] { styleClass };
        }

        path.SetAppTheme(
            Microsoft.Maui.Controls.Shapes.Path.FillProperty,
            new SolidColorBrush(ColorResource.GetColor("OnBackground", Colors.DarkGray).WithAlpha(.5f)),
            new SolidColorBrush(ColorResource.GetColor("OnBackgroundDark", Colors.DarkGray).WithAlpha(.5f)));

        return path;
    }

    #region BindableProperties
    public string Title { get => (string)GetValue(TitleProperty); set => SetValue(TitleProperty, value); }

    public static readonly BindableProperty TitleProperty = BindableProperty.Create(
        nameof(Title),
        typeof(string),
        typeof(InputField),
        string.Empty,
        propertyChanged: (bo, ov, nv) =>
        {
            var inputField = bo as InputField;
            inputField.InitializeBorder();
            inputField.UpdateContentSemanticDescription();
        });

    public FormattedString TitleFormattedText { get => (FormattedString)GetValue(TitleFormattedTextProperty); set => SetValue(TitleFormattedTextProperty, value); }

    public static readonly BindableProperty TitleFormattedTextProperty = BindableProperty.Create(
        nameof(TitleFormattedText),
        typeof(FormattedString),
        typeof(InputField),
        default(FormattedString),
        propertyChanged: (bo, ov, nv) =>
        {
            var inputField = bo as InputField;
            inputField.ApplyTitleFormattedText();
            inputField.InitializeBorder();
            inputField.UpdateContentSemanticDescription();
        });

    public Color AccentColor { get => (Color)GetValue(AccentColorProperty); set => SetValue(AccentColorProperty, value); }

    public static readonly BindableProperty AccentColorProperty = BindableProperty.Create(
        nameof(AccentColor),
        typeof(Color),
        typeof(InputField),
        ColorResource.GetColor("Primary", "PrimaryDark", Colors.Purple),
        propertyChanged: (bindable, oldValue, newValue) => (bindable as InputField)?.OnAccentColorChanged());

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
