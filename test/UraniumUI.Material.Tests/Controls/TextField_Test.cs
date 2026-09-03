using FluentAssertions;
using InputKit.Shared.Validations;
using Microsoft.Maui.Handlers;
using NSubstitute;
using Shouldly;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Windows.Input;
using UraniumUI.Material.Controls;
using UraniumUI.Options;
using UraniumUI.Tests.Core;
using UraniumUI.ViewExtensions;
using UraniumUI.Views;
using Path = Microsoft.Maui.Controls.Shapes.Path;

namespace UraniumUI.Material.Tests.Controls;
public class TextField_Test
{
    public TextField_Test()
    {
        ApplicationExtensions.CreateAndSetMockApplication();
    }

    [Fact]
    public void Focus_ShouldForward_ToContent()
    {
        var control = AnimationReadyHandler.Prepare(new TextField());
        var handler = new FocusTrackingHandler();

        control.EntryView.Handler = handler;

        control.Focus();

        handler.FocusRequested.ShouldBeTrue();
    }

    [Fact]
    public void Text_BindingForInitialization_FromSource()
    {
        var control = AnimationReadyHandler.Prepare(new TextField());
        var viewModel = new TestViewModel { Text = "Text Initial Value" };
        control.BindingContext = viewModel;
        control.SetBinding(TextField.TextProperty, new Binding(nameof(TestViewModel.Text)));

        // Assert
        control.Text.ShouldBe(viewModel.Text);
    }

    [Fact]
    public void Text_Binding_FromSource()
    {
        var control = AnimationReadyHandler.Prepare(new TextField());
        var viewModel = new TestViewModel { Text = "Text Initial Value" };
        control.BindingContext = viewModel;
        control.SetBinding(TextField.TextProperty, new Binding(nameof(TestViewModel.Text)));

        // Act
        viewModel.Text = "Changed Value";

        // Assert
        control.Text.ShouldBe(viewModel.Text);
    }

    [Fact]
    public void Text_Binding_RaisesPropertyChangedEvent_ExactlyOnce()
    {
        var control = AnimationReadyHandler.Prepare(new TextField());
        var viewModel = new TestViewModel { Text = "Text Initial Value" };
        control.BindingContext = viewModel;
        control.SetBinding(TextField.TextProperty, new Binding(nameof(TestViewModel.Text)));

        var monitoredSubject = control.Monitor();
        // Act
        viewModel.Text = "Changed Value";

        // Assert
        monitoredSubject.Should().RaisePropertyChangeFor(x => x.Text).ShouldHaveSingleItem();
    }

    [Fact]
    public void Text_Binding_ToSource()
    {
        var control = AnimationReadyHandler.Prepare(new TextField());
        var viewModel = new TestViewModel { Text = "Text Initial Value" };
        control.BindingContext = viewModel;
        control.SetBinding(TextField.TextProperty, new Binding(nameof(TestViewModel.Text)));

        // Act
        control.Text = "Updated from control";

        // Assert
        viewModel.Text.ShouldBe(control.Text);
    }

    [Fact]
    public void TextProperty_Parent_ShouldTwoWayBind_Child()
    {
        var control = AnimationReadyHandler.Prepare(new TextField());

        //Test Child->Parent
        // Act
        control.EntryView.Text = "Test 1";

        // Assert
        control.Text.ShouldBe(control.EntryView.Text);

        //Test Parent->Child
        // Act
        control.Text = "Test 2";

        // Assert
        control.Text.ShouldBe(control.EntryView.Text);
    }

    [Fact]
    public void TitleFormattedText_ShouldBind_ToTitleLabel()
    {
        var formattedTitle = new FormattedString();
        formattedTitle.Spans.Add(new Span { Text = "Name", FontAttributes = FontAttributes.Bold });
        formattedTitle.Spans.Add(new Span { Text = " *", TextColor = Colors.Red });

        var control = AnimationReadyHandler.Prepare(new TextField { TitleFormattedText = formattedTitle });

        var titleLabel = control.FindByViewQueryIdInVisualTreeDescendants<Label>("TitleLabel");

        titleLabel.ShouldNotBeNull();
        titleLabel.FormattedText.ShouldBeSameAs(formattedTitle);
        titleLabel.Text.ShouldBeNull();

        control.Title = "Updated plain title";

        titleLabel.FormattedText.ShouldBeSameAs(formattedTitle);
        titleLabel.Text.ShouldBeNull();

        control.TitleFormattedText = null;

        titleLabel.FormattedText.ShouldBeNull();
        titleLabel.Text.ShouldBe(control.Title);

        control.Title = "Restored plain title";

        titleLabel.Text.ShouldBe(control.Title);
    }

    [Fact]
    public void Attachments_Container_HasDefaultSpacingFromBorderAndBetweenChildren()
    {
        // The Attachments container provides a default gap to the field border and
        // between sibling attachments. Container defaults stay independent of children.
        var control = AnimationReadyHandler.Prepare(new TextField());

        var endIconsContainer = control.FindByViewQueryIdInVisualTreeDescendants<HorizontalStackLayout>("EndIconsContainer");

        endIconsContainer.ShouldNotBeNull();
        endIconsContainer.Margin.ShouldBe(new Thickness(0, 0, InputField.EdgePadding, 0));
        endIconsContainer.Spacing.ShouldBe(InputField.AttachmentsSpacing);

        var first = new ActivityIndicator { IsRunning = true };
        var second = new ActivityIndicator { IsRunning = true };
        control.Attachments.Add(first);
        control.Attachments.Add(second);

        endIconsContainer.Margin.ShouldBe(new Thickness(0, 0, InputField.EdgePadding, 0));
        endIconsContainer.Spacing.ShouldBe(InputField.AttachmentsSpacing);
        control.Attachments.ShouldContain(first);
        control.Attachments.ShouldContain(second);
        endIconsContainer.Children.ShouldContain(first);
        endIconsContainer.Children.ShouldContain(second);
    }

    [Fact]
    public void Attachments_AddedBeforeTemplateApplied_ShouldBeRenderedAfterTemplateApplied()
    {
        var attachment = new ActivityIndicator { IsRunning = true };
        var control = new TextField();

        control.Attachments.Add(attachment);

        AnimationReadyHandler.Prepare(control);

        var endIconsContainer = control.FindByViewQueryIdInVisualTreeDescendants<HorizontalStackLayout>("EndIconsContainer");

        endIconsContainer.ShouldNotBeNull();
        endIconsContainer.Children.ShouldContain(attachment);
    }

    [Fact]
    public void Attachments_CanBeSetViaImplicitStyle()
    {
        var attachment = new ActivityIndicator { IsRunning = true };
        var style = new Style(typeof(TextField));
        style.Setters.Add(new Setter
        {
            Property = InputField.AttachmentsProperty,
            Value = attachment,
        });

        var resources = new ResourceDictionary();
        resources.Add(style);

        var control = AnimationReadyHandler.Prepare(new TextField());

        control.Resources = resources;

        var endIconsContainer = control.FindByViewQueryIdInVisualTreeDescendants<HorizontalStackLayout>("EndIconsContainer");

        endIconsContainer.ShouldNotBeNull();
        control.Attachments.ShouldContain(attachment);
        endIconsContainer.Children.ShouldContain(attachment);
    }

    [Fact]
    public void Attachments_CanBeBoundToCollection()
    {
        var attachment = new ActivityIndicator { IsRunning = true };
        var viewModel = new TestViewModel
        {
            Attachments = new ObservableCollection<IView> { attachment },
        };
        var control = new TextField
        {
            BindingContext = viewModel,
        };

        control.SetBinding(InputField.AttachmentsProperty, new Binding(nameof(TestViewModel.Attachments)));

        AnimationReadyHandler.Prepare(control);

        var endIconsContainer = control.FindByViewQueryIdInVisualTreeDescendants<HorizontalStackLayout>("EndIconsContainer");

        endIconsContainer.ShouldNotBeNull();
        endIconsContainer.Children.ShouldContain(attachment);
    }

    [Fact]
    public void Attachments_ReplacedAfterTemplateApplied_ShouldPreserveBuiltInClearIcon()
    {
        var control = AnimationReadyHandler.Prepare(new TextField { AllowClear = true });
        var clearIcon = control.FindByViewQueryIdInVisualTreeDescendants<StatefulContentView>("ClearIcon");
        var attachment = new ActivityIndicator { IsRunning = true };

        control.Attachments = new ObservableCollection<IView> { attachment };

        var endIconsContainer = control.FindByViewQueryIdInVisualTreeDescendants<HorizontalStackLayout>("EndIconsContainer");

        clearIcon.ShouldNotBeNull();
        endIconsContainer.ShouldNotBeNull();
        endIconsContainer.Children.ShouldContain(clearIcon);
        endIconsContainer.Children.ShouldContain(attachment);
    }

    [Fact]
    public void ClearIcon_HasAsymmetricLeftHitPadding()
    {
        // Clear X uses asymmetric Padding: right edge flush with the container margin
        // (matching a user-supplied attachment), left side extended for a wider tap target.
        var control = AnimationReadyHandler.Prepare(new TextField { AllowClear = true });
        var clearIcon = control.FindByViewQueryIdInVisualTreeDescendants<StatefulContentView>("ClearIcon");

        clearIcon.ShouldNotBeNull();
        clearIcon.Margin.ShouldBe(default(Thickness));
        clearIcon.Padding.ShouldBe(new Thickness(InputField.BuiltInAttachmentLeftPadding, 0, 0, 0));
    }

    [Fact]
    public void Title_ShouldGenerateInputSemanticDescription_AndHideFloatingLabelFromAccessibleTree()
    {
        var control = AnimationReadyHandler.Prepare(new TextField());

        control.Title = "Email";

        var titleLabel = control.FindByViewQueryIdInVisualTreeDescendants<Label>("TitleLabel");

        SemanticProperties.GetDescription(control.EntryView).ShouldBe("Email");
        AutomationProperties.GetIsInAccessibleTree(titleLabel).ShouldBe(false);
    }

    [Fact]
    public void Title_ShouldNotOverwriteExplicitInputSemanticDescription()
    {
        var control = AnimationReadyHandler.Prepare(new TextField());

        SemanticProperties.SetDescription(control.Content, "Custom email field");
        control.Title = "Email";

        SemanticProperties.GetDescription(control.EntryView).ShouldBe("Custom email field");
    }

    [Fact]
    public void DisplayValidation_ShouldExposeValidationMessage_OnInputAndValidationLabel()
    {
        var control = AnimationReadyHandler.Prepare(new TextField());

        control.Title = "Email";

        control.Validations.Add(new FailingValidation("Email is required."));
        control.DisplayValidation();

        var validationLabel = control.FindByViewQueryIdInVisualTreeDescendants<Label>("ValidationLabel");
        SemanticProperties.GetHint(control.EntryView).ShouldBe("Error: Email is required.");
        SemanticProperties.GetDescription(validationLabel).ShouldBe("Email is required.");
    }

    [Fact]
    public void ReadOnlyAndDisabledStates_ShouldBeIncludedInInputSemanticHint()
    {
        var control = AnimationReadyHandler.Prepare(new TextField());

        control.IsReadOnly = true;

        SemanticProperties.GetHint(control.EntryView).ShouldContain("Read only.");

        control.IsEnabled = false;

        SemanticProperties.GetHint(control.EntryView).ShouldContain("Disabled.");
    }

    [Fact]
    public void ClearIcon_ShouldExposeSemanticText_AndRespectDisallowFocus()
    {
        var control = AnimationReadyHandler.Prepare(new TextField { AllowClear = true });
        var clearIcon = control.FindByViewQueryIdInVisualTreeDescendants<StatefulContentView>("ClearIcon");

        SemanticProperties.GetDescription(clearIcon).ShouldBe("Clear text");
        SemanticProperties.GetHint(clearIcon).ShouldBe("Clears the current value.");
        clearIcon.IsFocusable.ShouldBeTrue();

        control.DisallowClearButtonFocus = true;

        clearIcon.IsFocusable.ShouldBeFalse();
    }

    [Fact]
    public void AccessibilityOptions_ShouldLocalizeGeneratedSemantics()
    {
        ApplicationExtensions.CreateAndSetMockApplication(builder =>
        {
            builder.ConfigureUraniumUIAccessibility(options =>
            {
                options.ClearTextDescription = "Metni temizle";
                options.ClearTextHint = "Geçerli değeri temizler.";
                options.ValidationErrorHintFormat = "Hata: {0}";
            });
        });

        var control = AnimationReadyHandler.Prepare(new TextField { AllowClear = true });
        var clearIcon = control.FindByViewQueryIdInVisualTreeDescendants<StatefulContentView>("ClearIcon");

        control.Validations.Add(new FailingValidation("E-posta gerekli."));
        control.DisplayValidation();

        SemanticProperties.GetDescription(clearIcon).ShouldBe("Metni temizle");
        SemanticProperties.GetHint(clearIcon).ShouldBe("Geçerli değeri temizler.");
        SemanticProperties.GetHint(control.EntryView).ShouldBe("Hata: E-posta gerekli.");
    }

    [Fact]
    public void PasswordShowHideAttachment_ShouldExposeDynamicSemanticText()
    {
        var control = AnimationReadyHandler.Prepare(new TextField { IsPassword = true });
        var attachment = new TextFieldPasswordShowHideAttachment();

        control.Attachments.Add(attachment);

        SemanticProperties.GetDescription(attachment).ShouldBe("Show password");
        SemanticProperties.GetHint(attachment).ShouldBe("Toggles password visibility.");

        control.IsPassword = false;

        SemanticProperties.GetDescription(attachment).ShouldBe("Hide password");
    }
  
    [Fact]
    public void Icon_ClearedToNull_CollapsesLeadingIconSlot()
    {
        // Repro for #1002 AND assertion of the InputField invariant maintained by OnIconChanged:
        //   imageIcon is materialized, in the grid, and visible iff Icon != null (i.e. iff HasIcon).
        // Also: Content.Margin must match a TextField that never had an icon when Icon is null,
        // so the cleared field is layout-identical to a never-iconed field.
        var iconA = new FontImageSource { FontFamily = "MaterialSharp", Glyph = "A" };

        var reference = AnimationReadyHandler.Prepare(new TextField());
        var referenceMargin = reference.Content.Margin;

        var control = AnimationReadyHandler.Prepare(new TextField { Icon = iconA });
        var imageIcon = control.FindByViewQueryIdInVisualTreeDescendants<Image>("IconImage");

        imageIcon.ShouldNotBeNull();
        imageIcon.IsVisible.ShouldBeTrue();
        imageIcon.Source.ShouldBe(iconA);

        // Clear the Icon (e.g. the binding now resolves to null).
        control.Icon = null;

        // The same Image instance is still in the visual tree but collapsed.
        imageIcon.IsVisible.ShouldBeFalse();
        imageIcon.Source.ShouldBeNull();
        // And Content.Margin must match a TextField that never had an icon.
        control.Content.Margin.ShouldBe(referenceMargin);

        // Restoring a non-null Icon must un-collapse it.
        control.Icon = iconA;
        imageIcon.IsVisible.ShouldBeTrue();
        imageIcon.Source.ShouldBe(iconA);
    }

    [Fact]
    public void AccentColor_ChangedWhileFocused_ShouldRepaintFocusedVisuals()
    {
        var icon = new FontImageSource
        {
            FontFamily = "MaterialSharp",
            Glyph = "A",
            Color = Colors.Blue,
        };
        var control = AnimationReadyHandler.Prepare(new TextField
        {
            AccentColor = Colors.Blue,
            Icon = icon,
        });
        var border = control.FindByViewQueryIdInVisualTreeDescendants<Border>("Border");
        var titleLabel = control.FindByViewQueryIdInVisualTreeDescendants<Label>("TitleLabel");

        control.EntryView.SetValue(VisualElement.IsFocusedPropertyKey, true);
        control.AccentColor = Colors.Green;

        border.Stroke.ShouldBeOfType<SolidColorBrush>().Color.ShouldBe(Colors.Green);
        titleLabel.TextColor.ShouldBe(Colors.Green);
        icon.Color.ShouldBe(Colors.Green);

        control.EntryView.SetValue(VisualElement.IsFocusedPropertyKey, false);
        icon.Color.ShouldBe(Colors.Blue);
    }

    [Fact]
    public void ExplicitIconColorMatchingAccent_ShouldRemainAfterFocusCycle()
    {
        var icon = new FontImageSource
        {
            FontFamily = "MaterialSharp",
            Glyph = "A",
            Color = Colors.Blue,
        };
        var control = AnimationReadyHandler.Prepare(new TextField
        {
            AccentColor = Colors.Blue,
            Icon = icon,
        });

        control.EntryView.SetValue(VisualElement.IsFocusedPropertyKey, true);
        control.EntryView.SetValue(VisualElement.IsFocusedPropertyKey, false);

        icon.Color.ShouldBe(Colors.Blue);
    }

    [Fact]
    public void Icon_ReplacedWhileFocused_ShouldRestorePreviousIconColor()
    {
        var originalIcon = new FontImageSource
        {
            FontFamily = "MaterialSharp",
            Glyph = "A",
            Color = Colors.Black,
        };
        var replacementIcon = new FontImageSource
        {
            FontFamily = "MaterialSharp",
            Glyph = "B",
            Color = Colors.Green,
        };
        var control = AnimationReadyHandler.Prepare(new TextField
        {
            AccentColor = Colors.Blue,
            Icon = originalIcon,
        });

        control.EntryView.SetValue(VisualElement.IsFocusedPropertyKey, true);
        control.Icon = replacementIcon;

        originalIcon.Color.ShouldBe(Colors.Black);

        control.EntryView.SetValue(VisualElement.IsFocusedPropertyKey, false);
        replacementIcon.Color.ShouldBe(Colors.Green);
    }

    [Fact]
    public void DefaultIconColor_ShouldResumeFollowingTheme_AfterUnfocus()
    {
        var originalTheme = Application.Current.UserAppTheme;

        try
        {
            Application.Current.UserAppTheme = AppTheme.Light;
            Application.Current.Resources["OnBackground"] = Colors.Purple;
            Application.Current.Resources["OnBackgroundDark"] = Colors.White;
            var icon = new FontImageSource { FontFamily = "MaterialSharp", Glyph = "A" };
            var control = AnimationReadyHandler.Prepare(new TextField
            {
                AccentColor = Colors.Blue,
                Icon = icon,
            });

            control.EntryView.SetValue(VisualElement.IsFocusedPropertyKey, true);
            icon.Color.ShouldBe(Colors.Blue);

            control.EntryView.SetValue(VisualElement.IsFocusedPropertyKey, false);
            var colorBinding = GetBinding(icon, FontImageSource.ColorProperty);

            colorBinding.GetType().Name.ShouldBe("AppThemeBinding");
            GetAppThemeColor(colorBinding, "Light").ShouldBe(Colors.Purple);
            GetAppThemeColor(colorBinding, "Dark").ShouldBe(Colors.White);
        }
        finally
        {
            Application.Current.UserAppTheme = originalTheme;
        }
    }

    [Fact]
    public void DefaultIcon_ReassignedWithExplicitColor_AfterReplacement_ShouldKeepExplicitColor()
    {
        var defaultIcon = new FontImageSource { FontFamily = "MaterialSharp", Glyph = "A" };
        var replacementIcon = new FontImageSource
        {
            FontFamily = "MaterialSharp",
            Glyph = "B",
            Color = Colors.Green,
        };
        var control = AnimationReadyHandler.Prepare(new TextField { Icon = defaultIcon });

        control.Icon = replacementIcon;
        defaultIcon.Color = Colors.Red;
        control.Icon = defaultIcon;

        defaultIcon.Color.ShouldBe(Colors.Red);
    }

    [Fact]
    public void TextChanges_ShouldShouldCorrectlyUpdateClearButtonVisibility()
    {
        var control = AnimationReadyHandler.Prepare(new TextField() { AllowClear = true });
        // Currently no easier way provided by TextField/InputField to access this control
        var clearIcon = control.FindByViewQueryIdInVisualTreeDescendants<StatefulContentView>("ClearIcon");

        // Since we initialized with AllowClear = true
        clearIcon.Should().NotBeNull();
        // TextField initialized without text -> clear icon should be initially hidden
        clearIcon.IsVisible.ShouldBeFalse();
        control.Text = "Test";
        clearIcon.IsVisible.ShouldBeTrue();
        control.Text = "";
        clearIcon.IsVisible.ShouldBeFalse();
    }

    [Fact]
    public void OnApplyTemplate_ShouldReevaluateClearIconState_WhenAllowClearIsInitialized()
    {
        var control = new TemplateAwareTextField { AllowClear = true };
        var updatesBeforeTemplate = control.UpdateClearIconStateCallCount;

        control.ApplyTemplateForTest();

        control.UpdateClearIconStateCallCount.ShouldBe(updatesBeforeTemplate + 1);
    }

    [Fact]
    public void SelectionLength_ShouldBeSent_ToViewModel()
    {
        var control = AnimationReadyHandler.Prepare(new TextField());
        var viewModel = new TestViewModel();
        control.SetBinding(TextField.SelectionLengthProperty, new Binding(nameof(TestViewModel.SelectionLength)));
        control.BindingContext = viewModel;

        // Act
        control.EntryView.SelectionLength = 5;

        // Assert
        viewModel.SelectionLength.ShouldBe(control.SelectionLength);
    }

    [Fact]
    public void SelectionLengthProperty_Parent_ShouldTwoWayBind_Child ()
    {
        var control = AnimationReadyHandler.Prepare(new TextField());

        //Test Child->Parent
        // Act
        control.EntryView.SelectionLength = 5;

        // Assert
        control.SelectionLength.ShouldBe(control.EntryView.SelectionLength);

        //Test Parent->Child
        // Act
        control.SelectionLength = 10;

        // Assert
        control.SelectionLength.ShouldBe(control.EntryView.SelectionLength);
    }

    [Fact]
    public void CursorPositionProperty_Parent_ShouldTwoWayBind_Child()
    {
        var control = AnimationReadyHandler.Prepare(new TextField());

        //Test Child->Parent
        // Act
        control.EntryView.CursorPosition = 5;

        // Assert
        control.CursorPosition.ShouldBe(control.EntryView.CursorPosition);

        //Test Parent->Child
        // Act
        control.CursorPosition = 10;

        // Assert
        control.CursorPosition.ShouldBe(control.EntryView.CursorPosition);
    }

    [Fact]
    public void IsPassword_ShouldBeSet_FromViewModel()
    {
        var control = AnimationReadyHandler.Prepare(new TextField());
        var viewModel = new TestViewModel() { IsPassword = true };
        control.BindingContext = viewModel;

        // Act
        control.SetBinding(TextField.IsPasswordProperty, new Binding(nameof(TestViewModel.IsPassword)));

        // Assert
        control.IsPassword.ShouldBeTrue();
        control.EntryView.IsPassword.ShouldBeTrue();
    }

    [Fact]
    public void IsPassword_ShouldBeUpdated_FromViewModel()
    {
        var control = AnimationReadyHandler.Prepare(new TextField());
        var viewModel = new TestViewModel();
        control.BindingContext = viewModel;
        control.SetBinding(TextField.IsPasswordProperty, new Binding(nameof(TestViewModel.IsPassword)));

        // Act
        viewModel.IsPassword = true;

        // Assert
        control.IsPassword.ShouldBeTrue();
        control.EntryView.IsPassword.ShouldBeTrue();
    }

    [Fact]
    public void PasswordShowHideAttachment_IconFill_ShouldUseAppThemeBinding()
    {
        var originalTheme = Application.Current.UserAppTheme;

        try
        {
            Application.Current.UserAppTheme = AppTheme.Light;
            Application.Current.Resources["OnBackground"] = Colors.Purple;
            Application.Current.Resources["OnBackgroundDark"] = Colors.White;
            var attachment = new TextFieldPasswordShowHideAttachment();
            var control = AnimationReadyHandler.Prepare(new TextField { IsPassword = true });

            control.Attachments.Add(attachment);

            var iconPath = attachment.Content.ShouldBeOfType<Path>();
            iconPath.Fill.ShouldBeOfType<SolidColorBrush>().Color.ShouldBe(Colors.Purple.WithAlpha(.5f));
            var fillBinding = GetBinding(iconPath, Path.FillProperty);

            fillBinding.GetType().Name.ShouldBe("AppThemeBinding");
            GetAppThemeBrush(fillBinding, "Light").Color.ShouldBe(Colors.Purple.WithAlpha(.5f));
            GetAppThemeBrush(fillBinding, "Dark").Color.ShouldBe(Colors.White.WithAlpha(.5f));
        }
        finally
        {
            Application.Current.UserAppTheme = originalTheme;
        }
    }

    [Fact]
    public void IsSpellCheckEnabled_ShouldBeSet_FromViewModel()
    {
        var control = AnimationReadyHandler.Prepare(new TextField());
        var viewModel = new TestViewModel { IsSpellCheckEnabled = false };
        control.BindingContext = viewModel;

        // Act
        control.SetBinding(TextField.IsSpellCheckEnabledProperty, new Binding(nameof(TestViewModel.IsSpellCheckEnabled)));

        // Assert
        control.IsSpellCheckEnabled.ShouldBeFalse();
        control.EntryView.IsSpellCheckEnabled.ShouldBeFalse();
    }

    [Fact]
    public void IsSpellCheckEnabled_ShouldBeUpdated_FromViewModel()
    {
        var control = AnimationReadyHandler.Prepare(new TextField());
        var viewModel = new TestViewModel { IsSpellCheckEnabled = true };
        control.BindingContext = viewModel;
        control.SetBinding(TextField.IsSpellCheckEnabledProperty, new Binding(nameof(TestViewModel.IsSpellCheckEnabled)));

        // Act
        viewModel.IsSpellCheckEnabled = false;

        // Assert
        control.IsSpellCheckEnabled.ShouldBeFalse();
        control.EntryView.IsSpellCheckEnabled.ShouldBeFalse();
    }

    [Fact]
    public void Keyboard_ShouldBeSet_FromViewModel()
    {
        var control = AnimationReadyHandler.Prepare(new TextField());
        var viewModel = new TestViewModel();
        viewModel.Keyboard = Keyboard.Email;
        control.BindingContext = viewModel;

        // Act
        control.SetBinding(TextField.KeyboardProperty, new Binding(nameof(TestViewModel.Keyboard)));

        // Assert
        control.EntryView.Keyboard.ShouldBe(viewModel.Keyboard);
    }

    [Fact]
    public void Keyboard_ShouldBeUpdated_FromViewModel()
    {
        var control = AnimationReadyHandler.Prepare(new TextField());
        var viewModel = new TestViewModel();
        control.BindingContext = viewModel;
        control.SetBinding(TextField.KeyboardProperty, new Binding(nameof(TestViewModel.Keyboard)));

        // Act
        viewModel.Keyboard = Keyboard.Telephone;

        // Assert
        control.EntryView.Keyboard.ShouldBe(viewModel.Keyboard);
    }

    [Fact]
    public void ClearButtonVisibility_ShouldBeSet_FromViewModel()
    {
        var control = AnimationReadyHandler.Prepare(new TextField());
        var viewModel = new TestViewModel();
        control.BindingContext = viewModel;

        // Act
        control.SetBinding(TextField.ClearButtonVisibilityProperty, new Binding(nameof(TestViewModel.ClearButtonVisibility)));

        // Assert
        control.EntryView.ClearButtonVisibility.ShouldBe(viewModel.ClearButtonVisibility);
    }

    [Fact]
    public void ClearButtonVisibility_ShouldBeUpdated_FromViewModel()
    {
        var control = AnimationReadyHandler.Prepare(new TextField());
        var viewModel = new TestViewModel();
        control.BindingContext = viewModel;
        control.SetBinding(TextField.ClearButtonVisibilityProperty, new Binding(nameof(TestViewModel.ClearButtonVisibility)));

        // Act
        viewModel.ClearButtonVisibility = ClearButtonVisibility.WhileEditing;

        // Assert
        control.EntryView.ClearButtonVisibility.ShouldBe(viewModel.ClearButtonVisibility);
    }

    [Fact]
    public void ReturnCommandParameter_ShouldBeSet_FromViewModel()
    {
        var control = AnimationReadyHandler.Prepare(new TextField());
        var viewModel = new TestViewModel();
        control.BindingContext = viewModel;

        // Act
        control.SetBinding(TextField.ReturnCommandParameterProperty, new Binding(nameof(TestViewModel.CommandParameter)));

        // Assert child
        control.EntryView.ReturnCommandParameter.ShouldBe(viewModel.CommandParameter);

        // Assert parent
        control.ReturnCommandParameter.ShouldBe(viewModel.CommandParameter);
    }

    [Fact]
    public void ReturnCommandParameter_ShouldBeUpdated_FromViewModel()
    {
        var control = AnimationReadyHandler.Prepare(new TextField());
        var viewModel = new TestViewModel();
        control.BindingContext = viewModel;
        control.SetBinding(TextField.ReturnCommandParameterProperty, new Binding(nameof(TestViewModel.CommandParameter)));

        // Act
        viewModel.CommandParameter = "Yet Another Object";

        // Assert
        control.EntryView.ReturnCommandParameter.ShouldBe(viewModel.CommandParameter);
    }

    [Fact]
    public void ReturnCommand_ShouldBeSet_FromViewModel()
    {
        var control = AnimationReadyHandler.Prepare(new TextField());
        var viewModel = new TestViewModel();
        viewModel.Command = new Command(() => Console.WriteLine("My Custom Command"));
        control.BindingContext = viewModel;

        // Act
        control.SetBinding(TextField.ReturnCommandProperty, new Binding(nameof(TestViewModel.Command)));

        // Assert
        control.EntryView.ReturnCommand.ShouldBe(viewModel.Command);
    }

    [Fact]
    public void ClearCommand_ShouldBeSet_FromViewModel()
    {
        var control = AnimationReadyHandler.Prepare(new TextField());
        var viewModel = new TestViewModel();
        viewModel.Command = new Command(() => Console.WriteLine("Clear Command"));
        control.BindingContext = viewModel;

        control.SetBinding(TextField.ClearCommandProperty, new Binding(nameof(TestViewModel.Command)));

        control.ClearCommand.ShouldBe(viewModel.Command);
    }

    [Fact]
    public void ClearIcon_ShouldExecute_ClearCommand_WhenProvided()
    {
        var control = AnimationReadyHandler.Prepare(new TextField { AllowClear = true });
        var clearIcon = control.FindByViewQueryIdInVisualTreeDescendants<StatefulContentView>("ClearIcon");
        object parameter = null;

        control.ClearCommand = new Command<object>(value => parameter = value);
        control.Text = "Test";

        clearIcon.TappedCommand.Execute(null);

        parameter.ShouldBe(control);
        control.Text.ShouldBe("Test");
    }

    [Fact]
    public void CharacterSpacing_ShouldBeSet_FromViewModel()
    {
        var control = AnimationReadyHandler.Prepare(new TextField());
        var viewModel = new TestViewModel();
        viewModel.CharacterSpacing = 6;
        control.BindingContext = viewModel;

        // Act
        control.SetBinding(TextField.CharacterSpacingProperty, new Binding(nameof(TestViewModel.CharacterSpacing)));

        // Assert
        control.EntryView.CharacterSpacing.ShouldBe(viewModel.CharacterSpacing);
    }

    [Fact]
    public void CharacterSpacing_ShouldBeUpdated_FromViewModel()
    {
        var control = AnimationReadyHandler.Prepare(new TextField());
        var viewModel = new TestViewModel();
        viewModel.CharacterSpacing = 6;
        control.BindingContext = viewModel;
        control.SetBinding(TextField.CharacterSpacingProperty, new Binding(nameof(TestViewModel.CharacterSpacing)));

        // Act
        viewModel.CharacterSpacing = 3;

        // Assert
        control.EntryView.CharacterSpacing.ShouldBe(viewModel.CharacterSpacing);
    }

    public class TestViewModel : UraniumBindableObject
    {
        private bool isChecked;
        private string text;
        private ICommand command;
        private int selectionLength;
        private bool isPassword;
        private bool isSpellCheckEnabled;
        private Keyboard keyboard;
        private ClearButtonVisibility clearButtonVisibility;
        private double characterSpacing;
        private IList<IView> attachments;
        private object commandParameter = "My Command Parameter 1";

        public bool IsChecked { get => isChecked; set => SetProperty(ref isChecked, value); }

        public string Text { get => text; set => SetProperty(ref text, value); }

        public ICommand Command { get => command; set => SetProperty(ref command, value); }

        public object CommandParameter { get => commandParameter; set => SetProperty(ref commandParameter, value); }
        public int SelectionLength { get => selectionLength; set => SetProperty(ref selectionLength, value); }

        public bool IsPassword { get => isPassword; set => SetProperty(ref isPassword, value); }

        public bool IsSpellCheckEnabled { get => isSpellCheckEnabled; set => SetProperty(ref isSpellCheckEnabled, value); }

        public Keyboard Keyboard { get => keyboard; set => SetProperty(ref keyboard, value); }

        public ClearButtonVisibility ClearButtonVisibility { get => clearButtonVisibility; set => SetProperty(ref clearButtonVisibility, value); }

        public double CharacterSpacing { get => characterSpacing; set => SetProperty(ref characterSpacing, value); }

        public IList<IView> Attachments { get => attachments; set => SetProperty(ref attachments, value); }
    }

    private sealed class TemplateAwareTextField : TextField
    {
        public int UpdateClearIconStateCallCount { get; private set; }

        public void ApplyTemplateForTest() => base.OnApplyTemplate();

        protected override void UpdateClearIconState()
        {
            UpdateClearIconStateCallCount++;
            base.UpdateClearIconState();
        }
    }

    private sealed class FocusTrackingHandler : ViewHandler<IView, object>
    {
        private static readonly IPropertyMapper<IView, FocusTrackingHandler> Mapper = new PropertyMapper<IView, FocusTrackingHandler>(ViewHandler.ViewMapper);

        private static readonly CommandMapper<IView, FocusTrackingHandler> Commands = new(ViewHandler.ViewCommandMapper)
        {
            [nameof(IView.Focus)] = MapFocus,
        };

        public FocusTrackingHandler()
            : base(Mapper, Commands)
        {
            SetMauiContext(new AnimationReadyHandler.AnimationReadyMauiContext());
        }

        public bool FocusRequested { get; private set; }

        protected override object CreatePlatformView() => new();

        private static void MapFocus(FocusTrackingHandler handler, IView view, object args)
        {
            handler.FocusRequested = true;

            if (args is RetrievePlatformValueRequest<bool> request)
            {
                request.SetResult(true);
            }
        }
    }

    private static object GetBinding(BindableObject bindableObject, BindableProperty bindableProperty)
    {
        var properties = (System.Collections.IDictionary)typeof(BindableObject)
            .GetField("_properties", BindingFlags.Instance | BindingFlags.NonPublic)
            .GetValue(bindableObject);
        var context = properties[bindableProperty];

        context.ShouldNotBeNull();
        var bindings = context.GetType()
            .GetField("Bindings", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            .GetValue(context);

        return bindings.GetType()
            .GetMethod("GetValue", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, Type.EmptyTypes)
            .Invoke(bindings, null);
    }

    private static SolidColorBrush GetAppThemeBrush(object appThemeBinding, string propertyName)
    {
        return appThemeBinding.GetType()
            .GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            .GetValue(appThemeBinding)
            .ShouldBeOfType<SolidColorBrush>();
    }

    private static Color GetAppThemeColor(object appThemeBinding, string propertyName)
    {
        return appThemeBinding.GetType()
            .GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            .GetValue(appThemeBinding)
            .ShouldBeOfType<Color>();
    }

    private sealed class FailingValidation : IValidation
    {
        public FailingValidation(string message)
        {
            Message = message;
        }

        public string Message { get; }

        public bool Validate(object value) => false;
    }
}
