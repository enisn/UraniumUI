using Plainer.Maui.Controls;
using System.ComponentModel;
using System.Globalization;
using UraniumUI.Controls;
using UraniumUI.Dialogs;
using UraniumUI.Pages;
using UraniumUI.Resources;
using UraniumUI.Views;
using Path = Microsoft.Maui.Controls.Shapes.Path;

namespace UraniumUI.Material.Controls;

[ContentProperty(nameof(Validations))]
public class DatePickerField : InputField
{
    private bool isDatePromptOpen;

    public DatePickerView DatePickerView { get; } = new DatePickerView
    {
        VerticalOptions = LayoutOptions.Center,
#if ANDROID
        Margin = new Thickness(16, 0),
#else
        Margin = new Thickness(10, 0),
#endif
        Opacity = 0,
    };

    public override View Content { get; set; } = new Label
    {
        VerticalOptions = LayoutOptions.Center,
        VerticalTextAlignment = TextAlignment.Center,
        HorizontalOptions = LayoutOptions.Fill,
        LineBreakMode = LineBreakMode.TailTruncation,
        MinimumHeightRequest = 40,
        Margin = new Thickness(10, 0),
        InputTransparent = false,
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

    public override bool HasValue => Date != null;

    protected IDialogService DialogService { get; }

    public DatePickerField()
    {
        base.RegisterForEvents();
        iconClear.TappedCommand = new Command(OnClearTapped);
        DialogService = UraniumServiceProvider.Current.GetRequiredService<IDialogService>();

        Content.GestureRecognizers.Add(new TapGestureRecognizer
        {
            Command = new Command(async () => await OpenDatePromptAsync())
        });

        UpdateClearIconState();
    }

    protected Label DateLabel => Content as Label;

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        UpdateClearIconState();
        UpdateDateText();
    }

    protected override object GetValueForValidator()
    {
        return Date;
    }

    protected virtual void OnClearTapped(object parameter)
    {
        if (IsEnabled)
        {
            Date = null;
        }
    }

    protected virtual async Task OpenDatePromptAsync()
    {
        if (!IsEnabled || isDatePromptOpen)
        {
            return;
        }

        try
        {
            isDatePromptOpen = true;
            Date = await DialogService.DisplayDatePromptAsync(Title, Date, MinimumDate, MaximumDate);
        }
        finally
        {
            isDatePromptOpen = false;
        }
    }

    protected virtual void OnDateChanged()
    {
        OnPropertyChanged(nameof(Date));
        CheckAndShowValidations();
        UpdateDateText();
        UpdateDatePickerViewDate();

        if (AllowClear)
        {
            iconClear.IsVisible = Date != null;
        }

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
    protected virtual void OnAllowClearChanged()
    {
        UpdateClearIconState();
    }

    protected virtual void UpdateDateText()
    {
        if (DateLabel is not null)
        {
            DateLabel.Text = Date?.ToString(Format, CultureInfo.CurrentCulture) ?? string.Empty;
        }
    }

    protected virtual void UpdateDatePickerViewDate()
    {
        var date = (Date ?? GetDatePickerViewFallbackDate()).Date;

        if (MinimumDate.HasValue && date < MinimumDate.Value.Date)
        {
            date = MinimumDate.Value.Date;
        }

        if (MaximumDate.HasValue && date > MaximumDate.Value.Date)
        {
            date = MaximumDate.Value.Date;
        }

        DatePickerView.Date = date;
    }

    protected virtual DateTime GetDatePickerViewFallbackDate()
    {
        return DatePicker.DateProperty.DefaultValue is DateTime date ? date.Date : DateTime.Today;
    }

    protected virtual void UpdateClearIconState()
    {
        if (endIconsContainer is null)
        {
            return;
        }

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

    public override void ResetValidation()
    {
        Date = null;
        base.ResetValidation();
    }

    public DateTime? Date { get => (DateTime?)GetValue(DateProperty); set => SetValue(DateProperty, value); }

    public static readonly BindableProperty DateProperty = BindableProperty.Create(
        nameof(Date), typeof(DateTime?), typeof(DatePickerField),
        defaultValue: null, defaultBindingMode: BindingMode.TwoWay,
        propertyChanged: (bindable, oldValue, newValue) => (bindable as DatePickerField).OnDateChanged()
        );

    public DateTime? MaximumDate { get => (DateTime?)GetValue(MaximumDateProperty); set => SetValue(MaximumDateProperty, value); }

    public static readonly BindableProperty MaximumDateProperty = BindableProperty.Create(
         nameof(MaximumDate), typeof(DateTime?), typeof(DatePickerField),
         defaultValue: null,
         propertyChanged: (bindable, oldValue, newValue) =>
         {
             var datePickerField = bindable as DatePickerField;
             datePickerField.DatePickerView.MaximumDate = (DateTime)(newValue ?? DatePicker.MaximumDateProperty.DefaultValue);
             datePickerField.UpdateDatePickerViewDate();
         }
         );

    public DateTime? MinimumDate { get => (DateTime?)GetValue(MinimumDateProperty); set => SetValue(MinimumDateProperty, value); }

    public static readonly BindableProperty MinimumDateProperty = BindableProperty.Create(
         nameof(MinimumDate), typeof(DateTime?), typeof(DatePickerField),
         defaultValue: null,
         propertyChanged: (bindable, oldValue, newValue) =>
         {
             var datePickerField = bindable as DatePickerField;
             datePickerField.DatePickerView.MinimumDate = (DateTime)(newValue ?? DatePicker.MinimumDateProperty.DefaultValue);
             datePickerField.UpdateDatePickerViewDate();
         }
         );

    public string Format { get => (string)GetValue(FormatProperty); set => SetValue(FormatProperty, value); }

    public static readonly BindableProperty FormatProperty = BindableProperty.Create(
            nameof(Format), typeof(string), typeof(DatePickerField), DatePicker.FormatProperty.DefaultValue,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                var datePickerField = bindable as DatePickerField;
                datePickerField.DatePickerView.Format = (string)newValue;
                datePickerField.UpdateDateText();
            });

    public Color TextColor { get => (Color)GetValue(TextColorProperty); set => SetValue(TextColorProperty, value); }

    public static readonly BindableProperty TextColorProperty = BindableProperty.Create(
        nameof(TextColor), typeof(Color), typeof(DatePickerField), DatePicker.TextColorProperty.DefaultValue,
        propertyChanged: (bindable, oldValue, newValue) =>
        {
            var datePickerField = bindable as DatePickerField;
            datePickerField.DatePickerView.TextColor = (Color)newValue;
            datePickerField.DateLabel?.SetValue(Label.TextColorProperty, newValue);
        });

    public double CharacterSpacing { get => (double)GetValue(CharacterSpacingProperty); set => SetValue(CharacterSpacingProperty, value); }

    public static readonly BindableProperty CharacterSpacingProperty = BindableProperty.Create(
        nameof(CharacterSpacing), typeof(double), typeof(DatePickerField), DatePicker.CharacterSpacingProperty.DefaultValue,
        propertyChanged: (bindable, oldValue, newValue) =>
        {
            var datePickerField = bindable as DatePickerField;
            datePickerField.DatePickerView.CharacterSpacing = (double)newValue;
            datePickerField.DateLabel?.SetValue(Label.CharacterSpacingProperty, newValue);
        });

    public FontAttributes FontAttributes { get => (FontAttributes)GetValue(FontAttributesProperty); set => SetValue(FontAttributesProperty, value); }

    public static readonly BindableProperty FontAttributesProperty = BindableProperty.Create(
        nameof(FontAttributes), typeof(FontAttributes), typeof(DatePickerField), TimePicker.FontAttributesProperty.DefaultValue,
        propertyChanged: (bindable, oldValue, newValue) =>
        {
            var datePickerField = bindable as DatePickerField;
            datePickerField.DatePickerView.FontAttributes = (FontAttributes)newValue;
            datePickerField.DateLabel?.SetValue(Label.FontAttributesProperty, newValue);
        });

    public string FontFamily { get => (string)GetValue(FontFamilyProperty); set => SetValue(FontFamilyProperty, value); }

    public static readonly BindableProperty FontFamilyProperty = BindableProperty.Create(
        nameof(FontFamily), typeof(string), typeof(DatePickerField), TimePicker.FontFamilyProperty.DefaultValue,
        propertyChanged: (bindable, oldValue, newValue) =>
        {
            var datePickerField = bindable as DatePickerField;
            datePickerField.DatePickerView.FontFamily = (string)newValue;
            datePickerField.DateLabel?.SetValue(Label.FontFamilyProperty, newValue);
        });

    [TypeConverter(typeof(FontSizeConverter))]
    public double FontSize { get => (double)GetValue(FontSizeProperty); set => SetValue(FontSizeProperty, value); }

    public static readonly BindableProperty FontSizeProperty = BindableProperty.Create(
        nameof(FontSize), typeof(double), typeof(DatePickerField), TimePicker.FontSizeProperty.DefaultValue,
        propertyChanged: (bindable, oldValue, newValue) =>
        {
            var datePickerField = bindable as DatePickerField;
            datePickerField.DatePickerView.FontSize = (double)newValue;
            datePickerField.DateLabel?.SetValue(Label.FontSizeProperty, newValue);
        });

    public bool FontAutoScalingEnabled { get => (bool)GetValue(FontAutoScalingEnabledProperty); set => SetValue(FontAutoScalingEnabledProperty, value); }

    public static readonly BindableProperty FontAutoScalingEnabledProperty = BindableProperty.Create(
        nameof(FontAutoScalingEnabled), typeof(bool), typeof(DatePickerField), TimePicker.FontAutoScalingEnabledProperty.DefaultValue,
        propertyChanged: (bindable, oldValue, newValue) =>
        {
            var datePickerField = bindable as DatePickerField;
            datePickerField.DatePickerView.FontAutoScalingEnabled = (bool)newValue;
            datePickerField.DateLabel?.SetValue(Label.FontAutoScalingEnabledProperty, newValue);
        });
    public bool AllowClear { get => (bool)GetValue(AllowClearProperty); set => SetValue(AllowClearProperty, value); }

    public static BindableProperty AllowClearProperty = BindableProperty.Create(
        nameof(AllowClear),
        typeof(bool), typeof(DatePickerField),
        true,
        propertyChanged: (bindable, oldValue, newValue) => (bindable as DatePickerField).OnAllowClearChanged());
}
