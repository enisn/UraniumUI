using Plainer.Maui.Controls;
using System.ComponentModel;
using UraniumUI.Pages;
using UraniumUI.Resources;
using Path = Microsoft.Maui.Controls.Shapes.Path;

namespace UraniumUI.Material.Controls;

[ContentProperty(nameof(Validations))]
public class DatePickerField : InputField
{
    public DatePickerView DatePickerView => Content as DatePickerView;
    public override View Content { get; set; } = new DatePickerView
    {
        VerticalOptions = LayoutOptions.Center,
        Margin = new Thickness(10, 0),
#if IOS || MACCATALYST
        Opacity = 0.10,
#else
        Opacity = 0,
#endif
    };

    protected ContentView iconClear = new ContentView
    {
        VerticalOptions = LayoutOptions.Center,
        HorizontalOptions = LayoutOptions.End,
        IsVisible = false,
        Padding = 10,
        Content = new Path
        {
            Data = UraniumShapes.X,
            Fill = ColorResource.GetColor("OnBackground", "OnBackgroundDark", Colors.DarkGray).WithAlpha(.5f),
        }
    };

    public override bool HasValue => Date != null;

    public DatePickerField()
    {
        var clearGestureRecognizer = new TapGestureRecognizer();
        clearGestureRecognizer.Tapped += OnClearTapped;
        iconClear.GestureRecognizers.Add(clearGestureRecognizer);

        endIconsContainer.Add(iconClear);

        DatePickerView.SetBinding(DatePickerView.DateProperty, new Binding(nameof(Date), source: this));

#if MACCATALYST
        labelTitle.InputTransparent = false;
        labelTitle.GestureRecognizers.Add(new TapGestureRecognizer
        {
            Command = new Command(() =>
            {
                if (!HasValue)
                {
                    Date = (DateTime)DatePicker.DateProperty.DefaultValue;
                }
            })
        });
#endif
    }

    protected override object GetValueForValidator()
    {
        return Date;
    }

    protected void OnClearTapped(object sender, EventArgs e)
    {
        Date = null;
    }

    protected virtual void OnDateChanged()
    {
        OnPropertyChanged(nameof(Date));
        CheckAndShowValidations();

#if IOS || MACCATALYST
        DatePickerView.Opacity = Date == null ? 0.1 : 1;
#else
        DatePickerView.Opacity = Date == null ? 0 : 1;
#endif
        iconClear.IsVisible = Date != null;

        UpdateState();
    }

    protected override void OnIconChanged()
    {
        base.OnIconChanged();

        if (Icon == null)
        {
            DatePickerView.Margin = new Thickness(10, 0);
        }
        else
        {
            DatePickerView.Margin = new Thickness(5, 1);
        }
    }

    public DateTime? Date { get => (DateTime?)GetValue(DateProperty); set => SetValue(DateProperty, value); }

    public static readonly BindableProperty DateProperty = BindableProperty.Create(
        nameof(Date), typeof(DateTime?), typeof(DatePickerField), defaultValue: null, defaultBindingMode: BindingMode.TwoWay,
        propertyChanged: (bindable, oldValue, newValue) => (bindable as DatePickerField).OnDateChanged()
        );

    public DateTime MaximumDate { get => (DateTime)GetValue(MaximumDateProperty); set => SetValue(MaximumDateProperty, value); }

    public static readonly BindableProperty MaximumDateProperty = BindableProperty.Create(
         nameof(MaximumDate), typeof(DateTime?), typeof(DatePickerField), 
         defaultValue: DatePicker.MaximumDateProperty.DefaultValue,
         propertyChanged: (bindable, oldValue, newValue) => (bindable as DatePickerField).DatePickerView.MaximumDate = (DateTime)newValue
         );

    public DateTime MinimumDate { get => (DateTime)GetValue(MinimumDateProperty); set => SetValue(MinimumDateProperty, value); }

    public static readonly BindableProperty MinimumDateProperty = BindableProperty.Create(
         nameof(MinimumDate), typeof(DateTime?), typeof(DatePickerField),
         defaultValue: DatePicker.MinimumDateProperty.DefaultValue,
         propertyChanged: (bindable, oldValue, newValue) => (bindable as DatePickerField).DatePickerView.MinimumDate = (DateTime)newValue
         );

    public string Format { get => (string)GetValue(FormatProperty); set => SetValue(FormatProperty, value); }

    public static readonly BindableProperty FormatProperty = BindableProperty.Create(
            nameof(Format), typeof(string), typeof(DatePickerField), DatePicker.FormatProperty.DefaultValue,
            propertyChanged: (bindable, oldValue, newValue) => (bindable as DatePickerField).DatePickerView.Format = (string)newValue);

    public Color TextColor { get => (Color)GetValue(TextColorProperty); set => SetValue(TextColorProperty, value); }

    public static readonly BindableProperty TextColorProperty = BindableProperty.Create(
        nameof(TextColor), typeof(Color), typeof(DatePickerField), DatePicker.TextColorProperty.DefaultValue,
        propertyChanged: (bindable, oldValue, newValue) => (bindable as DatePickerField).DatePickerView.TextColor = (Color)newValue);

    public double CharacterSpacing { get => (double)GetValue(CharacterSpacingProperty); set => SetValue(CharacterSpacingProperty, value); }

    public static readonly BindableProperty CharacterSpacingProperty = BindableProperty.Create(
        nameof(CharacterSpacing), typeof(double), typeof(DatePickerField), DatePicker.CharacterSpacingProperty.DefaultValue,
        propertyChanged: (bindable, oldValue, newValue) => (bindable as DatePickerField).DatePickerView.CharacterSpacing = (double)newValue);

    public FontAttributes FontAttributes { get => (FontAttributes)GetValue(FontAttributesProperty); set => SetValue(FontAttributesProperty, value); }

    public static readonly BindableProperty FontAttributesProperty = BindableProperty.Create(
        nameof(FontAttributes), typeof(FontAttributes), typeof(DatePickerField), TimePicker.FontAttributesProperty.DefaultValue,
        propertyChanged: (bindable, oldValue, newValue) => (bindable as DatePickerField).DatePickerView.FontAttributes = (FontAttributes)newValue);

    public string FontFamily { get => (string)GetValue(FontFamilyProperty); set => SetValue(FontFamilyProperty, value); }

    public static readonly BindableProperty FontFamilyProperty = BindableProperty.Create(
        nameof(FontFamily), typeof(string), typeof(DatePickerField), TimePicker.FontFamilyProperty.DefaultValue,
        propertyChanged: (bindable, oldValue, newValue) => (bindable as DatePickerField).DatePickerView.FontFamily = (string)newValue);

    [TypeConverter(typeof(FontSizeConverter))]
    public double FontSize { get => (double)GetValue(FontSizeProperty); set => SetValue(FontSizeProperty, value); }

    public static readonly BindableProperty FontSizeProperty = BindableProperty.Create(
        nameof(FontSize), typeof(double), typeof(DatePickerField), TimePicker.FontSizeProperty.DefaultValue,
        propertyChanged: (bindable, oldValue, newValue) => (bindable as DatePickerField).DatePickerView.FontSize = (double)newValue);

    public bool FontAutoScalingEnabled { get => (bool)GetValue(FontAutoScalingEnabledProperty); set => SetValue(FontAutoScalingEnabledProperty, value); }

    public static readonly BindableProperty FontAutoScalingEnabledProperty = BindableProperty.Create(
        nameof(FontAutoScalingEnabled), typeof(bool), typeof(DatePickerField), TimePicker.FontAutoScalingEnabledProperty.DefaultValue,
        propertyChanged: (bindable, oldValue, newValue) => (bindable as DatePickerField).DatePickerView.FontAutoScalingEnabled = (bool)newValue);
}
