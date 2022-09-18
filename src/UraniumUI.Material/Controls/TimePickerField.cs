using Plainer.Maui.Controls;
using System.ComponentModel;

namespace UraniumUI.Material.Controls;

[ContentProperty(nameof(Validations))]
public class TimePickerField : InputField
{
    public TimePickerView TimePickerView => Content as TimePickerView;
    public override View Content { get; set; } = new TimePickerView
    {
        VerticalOptions = LayoutOptions.Center,
        Margin = new Thickness(10, 0)
    };

    public override bool HasValue => true; // Timespan cannot be null

    public TimePickerField()
    {
        TimePickerView.SetBinding(TimePicker.TimeProperty, new Binding(nameof(Time), source: this));
    }

    protected override object GetValueForValidator()
    {
        return TimePickerView.Time;
    }

    protected virtual void OnTimeChanged()
    {
        OnPropertyChanged(nameof(Time));
        CheckAndShowValidations();
    }

    protected override void OnIconChanged()
    {
        base.OnIconChanged();

        if (Icon == null)
        {
            TimePickerView.Margin = new Thickness(10, 0);
        }
        else
        {
            TimePickerView.Margin = 0;
        }
    }

    public TimeSpan Time { get => (TimeSpan)GetValue(TimeProperty); set => SetValue(TimeProperty, value); }

    public static readonly BindableProperty TimeProperty =
        BindableProperty.Create(nameof(Time), typeof(TimeSpan), typeof(TimePickerField), TimePicker.TimeProperty.DefaultValue, defaultBindingMode: BindingMode.TwoWay,
            propertyChanged: (bindable, oldValue, newValue) => (bindable as TimePickerField).OnTimeChanged());

    public string Format { get => (string)GetValue(FormatProperty); set => SetValue(FormatProperty, value); }

    public static readonly BindableProperty FormatProperty = BindableProperty.Create(
            nameof(Format), typeof(string), typeof(TimePickerField), TimePicker.FormatProperty.DefaultValue,
            propertyChanged: (bindable, oldValue, newValue) => (bindable as TimePickerField).TimePickerView.Format = (string)newValue);

    public Color TextColor { get => (Color)GetValue(TextColorProperty); set => SetValue(TextColorProperty, value); }

    public static readonly BindableProperty TextColorProperty = BindableProperty.Create(
        nameof(TextColor), typeof(Color), typeof(TimePickerField), TimePicker.TextColorProperty.DefaultValue,
        propertyChanged: (bindable, oldValue, newValue) => (bindable as TimePickerField).TimePickerView.TextColor = (Color)newValue);

    public double CharacterSpacing { get => (double)GetValue(CharacterSpacingProperty); set => SetValue(CharacterSpacingProperty, value); }

    public static readonly BindableProperty CharacterSpacingProperty = BindableProperty.Create(
        nameof(CharacterSpacing), typeof(double), typeof(TimePickerField), TimePicker.CharacterSpacingProperty.DefaultValue,
        propertyChanged: (bindable, oldValue, newValue) => (bindable as TimePickerField).TimePickerView.CharacterSpacing = (double)newValue);

    public FontAttributes FontAttributes { get => (FontAttributes)GetValue(FontAttributesProperty); set => SetValue(FontAttributesProperty, value); }

    public static readonly BindableProperty FontAttributesProperty = BindableProperty.Create(
        nameof(FontAttributes), typeof(FontAttributes), typeof(TimePickerField), TimePicker.FontAttributesProperty.DefaultValue,
        propertyChanged: (bindable, oldValue, newValue) => (bindable as TimePickerField).TimePickerView.FontAttributes = (FontAttributes)newValue);

    public string FontFamily { get => (string)GetValue(FontFamilyProperty); set => SetValue(FontFamilyProperty, value); }

    public static readonly BindableProperty FontFamilyProperty = BindableProperty.Create(
        nameof(FontFamily), typeof(string), typeof(TimePickerField), TimePicker.FontFamilyProperty.DefaultValue,
        propertyChanged: (bindable, oldValue, newValue) => (bindable as TimePickerField).TimePickerView.FontFamily = (string)newValue);

    [TypeConverter(typeof(FontSizeConverter))]
    public double FontSize { get => (double)GetValue(FontSizeProperty); set => SetValue(FontSizeProperty, value); }

    public static readonly BindableProperty FontSizeProperty = BindableProperty.Create(
        nameof(FontSize), typeof(double), typeof(TimePickerField), TimePicker.FontSizeProperty.DefaultValue,
        propertyChanged: (bindable, oldValue, newValue) => (bindable as TimePickerField).TimePickerView.FontSize = (double)newValue);

    public bool FontAutoScalingEnabled { get => (bool)GetValue(FontAutoScalingEnabledProperty); set => SetValue(FontAutoScalingEnabledProperty, value); }

    public static readonly BindableProperty FontAutoScalingEnabledProperty = BindableProperty.Create(
        nameof(FontAutoScalingEnabled), typeof(bool), typeof(TimePickerField), TimePicker.FontAutoScalingEnabledProperty.DefaultValue,
        propertyChanged: (bindable, oldValue, newValue) => (bindable as TimePickerField).TimePickerView.FontAutoScalingEnabled = (bool)newValue);
}
