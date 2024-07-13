using Plainer.Maui.Controls;
using System;
using System.Collections;
using System.ComponentModel;
using System.Windows.Input;
using UraniumUI.Pages;
using UraniumUI.Resources;
using UraniumUI.Views;
using Path = Microsoft.Maui.Controls.Shapes.Path;

namespace UraniumUI.Material.Controls;

[ContentProperty(nameof(Validations))]
public class PickerField : InputField
{
    public PickerView PickerView => Content as PickerView;

    public override View Content { get; set; } = new PickerView
    {
        VerticalOptions = LayoutOptions.Center,
        Margin = new Thickness(15, 0),
#if WINDOWS
        Opacity = 0,
#endif
    };

#if WINDOWS
    Label labelSelectedItem = new Label
    {
        InputTransparent = true,
        HorizontalOptions = LayoutOptions.Start,
        VerticalOptions = LayoutOptions.Center,
        Margin = new Thickness(15, 0),
        TextColor = ColorResource.GetColor("OnBackground", "OnBackgroundDark", Colors.Gray)
    };
#endif

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

    public override bool HasValue => SelectedItem != null;

    public event EventHandler<object> SelectedValueChanged;

    public PickerField()
    {
        iconClear.TappedCommand = new Command(OnClearTapped);

        UpdateClearIconState();
        PickerView.SetBinding(PickerView.SelectedItemProperty, new Binding(nameof(SelectedItem), source: this));
        PickerView.SetBinding(PickerView.SelectedIndexProperty, new Binding(nameof(SelectedIndex), source: this));
        PickerView.SetBinding(PickerView.IsEnabledProperty, new Binding(nameof(IsEnabled), source: this));

        // TODO: Move platform specific codes into separate files.
#if ANDROID
		PickerView.HandlerChanged += (s, e) =>
		{
			if (PickerView.Handler != null)
			{
				var editText = PickerView.Handler.PlatformView as AndroidX.AppCompat.Widget.AppCompatEditText;
				editText.AfterTextChanged += (_, args) =>
				{
					editText.ClearFocus();
				};
			}
		};
#endif
#if WINDOWS
        rootGrid.Add(labelSelectedItem, column: 1);
        PickerView.HorizontalOptions = LayoutOptions.Start;
#endif
    }

#if WINDOWS
    protected override void OnSizeAllocated(double width, double height)
    {
        base.OnSizeAllocated(width, height);
        PickerView.MinimumWidthRequest = (width * .96f) - (AllowClear ? iconClear.Width : 0) - (imageIcon.IsValueCreated ? imageIcon.Value.Width : 0);
    }
#endif

    protected override object GetValueForValidator()
    {
        return SelectedItem;
    }

    protected virtual void OnClearTapped(object parameter)
    {
        if (IsEnabled)
        {
            SelectedItem = null;
            PickerView.Unfocus();
        }
    }

    protected virtual void OnSelectedItemChanged()
    {
        OnPropertyChanged(nameof(SelectedItem));
        CheckAndShowValidations();

        if (AllowClear)
        {
            iconClear.IsVisible = SelectedItem != null;
        }

#if WINDOWS
        if (ItemDisplayBinding != null)
        {
            Binding itemDisplayBinding = (Binding)ItemDisplayBinding;
            string nameOfDisplayProperty = itemDisplayBinding.Path;
            labelSelectedItem.SetBinding(Label.TextProperty, new Binding(nameof(SelectedItem) + '.' + nameOfDisplayProperty, source: this));
        }
        else
        {
            labelSelectedItem.Text = SelectedItem?.ToString();
        }
#endif

        UpdateState();

        SelectedValueChangedCommand?.Execute(SelectedItem);
        SelectedValueChanged?.Invoke(this, SelectedItem);
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

    public override void ResetValidation()
    {
        PickerView.SelectedItem = null;
        base.ResetValidation();
    }

    public IList<string> Items => PickerView.Items;

    #region BindableProperties

    public object SelectedItem { get => GetValue(SelectedItemProperty); set => SetValue(SelectedItemProperty, value); }

    public static readonly BindableProperty SelectedItemProperty = BindableProperty.Create(
        nameof(SelectedItem), typeof(object), typeof(PickerField),
        defaultValue: Picker.SelectedItemProperty.DefaultValue,
        defaultBindingMode: BindingMode.TwoWay,
        propertyChanged: (bindable, oldValue, newValue) => (bindable as PickerField).OnSelectedItemChanged());

    public int SelectedIndex { get => (int)GetValue(SelectedIndexProperty); set => SetValue(SelectedIndexProperty, value); }

    public static readonly BindableProperty SelectedIndexProperty = BindableProperty.Create(
        nameof(SelectedIndex), typeof(int), typeof(PickerField),
        defaultValue: Picker.SelectedIndexProperty.DefaultValue,
        defaultBindingMode: BindingMode.TwoWay);

    public IList ItemsSource { get => (IList)GetValue(ItemsSourceProperty); set => SetValue(ItemsSourceProperty, value); }

    public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(
       nameof(ItemsSource), typeof(IList), typeof(PickerField),
       defaultValue: Picker.ItemsSourceProperty.DefaultValue,
       propertyChanged: (bindable, oldValue, newValue) => (bindable as PickerField).PickerView.ItemsSource = (IList)newValue);

    public BindingBase ItemDisplayBinding { get => PickerView.ItemDisplayBinding; set => PickerView.ItemDisplayBinding = value; }

    public FontAttributes FontAttributes { get => (FontAttributes)GetValue(FontAttributesProperty); set => SetValue(FontAttributesProperty, value); }

    public static readonly BindableProperty FontAttributesProperty = BindableProperty.Create(
       nameof(FontAttributes), typeof(FontAttributes), typeof(PickerField),
       defaultValue: Picker.FontAttributesProperty.DefaultValue,
       propertyChanged: (bindable, oldValue, newValue) =>
       {
           var pickerField = (bindable as PickerField);
           pickerField.PickerView.FontAttributes = (FontAttributes)newValue;
           pickerField.labelTitle.FontAttributes = (FontAttributes)newValue;
#if WINDOWS
           pickerField.labelSelectedItem.FontAttributes = (FontAttributes)newValue;
#endif
       });

    public string FontFamily { get => (string)GetValue(FontFamilyProperty); set => SetValue(FontFamilyProperty, value); }

    public static readonly BindableProperty FontFamilyProperty = BindableProperty.Create(
         nameof(FontFamily), typeof(string), typeof(PickerField),
         defaultValue: Picker.FontFamilyProperty.DefaultValue,
         propertyChanged: (bindable, oldValue, newValue) => (bindable as PickerField).PickerView.FontFamily = (string)newValue);

    [TypeConverter(typeof(FontSizeConverter))]
    public double FontSize { get => (double)GetValue(FontSizeProperty); set => SetValue(FontSizeProperty, value); }

    public static readonly BindableProperty FontSizeProperty = BindableProperty.Create(
        nameof(FontSize), typeof(double), typeof(PickerField), Picker.FontSizeProperty.DefaultValue,
        propertyChanged: (bindable, oldValue, newValue) => (bindable as PickerField).PickerView.FontSize = (double)newValue);

    public bool FontAutoScalingEnabled { get => (bool)GetValue(FontAutoScalingEnabledProperty); set => SetValue(FontAutoScalingEnabledProperty, value); }

    public static readonly BindableProperty FontAutoScalingEnabledProperty = BindableProperty.Create(
        nameof(FontAutoScalingEnabled), typeof(bool), typeof(PickerField), Picker.FontAutoScalingEnabledProperty.DefaultValue,
        propertyChanged: (bindable, oldValue, newValue) => (bindable as PickerField).PickerView.FontAutoScalingEnabled = (bool)newValue);

    public Color TextColor { get => (Color)GetValue(TextColorProperty); set => SetValue(TextColorProperty, value); }

    public static readonly BindableProperty TextColorProperty = BindableProperty.Create(
        nameof(TextColor), typeof(Color), typeof(PickerField), Picker.TextColorProperty.DefaultValue,
        propertyChanged: (bindable, oldValue, newValue) => (bindable as PickerField).PickerView.TextColor = (Color)newValue);

    public double CharacterSpacing { get => (double)GetValue(CharacterSpacingProperty); set => SetValue(CharacterSpacingProperty, value); }

    public static readonly BindableProperty CharacterSpacingProperty = BindableProperty.Create(
        nameof(CharacterSpacing), typeof(double), typeof(PickerField), Picker.CharacterSpacingProperty.DefaultValue,
        propertyChanged: (bindable, oldValue, newValue) => (bindable as PickerField).PickerView.CharacterSpacing = (double)newValue);

    public bool AllowClear { get => (bool)GetValue(AllowClearProperty); set => SetValue(AllowClearProperty, value); }

    public static readonly BindableProperty AllowClearProperty = BindableProperty.Create(
        nameof(AllowClear),
        typeof(bool), typeof(PickerField),
        true,
        propertyChanged: (bindable, oldValue, newValue) => (bindable as PickerField).OnAllowClearChanged());

    public ICommand SelectedValueChangedCommand { get => (ICommand)GetValue(SelectedValueChangedCommandProperty); set => SetValue(SelectedValueChangedCommandProperty, value); }

    public static readonly BindableProperty SelectedValueChangedCommandProperty = BindableProperty.Create(
        nameof(SelectedValueChangedCommand),
        typeof(ICommand), typeof(PickerField),
        defaultValue: null);
    #endregion
}
