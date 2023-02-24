using FluentAssertions;
using NSubstitute;
using Shouldly;
using System.Windows.Input;
using UraniumUI.Material.Controls;
using UraniumUI.Tests.Core;

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

        // Assert
        control.EntryView.ReturnCommandParameter.ShouldBe(viewModel.CommandParameter);
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
