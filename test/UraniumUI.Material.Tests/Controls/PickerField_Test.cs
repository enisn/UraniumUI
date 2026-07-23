using Shouldly;
using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Maui.Controls.Xaml;
using UraniumUI.Material.Controls;
using UraniumUI.Tests.Core;
using UraniumUI.ViewExtensions;
using UraniumUI.Views;

namespace UraniumUI.Material.Tests.Controls;
[Collection("ApplicationResources")]
public class PickerField_Test
{
    public PickerField_Test()
    {
        ApplicationExtensions.CreateAndSetMockApplication();
    }

    private string[] GetItemsSource() => new string[] { "Option 1", "Option 2", "Option 3", "Option 4", };

    [Fact]
    public void SelectedItem_BindingForInitialization_FromSource()
    {
        var control = AnimationReadyHandler.Prepare(new PickerField());
        var viewModel = new TestViewModel { SelectedItem = "Option 3" };
        control.BindingContext = viewModel;
        control.ItemsSource = GetItemsSource();
        control.SetBinding(PickerField.SelectedItemProperty, new Binding(nameof(TestViewModel.SelectedItem)));

        // Assert
        control.SelectedItem.ShouldBe(viewModel.SelectedItem);
    }

    [Fact]
    public void SelectedItem_Binding_FromSource()
    {
        var control = AnimationReadyHandler.Prepare(new PickerField());
        var viewModel = new TestViewModel { SelectedItem = "Option 3" };
        control.BindingContext = viewModel;
        control.ItemsSource = GetItemsSource();
        control.SetBinding(PickerField.SelectedItemProperty, new Binding(nameof(TestViewModel.SelectedItem)));

        // Act
        viewModel.SelectedItem = "Option 2";

        // Assert
        control.SelectedItem.ShouldBe(viewModel.SelectedItem);
    }

    [Fact]
    public void SelectedItem_Binding_ToSource()
    {
        var control = AnimationReadyHandler.Prepare(new PickerField());
        var viewModel = new TestViewModel { SelectedItem = "Option 3" };
        control.BindingContext = viewModel;
        control.SetBinding(PickerField.SelectedItemProperty, new Binding(nameof(TestViewModel.SelectedItem)));

        // Act
        control.SelectedItem = "Option 4";

        // Assert
        viewModel.SelectedItem.ShouldBe(control.SelectedItem);
    }

    [Fact]
    public void ItemsSource_ShouldBeSet_FromViewModel()
    {
        var control = AnimationReadyHandler.Prepare(new PickerField());
        var viewModel = new TestViewModel();
        viewModel.ItemsSource = GetItemsSource();
        control.BindingContext = viewModel;

        // Act
        control.SetBinding(PickerField.ItemsSourceProperty, new Binding(nameof(TestViewModel.ItemsSource)));

        // Assert
        control.PickerView.ItemsSource.ShouldBe(viewModel.ItemsSource);
    }

    [Fact]
    public void ItemsSource_ShouldBeUpdated_FromViewModel()
    {
        var control = AnimationReadyHandler.Prepare(new PickerField());
        var viewModel = new TestViewModel();
        viewModel.ItemsSource = new ObservableCollection<string>(GetItemsSource());
        control.BindingContext = viewModel;
        control.SetBinding(PickerField.ItemsSourceProperty, new Binding(nameof(TestViewModel.ItemsSource)));

        // Act
        viewModel.ItemsSource.Add("Option 5");

        // Assert
        control.PickerView.ItemsSource.ShouldBe(viewModel.ItemsSource);
        control.PickerView.ItemsSource.Count.ShouldBe(viewModel.ItemsSource.Count);
    }

    [Fact]
    public void ItemDisplayBinding_ShouldBeSet_FromBindableProperty()
    {
        var control = AnimationReadyHandler.Prepare(new PickerField());
        var itemDisplayBinding = new Binding(nameof(TestItem.DisplayName));

        // Act
        control.SetValue(PickerField.ItemDisplayBindingProperty, itemDisplayBinding);

        // Assert
        control.ItemDisplayBinding.ShouldBeSameAs(itemDisplayBinding);
        control.PickerView.ItemDisplayBinding.ShouldBeSameAs(itemDisplayBinding);
    }

    [Fact]
    public void ItemDisplayBinding_ShouldPopulateItemText_WhenSetFromXaml()
    {
        var control = new PickerField().LoadFromXaml("""
            <material:PickerField
                xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
                xmlns:material="clr-namespace:UraniumUI.Material.Controls;assembly=UraniumUI.Material"
                ItemDisplayBinding="{Binding DisplayName}" />
            """);
        AnimationReadyHandler.Prepare(control);
        control.BindingContext = new TestViewModel();

        control.ItemsSource = new[] { new TestItem { DisplayName = "Expected display name" } };

        control.PickerView.Items.Count.ShouldBe(1);
        control.PickerView.Items[0].ShouldBe("Expected display name");
    }

    [Fact]
    public void TextColor_ShouldBeSet_FromViewModel()
    {
        var control = AnimationReadyHandler.Prepare(new PickerField());
        var viewModel = new TestViewModel();
        viewModel.TextColor = Colors.Blue;
        control.BindingContext = viewModel;

        // Act
        control.SetBinding(PickerField.TextColorProperty, new Binding(nameof(TestViewModel.TextColor)));

        // Assert
        control.PickerView.TextColor.ShouldBe(viewModel.TextColor);
    }

    [Fact]
    public void CharacterSpacing_ShouldBeSet_FromViewModel()
    {
        var control = AnimationReadyHandler.Prepare(new PickerField());
        var viewModel = new TestViewModel();
        viewModel.CharacterSpacing = 4.41;
        control.BindingContext = viewModel;

        // Act
        control.SetBinding(PickerField.CharacterSpacingProperty, new Binding(nameof(TestViewModel.CharacterSpacing)));

        // Assert
        control.PickerView.CharacterSpacing.ShouldBe(viewModel.CharacterSpacing);
    }

    [Fact]
    public void FontAttributes_ShouldBeSet_FromControl()
    {
        var control = AnimationReadyHandler.Prepare(new PickerField());
        var fontAttributes = FontAttributes.Italic;

        // Act
        control.FontAttributes = fontAttributes;

        // Assert
        control.PickerView.FontAttributes.ShouldBe(fontAttributes);
    }

    [Fact]
    public void FontAttributes_ShouldBeSet_FromViewModel()
    {
        var control = AnimationReadyHandler.Prepare(new PickerField());
        var viewModel = new TestViewModel();
        viewModel.FontAttributes = FontAttributes.Italic;
        control.BindingContext = viewModel;

        // Act
        control.SetBinding(PickerField.FontAttributesProperty, new Binding(nameof(TestViewModel.FontAttributes)));

        // Assert
        control.PickerView.FontAttributes.ShouldBe(viewModel.FontAttributes);
    }

    [Fact]
    public void FontFamily_ShouldBeSet_FromViewModel()
    {
        var control = AnimationReadyHandler.Prepare(new PickerField());
        var viewModel = new TestViewModel();
        viewModel.FontFamily = "Roboto";
        control.BindingContext = viewModel;

        // Act
        control.SetBinding(PickerField.FontFamilyProperty, new Binding(nameof(TestViewModel.FontFamily)));

        // Assert
        control.PickerView.FontFamily.ShouldBe(viewModel.FontFamily);
    }

    [Fact]
    public void FontFamily_ShouldBeSet_FromControl()
    {
        var control = AnimationReadyHandler.Prepare(new PickerField());
        var fontFamily = "Roboto";

        // Act
        control.FontFamily = fontFamily;

        // Assert
        control.PickerView.FontFamily.ShouldBe(fontFamily);
    }

    [Fact]
    public void FontSize_ShouldBeSet_FromViewModel()
    {
        var control = AnimationReadyHandler.Prepare(new PickerField());
        var viewModel = new TestViewModel();
        viewModel.FontSize = 28.5;
        control.BindingContext = viewModel;

        // Act
        control.SetBinding(PickerField.FontSizeProperty, new Binding(nameof(TestViewModel.FontSize)));

        // Assert
        control.PickerView.FontSize.ShouldBe(viewModel.FontSize);
    }

    [Fact]
    public void FontSize_ShouldBeSet_FromControl()
    {
        var control = AnimationReadyHandler.Prepare(new PickerField());
        var fontSize = 24.75;

        // Act
        control.FontSize = fontSize;

        // Assert
        control.PickerView.FontSize.ShouldBe(fontSize);
    }

    [Fact]
    public void FontAutoScalingEnabled_CanBeSetViaImplicitStyle_DuringConstruction()
    {
        var style = new Style(typeof(PickerField));
        style.Setters.Add(new Setter { Property = PickerField.FontAutoScalingEnabledProperty, Value = false });

        var applicationResources = Application.Current.Resources ??= new ResourceDictionary();
        var testResources = new ResourceDictionary
        {
            style,
        };

        applicationResources.MergedDictionaries.Add(testResources);

        try
        {
            PickerField control = null;
            var exception = Record.Exception(() => control = new PickerField());

            exception.ShouldBeNull();

            AnimationReadyHandler.Prepare(control);

            control.FontAutoScalingEnabled.ShouldBeFalse();
            control.PickerView.FontAutoScalingEnabled.ShouldBeFalse();
        }
        finally
        {
            applicationResources.MergedDictionaries.Remove(testResources);
        }
    }

    [Fact]
    public void Icon_ShouldBeAdded_WhenSetBeforeHandler()
    {
        var icon = new FontImageSource
        {
            FontFamily = "MaterialSharp",
            Glyph = "A",
        };

        var control = new PickerField
        {
            Icon = icon,
        };

        AnimationReadyHandler.Prepare(control);

        var innerGrid = control.FindByViewQueryIdInVisualTreeDescendants<Grid>("InnerGrid");
        var iconView = innerGrid?.Children.OfType<Image>().FirstOrDefault(x => ReferenceEquals(x.Source, icon));

        innerGrid.ShouldNotBeNull();
        iconView.ShouldNotBeNull();
        control.Content.Margin.Left.ShouldBe(5);
    }

    [Fact]
    public void ClearIcon_ShouldBeAdded_AfterTemplateApplied_WhenAllowClearIsEnabled()
    {
        var control = AnimationReadyHandler.Prepare(new PickerField());

        var clearIcon = control.FindByViewQueryIdInVisualTreeDescendants<StatefulContentView>("ClearIcon");

        clearIcon.ShouldNotBeNull();
    }

    [Fact]
    public void ClearIcon_HasAsymmetricLeftHitPadding()
    {
        var control = AnimationReadyHandler.Prepare(new PickerField { AllowClear = true });
        var clearIcon = control.FindByViewQueryIdInVisualTreeDescendants<StatefulContentView>("ClearIcon");

        clearIcon.ShouldNotBeNull();
        clearIcon.Margin.ShouldBe(default(Thickness));
        clearIcon.Padding.ShouldBe(new Thickness(InputField.BuiltInAttachmentLeftPadding, 0, 0, 0));
    }

    public class TestViewModel : UraniumBindableObject
    {
        private object selectedItem;
        private Color textColor;
        private double characterSpacing;
        private FontAttributes fontAttributes;
        private string fontFamily;
        private double fontSize;
        private IList itemsSource;

        public object SelectedItem { get => selectedItem; set => SetProperty(ref selectedItem, value); }

        public Color TextColor { get => textColor; set => SetProperty(ref textColor, value); }

        public double CharacterSpacing { get => characterSpacing; set => SetProperty(ref characterSpacing, value); }

        public FontAttributes FontAttributes { get => fontAttributes; set => SetProperty(ref fontAttributes, value); }

        public string FontFamily { get => fontFamily; set => SetProperty(ref fontFamily, value); }

        public double FontSize { get => fontSize; set => SetProperty(ref fontSize, value); }

        public IList ItemsSource { get => itemsSource; set => SetProperty(ref itemsSource, value); }
    }

    private sealed class TestItem
    {
        public string DisplayName { get; set; }
    }
}
