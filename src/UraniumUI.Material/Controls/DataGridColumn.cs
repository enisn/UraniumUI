using System.ComponentModel;

namespace UraniumUI.Material.Controls;

public class DataGridColumn : BindableObject
{
    public string Title { get => (string)GetValue(TitleProperty); set => SetValue(TitleProperty, value); }

    public static readonly BindableProperty TitleProperty = BindableProperty.Create(
        nameof(Title),
        typeof(string),
        typeof(DataGridColumn),
        propertyChanged: (bindable, oldValue, newValue) =>
        {
            if (bindable is DataGridColumn column)
            {
                column.OnPropertyChanged(nameof(Title));
            }
        });

    public View TitleView { get; set; }

    public DataTemplate CellItemTemplate { get; set; }

    public BindingBase Binding { get; set; }

    [TypeConverter(typeof(GridLengthTypeConverter))]
    public GridLength Width { get; set; } = GridLength.Auto;

    public bool IsVisible { get => (bool)GetValue(IsVisibleProperty); set => SetValue(IsVisibleProperty, value); }

    public static readonly BindableProperty IsVisibleProperty = BindableProperty.Create(
        nameof(IsVisible),
        typeof(bool),
        typeof(DataGridColumn),
        true,
        propertyChanged: (bindable, oldValue, newValue) =>
        {
            if (bindable is DataGridColumn column)
            {
                column.OnPropertyChanged(nameof(IsVisible));
            }
        });
}
