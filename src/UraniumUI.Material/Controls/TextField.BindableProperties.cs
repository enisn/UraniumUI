using Microsoft.Maui.Converters;
using System.ComponentModel;
using System.Windows.Input;
using UraniumUI.Resources;

namespace UraniumUI.Material.Controls;

public partial class TextField
{
    public string Text { get => (string)GetValue(TextProperty); set => SetValue(TextProperty, value); }

    public static readonly BindableProperty TextProperty = BindableProperty.Create(
        nameof(Text),
        typeof(string),
        typeof(TextField),
        string.Empty,
        BindingMode.TwoWay, propertyChanging: (bindable, oldValue, newValue) =>
        {
            var textField = (TextField)bindable;
            textField.UpdateClearIconState();
        });

    public Color TextColor { get => (Color)GetValue(TextColorProperty); set => SetValue(TextColorProperty, value); }

    public static readonly BindableProperty TextColorProperty = BindableProperty.Create(
        nameof(TextColor),
        typeof(Color),
        typeof(TextField),
        ColorResource.GetColor("OnBackground", "OnBackgroundDark", Colors.DarkGray),
        propertyChanged: (bindable, oldValue, newValue) => (bindable as TextField).EntryView.TextColor = (Color)newValue);

    public string FontFamily { get => (string)GetValue(FontFamilyProperty); set => SetValue(FontFamilyProperty, value); }

    public static readonly BindableProperty FontFamilyProperty = BindableProperty.Create(
        nameof(FontFamily),
        typeof(string),
        typeof(TextField),
        propertyChanged: (bindable, oldValue, newValue) =>
        {
            var textField = (bindable as TextField);

            textField.EntryView.FontFamily = (string)newValue;
            textField.labelTitle.FontFamily = (string)newValue;
        });

    [TypeConverter(typeof(KeyboardTypeConverter))]
    public Keyboard Keyboard { get => (Keyboard)GetValue(KeyboardProperty); set => SetValue(KeyboardProperty, value); }

    public static readonly BindableProperty KeyboardProperty = BindableProperty.Create(
        nameof(Keyboard),
        typeof(Keyboard),
        typeof(TextField),
        Keyboard.Default,
        propertyChanged: (bindable, oldValue, newValue) => (bindable as TextField).EntryView.Keyboard = (Keyboard)newValue);

    public ClearButtonVisibility ClearButtonVisibility { get => (ClearButtonVisibility)GetValue(ClearButtonVisibilityProperty); set => SetValue(ClearButtonVisibilityProperty, value); }

    public static readonly BindableProperty ClearButtonVisibilityProperty = BindableProperty.Create(
        nameof(ClearButtonVisibility),
        typeof(ClearButtonVisibility),
        typeof(TextField),
        ClearButtonVisibility.WhileEditing,
        propertyChanged: (bindable, oldValue, newValue) => (bindable as TextField).EntryView.ClearButtonVisibility = (ClearButtonVisibility)newValue);

    public bool IsPassword { get => (bool)GetValue(IsPasswordProperty); set => SetValue(IsPasswordProperty, value); }

    public static readonly BindableProperty IsPasswordProperty = BindableProperty.Create(
        nameof(IsPassword),
        typeof(bool),
        typeof(TextField),
        false,
        propertyChanged: (bindable, oldValue, newValue) => (bindable as TextField).EntryView.IsPassword = (bool)newValue);

    public object ReturnCommandParameter { get => GetValue(ReturnCommandParameterProperty); set => SetValue(ReturnCommandParameterProperty, value); }

    public static readonly BindableProperty ReturnCommandParameterProperty = BindableProperty.Create(
        nameof(ReturnCommandParameter),
        typeof(object),
        typeof(TextField));

    public ICommand ReturnCommand { get => (ICommand)GetValue(ReturnCommandProperty); set => SetValue(ReturnCommandProperty, value); }

    public static readonly BindableProperty ReturnCommandProperty = BindableProperty.Create(
        nameof(ReturnCommand),
        typeof(ICommand),
        typeof(TextField), defaultBindingMode: BindingMode.TwoWay);

    public double CharacterSpacing { get => (double)GetValue(CharacterSpacingProperty); set => SetValue(CharacterSpacingProperty, value); }

    public static readonly BindableProperty CharacterSpacingProperty = BindableProperty.Create(
        nameof(CharacterSpacing),
        typeof(double),
        typeof(TextField),
        propertyChanged: (bindable, oldValue, newValue) => (bindable as TextField).EntryView.CharacterSpacing = (double)newValue);

    public ReturnType ReturnType { get => (ReturnType)GetValue(ReturnTypeProperty); set => SetValue(ReturnTypeProperty, value); }

    public static readonly BindableProperty ReturnTypeProperty = BindableProperty.Create(
        nameof(ReturnType),
        typeof(ReturnType),
        typeof(TextField),
        propertyChanged: (bindable, oldValue, newValue) => (bindable as TextField).EntryView.ReturnType = (ReturnType)newValue);

    public int SelectionLength { get => (int)GetValue(SelectionLengthProperty); set => SetValue(SelectionLengthProperty, value); }

    public static readonly BindableProperty SelectionLengthProperty = BindableProperty.Create(
        nameof(SelectionLength),
        typeof(int),
        typeof(TextField));

    public int CursorPosition { get => (int)GetValue(CursorPositionProperty); set => SetValue(CursorPositionProperty, value); }

    public static readonly BindableProperty CursorPositionProperty = BindableProperty.Create(
        nameof(CursorPosition),
        typeof(int),
        typeof(TextField));

    public bool IsTextPredictionEnabled { get => (bool)GetValue(IsTextPredictionEnabledProperty); set => SetValue(IsTextPredictionEnabledProperty, value); }

    public static readonly BindableProperty IsTextPredictionEnabledProperty = BindableProperty.Create(
        nameof(IsTextPredictionEnabled),
        typeof(bool),
        typeof(TextField),
        propertyChanged: (bindable, oldValue, newValue) => (bindable as TextField).EntryView.IsTextPredictionEnabled = (bool)newValue);

    public int MaxLength { get => (int)GetValue(MaxLengthProperty); set => SetValue(MaxLengthProperty, value); }

    public static readonly BindableProperty MaxLengthProperty = BindableProperty.Create(
        nameof(MaxLength),
        typeof(int),
        typeof(TextField),
        propertyChanged: (bindable, oldValue, newValue) => (bindable as TextField).EntryView.MaxLength = (int)newValue);

    public bool AllowClear { get => (bool)GetValue(AllowClearProperty); set => SetValue(AllowClearProperty, value); }

    public static BindableProperty AllowClearProperty = BindableProperty.Create(
        nameof(AllowClear),
        typeof(bool), typeof(TextField),
        false,
        propertyChanged: (bindable, oldValue, newValue) => (bindable as TextField).OnAllowClearChanged());

    public new bool IsReadOnly { get => (bool)GetValue(IsReadOnlyProperty); set => SetValue(IsReadOnlyProperty, value); }

    public static readonly BindableProperty IsReadOnlyProperty = BindableProperty.Create(
        nameof(IsReadOnly),
        typeof(bool),
        typeof(TextField),
        false);

    [System.ComponentModel.TypeConverter(typeof(FontSizeConverter))]
    public double FontSize { get => (double)GetValue(FontSizeProperty); set => SetValue(FontSizeProperty, value); }

    public static readonly BindableProperty FontSizeProperty = BindableProperty.Create(
        nameof(FontSize),
        typeof(double),
        typeof(InputField),
        defaultValue: Label.FontSizeProperty.DefaultValue,
        propertyChanged: (bindable, oldValue, newValue) => (bindable as TextField).EntryView.FontSize = (double)newValue
        );

    public FontAttributes FontAttributes { get => (FontAttributes)GetValue(FontAttributesProperty); set => SetValue(FontAttributesProperty, value); }

    public static readonly BindableProperty FontAttributesProperty = BindableProperty.Create(
        nameof(FontAttributes),
        typeof(FontAttributes),
        typeof(TextField),
        defaultValue: Entry.FontAttributesProperty.DefaultValue,
        propertyChanged: (bindable, oldValue, newValue) => (bindable as TextField).EntryView.FontAttributes = (FontAttributes)newValue
        );

    public TextAlignment HorizontalTextAlignment { get => (TextAlignment)GetValue(HorizontalTextAlignmentProperty); set => SetValue(HorizontalTextAlignmentProperty, value); }

    public static readonly BindableProperty HorizontalTextAlignmentProperty = BindableProperty.Create(
        nameof(HorizontalTextAlignment),
        typeof(TextAlignment),
        typeof(TextField),
        defaultValue: Entry.HorizontalTextAlignmentProperty.DefaultValue,
        propertyChanged: (bindable, oldValue, newValue) => (bindable as TextField).EntryView.HorizontalTextAlignment = (TextAlignment)newValue
        );
}