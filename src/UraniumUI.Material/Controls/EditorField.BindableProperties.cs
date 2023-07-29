using Microsoft.Maui.Converters;
using System.ComponentModel;
using UraniumUI.Resources;

namespace UraniumUI.Material.Controls;
public partial class EditorField
{
    public string Text { get => (string)GetValue(TextProperty); set => SetValue(TextProperty, value); }

    public static readonly BindableProperty TextProperty = BindableProperty.Create(
        nameof(Text),
        typeof(string),
        typeof(EditorField),
        string.Empty,
        BindingMode.TwoWay);

    public Color TextColor { get => (Color)GetValue(TextColorProperty); set => SetValue(TextColorProperty, value); }

    public static readonly BindableProperty TextColorProperty = BindableProperty.Create(
        nameof(TextColor),
        typeof(Color),
        typeof(EditorField),
        ColorResource.GetColor("OnBackground", "OnBackgroundDark", Colors.DarkGray),
        propertyChanged: (bindable, oldValue, newValue) => (bindable as EditorField).EditorView.TextColor = (Color)newValue);

    public string FontFamily { get => (string)GetValue(FontFamilyProperty); set => SetValue(FontFamilyProperty, value); }

    public static readonly BindableProperty FontFamilyProperty = BindableProperty.Create(
        nameof(FontFamily),
        typeof(string),
        typeof(EditorField),
        propertyChanged: (bindable, oldValue, newValue) =>
        {
            var editorField = (bindable as EditorField);

            editorField.EditorView.FontFamily = (string)newValue;
            editorField.labelTitle.FontFamily = (string)newValue;
        });

    [TypeConverter(typeof(KeyboardTypeConverter))]
    public Keyboard Keyboard { get => (Keyboard)GetValue(KeyboardProperty); set => SetValue(KeyboardProperty, value); }

    public static readonly BindableProperty KeyboardProperty = BindableProperty.Create(
        nameof(Keyboard),
        typeof(Keyboard),
        typeof(EditorField),
        Keyboard.Default,
        propertyChanged: (bindable, oldValue, newValue) => (bindable as EditorField).EditorView.Keyboard = (Keyboard)newValue);

    public double CharacterSpacing { get => (double)GetValue(CharacterSpacingProperty); set => SetValue(CharacterSpacingProperty, value); }

    public static readonly BindableProperty CharacterSpacingProperty = BindableProperty.Create(
        nameof(CharacterSpacing),
        typeof(double),
        typeof(EditorField),
        propertyChanged: (bindable, oldValue, newValue) => (bindable as EditorField).EditorView.CharacterSpacing = (double)newValue);

    public int SelectionLength { get => (int)GetValue(SelectionLengthProperty); set => SetValue(SelectionLengthProperty, value); }

    public static readonly BindableProperty SelectionLengthProperty = BindableProperty.Create(
        nameof(SelectionLength),
        typeof(int),
        typeof(EditorField));

    public int CursorPosition { get => (int)GetValue(CursorPositionProperty); set => SetValue(CursorPositionProperty, value); }

    public static readonly BindableProperty CursorPositionProperty = BindableProperty.Create(
        nameof(CursorPosition),
        typeof(int),
        typeof(EditorField));

    public bool IsTextPredictionEnabled { get => (bool)GetValue(IsTextPredictionEnabledProperty); set => SetValue(IsTextPredictionEnabledProperty, value); }

    public static readonly BindableProperty IsTextPredictionEnabledProperty = BindableProperty.Create(
        nameof(IsTextPredictionEnabled),
        typeof(bool),
        typeof(EditorField),
        propertyChanged: (bindable, oldValue, newValue) => (bindable as EditorField).EditorView.IsTextPredictionEnabled = (bool)newValue);


    public int MaxLength { get => (int)GetValue(MaxLengthProperty); set => SetValue(MaxLengthProperty, value); }

    public static readonly BindableProperty MaxLengthProperty = BindableProperty.Create(
        nameof(MaxLength),
        typeof(int),
        typeof(EditorField),
        propertyChanged: (bindable, oldValue, newValue) => (bindable as EditorField).EditorView.MaxLength = (int)newValue);

    public bool IsReadOnly { get => (bool)GetValue(IsReadOnlyProperty); set => SetValue(IsReadOnlyProperty, value); }

    public static readonly BindableProperty IsReadOnlyProperty = BindableProperty.Create(
        nameof(IsReadOnly),
        typeof(bool),
        typeof(EditorField),
        false,
        propertyChanged: (bindable, oldValue, newValue) => (bindable as EditorField).EditorView.IsReadOnly = (bool)newValue);

    public bool IsSpellCheckEnabled { get => (bool)GetValue(IsSpellCheckEnabledProperty); set => SetValue(IsSpellCheckEnabledProperty, value); }

    public static readonly BindableProperty IsSpellCheckEnabledProperty = BindableProperty.Create(
        nameof(IsSpellCheckEnabled),
        typeof(bool),
        typeof(EditorField),
        false,
        propertyChanged: (bindable, oldValue, newValue) => (bindable as EditorField).EditorView.IsSpellCheckEnabled = (bool)newValue);
}