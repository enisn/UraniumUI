using Plainer.Maui.Controls;
using System;
using System.Collections;
using System.ComponentModel;
using System.Windows.Input;
using UraniumUI.Converters;
using UraniumUI.Pages;
using UraniumUI.Resources;
using UraniumUI.ViewExtensions;
using UraniumUI.Views;
using Path = Microsoft.Maui.Controls.Shapes.Path;

namespace UraniumUI.Material.Controls;

[ContentProperty(nameof(Validations))]
public class PickerField : InputField
{
    public PickerView PickerView => this.FindByViewQueryIdInVisualTreeDescendants<PickerView>("PickerView");

#if WINDOWS
    protected Label labelSelectedItem => this.FindByViewQueryIdInVisualTreeDescendants<Label>("LabelSelectedItem");
#endif

    public override bool HasValue => SelectedItem != null;

    public override View Content { get; set; } = new PickerView
    {
        VerticalOptions = LayoutOptions.Center,
        Margin = new Thickness(15, 0),
#if WINDOWS
        Opacity = 0,
#endif
    };

    public event EventHandler<object> SelectedValueChanged;

    public PickerField()
    {
        var pickerView = Content as PickerView;
        pickerView.SetId("PickerView");

        Content = pickerView;

        UpdateClearIconState();
        pickerView.SetBinding(PickerView.SelectedItemProperty, new Binding(nameof(SelectedItem), source: this));
        pickerView.SetBinding(PickerView.SelectedIndexProperty, new Binding(nameof(SelectedIndex), source: this));
        pickerView.SetBinding(PickerView.IsEnabledProperty, new Binding(nameof(IsEnabled), source: this));
        pickerView.SetBinding(PickerView.ItemsSourceProperty, new Binding(nameof(ItemsSource), source: this));
        pickerView.SetBinding(PickerView.FontAttributesProperty, new Binding(nameof(FontAttributes), source: this));
        pickerView.SetBinding(PickerView.FontFamilyProperty, new Binding(nameof(FontFamily), source: this));
        pickerView.SetBinding(PickerView.FontSizeProperty, new Binding(nameof(FontSize), source: this));
        pickerView.SetBinding(PickerView.TextColorProperty, new Binding(nameof(TextColor), source: this));
        pickerView.SetBinding(PickerView.CharacterSpacingProperty, new Binding(nameof(CharacterSpacing), source: this));

        // TODO: Move platform specific codes into separate files.
#if ANDROID
// TODO: Possible memory leak here. Need to do without event handler.
		pickerView.HandlerChanged += (s, e) =>
		{
			if (pickerView.Handler != null)
			{
				var editText = pickerView.Handler.PlatformView as AndroidX.AppCompat.Widget.AppCompatEditText;
				editText.AfterTextChanged += (_, args) =>
				{
					editText.ClearFocus();
				};
			}
		};
#endif
#if WINDOWS
        innerGrid.Add(CreateLabelSelectedItem(), column: 1);
#endif
    }

#if WINDOWS
    protected override void OnSizeAllocated(double width, double height)
    {
        base.OnSizeAllocated(width, height);
        if (PickerView is null)
        {
            return;
        }

        //PickerView.MinimumWidthRequest = (width * .96f) - (AllowClear ? iconClear.Width : 0) - (imageIcon.IsValueCreated ? imageIcon.Value.Width : 0);
    }

    protected Label CreateLabelSelectedItem()
    {
        var selectedLabel = new Label
        {
            InputTransparent = true,
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(15, 0),
            TextColor = ColorResource.GetColor("OnBackground", "OnBackgroundDark", Colors.Gray)
        };
        selectedLabel.SetId("LabelSelectedItem");

        return selectedLabel;
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
        var existing = endIconsContainer.FindByViewQueryIdInVisualTreeDescendants<StatefulContentView>("ClearIcon");

        if (AllowClear)
        {
            if (existing == null)
            {
                var iconClear = CreateIconClear();
                endIconsContainer.Add(iconClear);
            }
        }
        else
        {
            endIconsContainer?.Remove(existing);
        }
    }

    public override void ResetValidation()
    {
        PickerView.SelectedItem = null;
        base.ResetValidation();
    }

    public IList<string> Items => PickerView?.Items;

    private static readonly ControlTemplate clearIconPathControlTemplate = new ControlTemplate(() => new Path
    {
        StyleClass = new[] { "PickerField.ClearIcon" },
        Data = UraniumShapes.X,
        Fill = ColorResource.GetColor("OnBackground", "OnBackgroundDark", Colors.DarkGray).WithAlpha(.5f),
    });

    protected virtual View CreateIconClear()
    {
        var iconClear = new StatefulContentView
        {
            VerticalOptions = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.End,
            IsVisible = false,
            Padding = 10,
            TappedCommand = new Command(OnClearTapped),
            ControlTemplate = clearIconPathControlTemplate
        };

        iconClear.SetId("ClearIcon");

        iconClear.SetBinding(StatefulContentView.IsVisibleProperty, new Binding(nameof(SelectedItem), converter: UraniumConverters.StringIsNotNullOrEmptyConverter, source: this));

        return iconClear;
    }

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
       defaultValue: Picker.ItemsSourceProperty.DefaultValue);

    public BindingBase ItemDisplayBinding { get => PickerView.ItemDisplayBinding; set => PickerView.ItemDisplayBinding = value; }

    public Color TextColor { get => (Color)GetValue(TextColorProperty); set => SetValue(TextColorProperty, value); }

    public static readonly BindableProperty TextColorProperty = BindableProperty.Create(
        nameof(TextColor), typeof(Color), typeof(PickerField), Picker.TextColorProperty.DefaultValue);

    public double CharacterSpacing { get => (double)GetValue(CharacterSpacingProperty); set => SetValue(CharacterSpacingProperty, value); }

    public static readonly BindableProperty CharacterSpacingProperty = BindableProperty.Create(
        nameof(CharacterSpacing), typeof(double), typeof(PickerField), Picker.CharacterSpacingProperty.DefaultValue);

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
