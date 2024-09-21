using System.Collections;
using System.Windows.Input;
using UraniumUI.Controls;
using UraniumUI.Pages;
using UraniumUI.Resources;
using UraniumUI.Views;
using Path = Microsoft.Maui.Controls.Shapes.Path;

namespace UraniumUI.Material.Controls;
public class DropdownField : InputField
{
    public Dropdown DropdownView => Content as Dropdown;

    public override View Content { get; set; } = new Dropdown
    {
        VerticalOptions = LayoutOptions.Center,
#if WINDOWS
        Margin = new Thickness(5, 0, 0, 0),
#endif
        HorizontalOptions = LayoutOptions.Fill,
        StyleClass = new List<string> { "InputField.Dropdown" }
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

    public override bool HasValue => SelectedItem != null;

    public event EventHandler<object> SelectedItemChanged;

    public DropdownField()
    {
        base.RegisterForEvents();

        iconClear.TappedCommand = new Command(OnClearTapped);
        UpdateClearIconState();

        DropdownView.SetBinding(Dropdown.SelectedItemProperty, new Binding(nameof(SelectedItem), BindingMode.TwoWay, source: this));
        DropdownView.SetBinding(Dropdown.ItemsSourceProperty, new Binding(nameof(ItemsSource), source: this));
        DropdownView.SetBinding(Dropdown.IsEnabledProperty, new Binding(nameof(IsEnabled), source: this));
        DropdownView.SetBinding(Dropdown.FontSizeProperty, new Binding(nameof(FontSize), source: this));
        DropdownView.SetBinding(Dropdown.FontAutoScalingEnabledProperty, new Binding(nameof(FontAutoScalingEnabled), source: this));
        DropdownView.SetBinding(Dropdown.FontFamilyProperty, new Binding(nameof(FontFamily), source: this));
    }

    protected override object GetValueForValidator()
    {
        return SelectedItem;
    }

    public override void ResetValidation()
    {
        SelectedItem = null;
        base.ResetValidation();
    }

    protected virtual void OnClearTapped(object parameter)
    {
        if (IsEnabled)
        {
            SelectedItem = null;
            DropdownView.Unfocus();
        }
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

    protected virtual void OnSelectedItemChanged()
    {
        OnPropertyChanged(nameof(SelectedItem));
        CheckAndShowValidations();

        if (AllowClear)
        {
            iconClear.IsVisible = SelectedItem != null;
        }

        UpdateState();
        SelectedItemChanged?.Invoke(this, SelectedItem);
        SelectedItemChangedCommand?.Execute(SelectedItem);
    }

    protected virtual void OnAllowClearChanged()
    {
        UpdateClearIconState();
    }

    #region BindableProperties

    public BindingBase ItemDisplayBinding { get => DropdownView?.ItemDisplayBinding; set => DropdownView.ItemDisplayBinding = value; }

    public IList ItemsSource { get => (IList)GetValue(ItemsSourceProperty); set => SetValue(ItemsSourceProperty, value); }

    public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(
        nameof(ItemsSource),
        typeof(IEnumerable),
        typeof(DropdownField),
        propertyChanged: (bindable, oldValue, newValue) => (bindable as DropdownField).DropdownView.ItemsSource = (IList)newValue);

    public object SelectedItem { get => GetValue(SelectedItemProperty); set => SetValue(SelectedItemProperty, value); }

    public static readonly BindableProperty SelectedItemProperty = BindableProperty.Create(
        nameof(SelectedItem),
        typeof(object),
        typeof(DropdownField),
        propertyChanged: (bindable, oldValue, newValue) => (bindable as DropdownField).OnSelectedItemChanged());

    public bool AllowClear { get => (bool)GetValue(AllowClearProperty); set => SetValue(AllowClearProperty, value); }

    public static readonly BindableProperty AllowClearProperty = BindableProperty.Create(
        nameof(AllowClear),
        typeof(bool),
        typeof(DropdownField),
        defaultValue: false,
        propertyChanged: (bindable, oldValue, newValue) => (bindable as DropdownField).OnAllowClearChanged());

    public ICommand SelectedItemChangedCommand { get => (ICommand)GetValue(SelectedItemChangedCommandProperty); set => SetValue(SelectedItemChangedCommandProperty, value); }

    public static readonly BindableProperty SelectedItemChangedCommandProperty = BindableProperty.Create(
        nameof(SelectedItemChangedCommand),
        typeof(ICommand),
        typeof(DropdownField));

    public Color TextColor { get => (Color)GetValue(TextColorProperty); set => SetValue(TextColorProperty, value); }

    public static readonly BindableProperty TextColorProperty = BindableProperty.Create(
        nameof(TextColor),
        typeof(Color),
        typeof(DropdownField),
        defaultValue: Dropdown.TextColorProperty.DefaultValue,
        propertyChanged: (bindable, oldValue, newValue) => (bindable as DropdownField).DropdownView.TextColor = (Color)newValue);

    public string FontFamily { get => (string)GetValue(FontFamilyProperty); set => SetValue(FontFamilyProperty, value); }

    public static readonly BindableProperty FontFamilyProperty = BindableProperty.Create(
        nameof(FontFamily),
        typeof(string),
        typeof(DropdownField),
        defaultValue: Dropdown.FontFamilyProperty.DefaultValue,
        propertyChanged: (bindable, oldValue, newValue) => (bindable as DropdownField).DropdownView.FontFamily = (string)newValue);

    public double FontSize { get => (double)GetValue(FontSizeProperty); set => SetValue(FontSizeProperty, value); }
    public static readonly BindableProperty FontSizeProperty = BindableProperty.Create(
        nameof(FontSize),
        typeof(double),
        typeof(DropdownField),
        defaultValue: Dropdown.FontSizeProperty.DefaultValue,
        propertyChanged: (bindable, oldValue, newValue) => (bindable as DropdownField).DropdownView.FontSize = (double)newValue);

    public FontAttributes FontAttributes { get => (FontAttributes)GetValue(FontAttributesProperty); set => SetValue(FontAttributesProperty, value); }
    public static readonly BindableProperty FontAttributesProperty = BindableProperty.Create(
        nameof(FontAttributes),
        typeof(FontAttributes),
        typeof(DropdownField),
        defaultValue: Dropdown.FontAttributesProperty.DefaultValue,
        propertyChanged: (bindable, oldValue, newValue) => (bindable as DropdownField).DropdownView.FontAttributes = (FontAttributes)newValue);

    public TextAlignment HorizontalTextAlignment { get => (TextAlignment)GetValue(HorizontalTextAlignmentProperty); set => SetValue(HorizontalTextAlignmentProperty, value); }

    public static readonly BindableProperty HorizontalTextAlignmentProperty = BindableProperty.Create(
        nameof(HorizontalTextAlignment),
        typeof(TextAlignment),
        typeof(DropdownField),
        defaultValue: Dropdown.HorizontalTextAlignmentProperty.DefaultValue,
        propertyChanged: (bindable, oldValue, newValue) => (bindable as DropdownField).DropdownView.HorizontalTextAlignment = (TextAlignment)newValue);
    #endregion
}
