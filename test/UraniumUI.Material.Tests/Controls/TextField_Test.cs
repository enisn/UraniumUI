using FluentAssertions;
using NSubstitute;
using Shouldly;
using System.Windows.Input;
using UraniumUI.Material.Controls;
using UraniumUI.Tests.Core;
using UraniumUI.ViewExtensions;
using UraniumUI.Views;

namespace UraniumUI.Material.Tests.Controls;
public class TextField_Test
{
    public TextField_Test()
    {
        ApplicationExtensions.CreateAndSetMockApplication();
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
        clearIcon.Padding.ShouldBe(new Thickness(5, 0, 0, 0));
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
        private Keyboard keyboard;
        private ClearButtonVisibility clearButtonVisibility;
        private double characterSpacing;
        private object commandParameter = "My Command Parameter 1";

        public bool IsChecked { get => isChecked; set => SetProperty(ref isChecked, value); }

        public string Text { get => text; set => SetProperty(ref text, value); }

        public ICommand Command { get => command; set => SetProperty(ref command, value); }

        public object CommandParameter { get => commandParameter; set => SetProperty(ref commandParameter, value); }
        public int SelectionLength { get => selectionLength; set => SetProperty(ref selectionLength, value); }

        public bool IsPassword { get => isPassword; set => SetProperty(ref isPassword, value); }

        public Keyboard Keyboard { get => keyboard; set => SetProperty(ref keyboard, value); }

        public ClearButtonVisibility ClearButtonVisibility { get => clearButtonVisibility; set => SetProperty(ref clearButtonVisibility, value); }

        public double CharacterSpacing { get => characterSpacing; set => SetProperty(ref characterSpacing, value); }
    }
}
