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

  
    public Color TextColor { get => (Color)GetValue(TextColorProperty); set => SetValue(TextColorProperty, value); }

    public static readonly BindableProperty TextColorProperty = BindableProperty.Create(
        nameof(TextColor),
        typeof(Color),
        typeof(TextField),
        ColorResource.GetColor("OnBackground", "OnBackgroundDark", Colors.DarkGray),
        propertyChanged: (bindable, oldValue, newValue) => (bindable as TextField).mainEntry.TextColor = (Color)newValue);
   }