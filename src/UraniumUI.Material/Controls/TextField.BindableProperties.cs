using Microsoft.Maui.Platform;
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
        BindingMode.TwoWay,
        propertyChanged: (bindable, oldValue, newValue) => (bindable as TextField).OnPropertyChanged(nameof(Text)));

    public string Title { get => (string)GetValue(TitleProperty); set => SetValue(TitleProperty, value); }

    public static readonly BindableProperty TitleProperty = BindableProperty.Create(
        nameof(Title),
        typeof(string),
        typeof(TextField),
        string.Empty,
        propertyChanged: (bindable, oldValue, newValue) =>
        {
            var textField = (bindable as TextField);
            textField.labelTitle.Text = (string)newValue;
            textField.InitializeBorder();
        });

    public Color AccentColor { get => (Color)GetValue(AccentColorProperty); set => SetValue(AccentColorProperty, value); }

    public static readonly BindableProperty AccentColorProperty = BindableProperty.Create(
        nameof(AccentColor),
        typeof(Color),
        typeof(TextField),
        ColorResource.GetColor("Primary", "PrimaryDark", Colors.Purple),
        propertyChanged: (bindable, oldValue, newValue) => (bindable as TextField).OnPropertyChanged(nameof(AccentColor)));

    public Color TextColor { get => (Color)GetValue(TextColorProperty); set => SetValue(TextColorProperty, value); }

    public static readonly BindableProperty TextColorProperty = BindableProperty.Create(
        nameof(TextColor),
        typeof(Color),
        typeof(TextField),
        ColorResource.GetColor("OnBackground", "OnBackgroundDark", Colors.DarkGray),
        propertyChanged: (bindable, oldValue, newValue) => (bindable as TextField).mainEntry.TextColor = (Color)newValue);

    public Color LabelColor { get => (Color)GetValue(LabelColorProperty); set => SetValue(LabelColorProperty, value); }

    public static readonly BindableProperty LabelColorProperty = BindableProperty.Create(
        nameof(LabelColor),
        typeof(Color),
        typeof(TextField),
        ColorResource.GetColor("OnBackground", "OnBackgroundDark", Colors.Gray),
        propertyChanged: (bindable, oldValue, newValue) => (bindable as TextField).labelTitle.TextColor = (Color)newValue
        );

    public Color BorderColor { get => (Color)GetValue(BorderColorProperty); set => SetValue(BorderColorProperty, value); }

    public static readonly BindableProperty BorderColorProperty = BindableProperty.Create(
        nameof(BorderColor),
        typeof(Color),
        typeof(TextField),
        ColorResource.GetColor("OnBackground", "OnBackgroundDark", Colors.Gray),
        propertyChanged: (bindable, oldValue, newValue) => (bindable as TextField).labelTitle.TextColor = (Color)newValue);
}