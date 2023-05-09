using Plainer.Maui.Controls;
using System.ComponentModel;
using UraniumUI.Pages;
using UraniumUI.Resources;
using UraniumUI.Views;
using Path = Microsoft.Maui.Controls.Shapes.Path;

namespace UraniumUI.Material.Controls;

[ContentProperty(nameof(Validations))]
public class TimePickerField : InputField
{
	public TimePickerView TimePickerView => Content as TimePickerView;
	public override View Content { get; set; } = new TimePickerView
	{
		VerticalOptions = LayoutOptions.Center,
		Margin = new Thickness(10, 0),
#if IOS || MACCATALYST
        Opacity = 0.10,
#else
		Opacity = 0,
#endif
	};

	protected StatefulContentView iconClear = new StatefulContentView
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

	public override bool HasValue => Time != null;

	public TimePickerField()
	{
		iconClear.TappedCommand = new Command(OnClearTapped);

		UpdateClearIconState();

#if MACCATALYST
        labelTitle.InputTransparent = false;
        labelTitle.GestureRecognizers.Add(new TapGestureRecognizer
        {
            Command = new Command(() =>
            {
                if (!HasValue)
                {
                    Time = (TimeSpan)TimePicker.TimeProperty.DefaultValue;
                }
            })
        });
#endif
		TimePickerView.SetBinding(TimePicker.TimeProperty, new Binding(nameof(Time), source: this));
		TimePickerView.SetBinding(TimePicker.IsEnabledProperty, new Binding(nameof(IsEnabled), source: this));
	}

	protected override object GetValueForValidator()
	{
		return Time;
	}

	protected virtual void OnClearTapped(object parameter)
	{
		if (IsEnabled)
		{
			Time = null;
		}
	}

	protected virtual void OnTimeChanged()
	{
		OnPropertyChanged(nameof(Time));
		CheckAndShowValidations();
#if IOS || MACCATALYST
        TimePickerView.Opacity = Time == null ?  0.1 : 1;
#else
		TimePickerView.Opacity = Time == null ? 0 : 1;
#endif
		if (AllowClear)
		{
			iconClear.IsVisible = Time != null;
		}

		UpdateState();
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
			TimePickerView.Margin = new Thickness(5, 1);
		}
	}

	protected virtual void OnAllowClearChanged()
	{
		UpdateClearIconState();
	}

	protected virtual void UpdateClearIconState()
	{
		if (AllowClear)
		{
			if (!endIconsContainer.Contains(iconClear))
			{
				endIconsContainer.Add(iconClear);
			}
		}
		else
		{
			endIconsContainer.Remove(iconClear);
		}
	}

	public TimeSpan? Time { get => (TimeSpan?)GetValue(TimeProperty); set => SetValue(TimeProperty, value); }

	public static readonly BindableProperty TimeProperty =
		BindableProperty.Create(nameof(Time), typeof(TimeSpan?), typeof(TimePickerField), null, defaultBindingMode: BindingMode.TwoWay,
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

	public bool AllowClear { get => (bool)GetValue(AllowClearProperty); set => SetValue(AllowClearProperty, value); }

	public static BindableProperty AllowClearProperty = BindableProperty.Create(
		nameof(AllowClear),
		typeof(bool), typeof(TimePickerField),
		true,
		propertyChanged: (bindable, oldValue, newValue) => (bindable as TimePickerField).OnAllowClearChanged());
}