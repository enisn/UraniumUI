using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UraniumUI.Material.Controls;
using UraniumUI.Tests.Core;
using static UraniumUI.Material.Tests.Controls.AutoCompleteTextField_Test;

namespace UraniumUI.Material.Tests.Controls;
public class EditorField_Test 
{
    public EditorField_Test()
    {
        ApplicationExtensions.CreateAndSetMockApplication();
    }

    [Fact]
    public void Text_BindingForInitialization_FromSource()
    {
        var control = AnimationReadyHandler.Prepare(new EditorField());
        var viewModel = new { Text = "My Title" };
        control.BindingContext = viewModel;
        control.SetBinding(EditorField.TextProperty, new Binding(nameof(viewModel.Text)));

        // Assert
        control.Text.ShouldBe(viewModel.Text);
    }

    [Fact]
    public void Text_ShouldBeChanged_FromSource()
    {
        var control = AnimationReadyHandler.Prepare(new EditorField());
        var viewModel = new EditorFieldTestViewModel { Text = "My Title" };
        control.BindingContext = viewModel;
        control.SetBinding(EditorField.TextProperty, new Binding(nameof(viewModel.Text)));

        // Act
        viewModel.Text = "Title (changed)";

        // Assert
        control.Text.ShouldBe(viewModel.Text);
    }

    [Fact]
    public void Text_ShouldBeChanged_FromControl()
    {
        var control = AnimationReadyHandler.Prepare(new EditorField());
        var viewModel = new EditorFieldTestViewModel { Text = "My Title" };
        control.BindingContext = viewModel;
        control.SetBinding(EditorField.TextProperty, new Binding(nameof(viewModel.Text)));

        // Act
        control.Text = "Title (changed)";

        // Assert
        viewModel.Text.ShouldBe(control.Text);
    }

    internal class EditorFieldTestViewModel : UraniumBindableObject
    {
        private string text;

        public string Text { get => text; set => SetProperty(ref text, value); }
    }

    [Fact]
    public void FontFamily_CanBeSetViaImplicitStyle()
    {
        // Arrange: Create an implicit style that sets FontFamily
        var style = new Style(typeof(EditorField));
        style.Setters.Add(new Setter { Property = EditorField.FontFamilyProperty, Value = "Arial" });

        // Create a ResourceDictionary and add the style
        var resources = new ResourceDictionary();
        resources.Add(style);

        var control = AnimationReadyHandler.Prepare(new EditorField());

        // Act
        control.Resources = resources;

        // Assert
        control.FontFamily.ShouldBe("Arial");
        control.EditorView.FontFamily.ShouldBe("Arial");
    }

    [Fact]
    public void FontFamily_CanBeSetDirectly()
    {
        // Arrange
        var control = AnimationReadyHandler.Prepare(new EditorField());

        // Act
        control.FontFamily = "Courier New";

        // Assert
        control.FontFamily.ShouldBe("Courier New");
        control.EditorView.FontFamily.ShouldBe("Courier New");
    }
}
