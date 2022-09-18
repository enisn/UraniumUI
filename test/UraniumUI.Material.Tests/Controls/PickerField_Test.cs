using Shouldly;
using System.Collections;
using System.Collections.ObjectModel;
using UraniumUI.Material.Controls;
using UraniumUI.Tests.Core;

namespace UraniumUI.Material.Tests.Controls;
public class PickerField_Test
{
    public PickerField_Test()
    {
        ApplicationExtensions.CreateAndSetMockApplication();
    }

    private string[] GetItemsSource() => new string[] { "Option 1", "Option 2", "Option 3", "Option 4", };

    [Fact]
    public void SelectedItem_BindingForInitializtion_FromSource()
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
}
