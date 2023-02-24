using Shouldly;
using UraniumUI.Material.Controls;
using UraniumUI.Tests.Core;

namespace UraniumUI.Material.Tests.Controls;
public class AutoCompleteTextField_Test
{
    public AutoCompleteTextField_Test()
    {
        ApplicationExtensions.CreateAndSetMockApplication();
    }

    [Fact]
    public void Text_BindingForInitialization_FromSource()
    {
        var control = AnimationReadyHandler.Prepare(new AutoCompleteTextField());
        var viewModel = new { Text = "My Title" };
        control.BindingContext = viewModel;
        control.SetBinding(AutoCompleteTextField.TextProperty, new Binding(nameof(viewModel.Text)));

        // Assert
        control.Text.ShouldBe(viewModel.Text);
    }

    [Fact]
    public void Text_ShouldBeChanged_FromSource()
    {
        var control = AnimationReadyHandler.Prepare(new AutoCompleteTextField());
        var viewModel = new AutoCompleteTextFieldTestViewModel { Text = "My Title" };
        control.BindingContext = viewModel;
        control.SetBinding(AutoCompleteTextField.TextProperty, new Binding(nameof(viewModel.Text)));

        // Act
        viewModel.Text = "Title (changed)";

        // Assert
        control.Text.ShouldBe(viewModel.Text);
    }

    [Fact]
    public void Text_ShouldBeChanged_FromControl()
    {
        var control = AnimationReadyHandler.Prepare(new AutoCompleteTextField());
        var viewModel = new AutoCompleteTextFieldTestViewModel { Text = "My Title" };
        control.BindingContext = viewModel;
        control.SetBinding(AutoCompleteTextField.TextProperty, new Binding(nameof(viewModel.Text)));

        // Act
        control.Text = "Title (changed)";

        // Assert
        viewModel.Text.ShouldBe(control.Text);
    }

    internal class AutoCompleteTextFieldTestViewModel : UraniumBindableObject
    {
        private string text;

        public string Text { get => text; set => SetProperty(ref text, value); }
    }
}
