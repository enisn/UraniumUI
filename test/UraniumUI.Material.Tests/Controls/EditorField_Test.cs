using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

    [Fact]
    public void TextColor_ShouldBeSet_FromViewModel()
    {
        var control = AnimationReadyHandler.Prepare(new EditorField());
        var viewModel = new EditorFieldTestViewModel { TextColor = Colors.Blue };
        control.BindingContext = viewModel;

        // Act
        control.SetBinding(EditorField.TextColorProperty, new Binding(nameof(EditorFieldTestViewModel.TextColor)));

        // Assert
        control.EditorView.TextColor.ShouldBe(viewModel.TextColor);
    }

    [Fact]
    public void TextColor_ShouldOverride_ExistingThemeBinding_WhenValueMatchesLightTheme()
    {
        var originalTheme = Application.Current.UserAppTheme;

        try
        {
            Application.Current.UserAppTheme = AppTheme.Light;

            var control = AnimationReadyHandler.Prepare(new EditorField());
            control.EditorView.SetAppThemeColor(Editor.TextColorProperty, Colors.Black, Colors.White);

            // Act
            control.TextColor = Colors.Black;
            Application.Current.UserAppTheme = AppTheme.Dark;

            // Assert
            control.EditorView.TextColor.ShouldBe(Colors.Black);
        }
        finally
        {
            Application.Current.UserAppTheme = originalTheme;
        }
    }

    internal class EditorFieldTestViewModel : UraniumBindableObject
    {
        private string text;
        private Color textColor;

        public string Text { get => text; set => SetProperty(ref text, value); }

        public Color TextColor { get => textColor; set => SetProperty(ref textColor, value); }
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

    [Fact]
    public void ForwardedProperties_DefaultToTheEditorDefaults()
    {
        // A property forwards only when it changes, so a default that differs from the editor's is never applied.
        string[] intentional = [nameof(EditorField.TextProperty), nameof(EditorField.TextColorProperty)];

        var mismatches = typeof(EditorField)
            .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
            .Where(field => field.FieldType == typeof(BindableProperty) && !intentional.Contains(field.Name))
            .Select(field => (Own: (BindableProperty)field.GetValue(null), Editor: typeof(Editor).GetField(field.Name, BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)?.GetValue(null) as BindableProperty))
            .Where(pair => pair.Editor is not null && !Equals(pair.Own.DefaultValue, pair.Editor.DefaultValue))
            .Select(pair => $"{pair.Own.PropertyName}: {pair.Own.DefaultValue} instead of {pair.Editor.DefaultValue}")
            .ToList();

        mismatches.ShouldBeEmpty();
    }

    [Fact]
    public void TextPredictionAndSpellCheck_SetToFalse_ReachTheEditor()
    {
        var control = AnimationReadyHandler.Prepare(new EditorField());

        control.IsTextPredictionEnabled = false;
        control.IsSpellCheckEnabled = false;

        control.EditorView.IsTextPredictionEnabled.ShouldBeFalse();
        control.EditorView.IsSpellCheckEnabled.ShouldBeFalse();
    }
}
