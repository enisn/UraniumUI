using Shouldly;
using UraniumUI.Material.Controls;
using UraniumUI.Tests.Core;

namespace UraniumUI.Material.Tests.Controls;
public class TimePickerField_Test
{
    public TimePickerField_Test()
    {
        ApplicationExtensions.CreateAndSetMockApplication();
    }

    [Fact]
    public void Time_BindingForInitializtion_FromSource()
    {
        var control = AnimationReadyHandler.Prepare(new TimePickerField());
        var viewModel = new TestViewModel { Time = TimeSpan.FromHours(2) };
        control.BindingContext = viewModel;
        control.SetBinding(TimePickerField.TimeProperty, new Binding(nameof(TestViewModel.Time)));

        // Assert
        control.Time.ShouldBe(viewModel.Time);
    }

    [Fact]
    public void Time_Binding_FromSource()
    {
        var control = AnimationReadyHandler.Prepare(new TimePickerField());
        var viewModel = new TestViewModel { Time = TimeSpan.FromHours(2) };
        control.BindingContext = viewModel;
        control.SetBinding(TimePickerField.TimeProperty, new Binding(nameof(TestViewModel.Time)));

        // Act
        viewModel.Time = TimeSpan.Parse("09:05");

        // Assert
        control.Time.ShouldBe(viewModel.Time);
    }

    [Fact]
    public void Time_Binding_ToSource()
    {
        var control = AnimationReadyHandler.Prepare(new TimePickerField());
        var viewModel = new TestViewModel { Time = TimeSpan.FromHours(2) };
        control.BindingContext = viewModel;
        control.SetBinding(TimePickerField.TimeProperty, new Binding(nameof(TestViewModel.Time)));

        // Act
        control.Time = TimeSpan.Parse("09:05");

        // Assert
        viewModel.Time.ShouldBe(control.Time);
    }

    [Fact]
    public void Format_ShouldBeSet_FromViewModel()
    {
        var control = AnimationReadyHandler.Prepare(new TimePickerField());
        var viewModel = new TestViewModel();
        viewModel.Format = "HH:mm"; //24H format
        control.BindingContext = viewModel;

        // Act
        control.SetBinding(TimePickerField.FormatProperty, new Binding(nameof(TestViewModel.Format)));

        // Assert
        control.TimePickerView.Format.ShouldBe(viewModel.Format);
    }

    [Fact]
    public void TextColor_ShouldBeSet_FromViewModel()
    {
        var control = AnimationReadyHandler.Prepare(new TimePickerField());
        var viewModel = new TestViewModel();
        viewModel.TextColor = Colors.Blue;
        control.BindingContext = viewModel;

        // Act
        control.SetBinding(TimePickerField.TextColorProperty, new Binding(nameof(TestViewModel.TextColor)));

        // Assert
        control.TimePickerView.TextColor.ShouldBe(viewModel.TextColor);
    }

    [Fact]
    public void CharacterSpacing_ShouldBeSet_FromViewModel()
    {
        var control = AnimationReadyHandler.Prepare(new TimePickerField());
        var viewModel = new TestViewModel();
        viewModel.CharacterSpacing = 4.41;
        control.BindingContext = viewModel;

        // Act
        control.SetBinding(TimePickerField.CharacterSpacingProperty, new Binding(nameof(TestViewModel.CharacterSpacing)));

        // Assert
        control.TimePickerView.CharacterSpacing.ShouldBe(viewModel.CharacterSpacing);
    }

    [Fact]
    public void FontAttributes_ShouldBeSet_FromControl()
    {
        var control = AnimationReadyHandler.Prepare(new TimePickerField());
        var fontAttributes = FontAttributes.Italic;

        // Act
        control.FontAttributes = fontAttributes;

        // Assert
        control.TimePickerView.FontAttributes.ShouldBe(fontAttributes);
    }

    [Fact]
    public void FontAttributes_ShouldBeSet_FromViewModel()
    {
        var control = AnimationReadyHandler.Prepare(new TimePickerField());
        var viewModel = new TestViewModel();
        viewModel.FontAttributes = FontAttributes.Italic;
        control.BindingContext = viewModel;

        // Act
        control.SetBinding(TimePickerField.FontAttributesProperty, new Binding(nameof(TestViewModel.FontAttributes)));

        // Assert
        control.TimePickerView.FontAttributes.ShouldBe(viewModel.FontAttributes);
    }

    [Fact]
    public void FontFamily_ShouldBeSet_FromViewModel()
    {
        var control = AnimationReadyHandler.Prepare(new TimePickerField());
        var viewModel = new TestViewModel();
        viewModel.FontFamily = "Roboto";
        control.BindingContext = viewModel;

        // Act
        control.SetBinding(TimePickerField.FontFamilyProperty, new Binding(nameof(TestViewModel.FontFamily)));

        // Assert
        control.TimePickerView.FontFamily.ShouldBe(viewModel.FontFamily);
    }

    [Fact]
    public void FontFamily_ShouldBeSet_FromControl()
    {
        var control = AnimationReadyHandler.Prepare(new TimePickerField());
        var fontFamily = "Roboto";

        // Act
        control.FontFamily = fontFamily;

        // Assert
        control.TimePickerView.FontFamily.ShouldBe(fontFamily);
    }

    [Fact]
    public void FontSize_ShouldBeSet_FromViewModel()
    {
        var control = AnimationReadyHandler.Prepare(new TimePickerField());
        var viewModel = new TestViewModel();
        viewModel.FontSize = 28.5;
        control.BindingContext = viewModel;

        // Act
        control.SetBinding(TimePickerField.FontSizeProperty, new Binding(nameof(TestViewModel.FontSize)));

        // Assert
        control.TimePickerView.FontSize.ShouldBe(viewModel.FontSize);
    }

    [Fact]
    public void FontSize_ShouldBeSet_FromControl()
    {
        var control = AnimationReadyHandler.Prepare(new TimePickerField());
        var fontSize = 24.75;

        // Act
        control.FontSize = fontSize;

        // Assert
        control.TimePickerView.FontSize.ShouldBe(fontSize);
    }

    public class TestViewModel : UraniumBindableObject
    {
        private TimeSpan? time;
        private string format;
        private Color textColor;
        private double characterSpacing;
        private FontAttributes fontAttributes;
        private string fontFamily;
        private double fontSize;

        public TimeSpan? Time { get => time; set => SetProperty(ref time, value); }

        public string Format { get => format; set => SetProperty(ref format, value); }

        public Color TextColor { get => textColor; set => SetProperty(ref textColor, value); }

        public double CharacterSpacing { get => characterSpacing; set => SetProperty(ref characterSpacing, value); }

        public FontAttributes FontAttributes { get => fontAttributes; set => SetProperty(ref fontAttributes, value); }

        public string FontFamily { get => fontFamily; set => SetProperty(ref fontFamily, value); }

        public double FontSize { get => fontSize; set => SetProperty(ref fontSize , value); }
    }
}
