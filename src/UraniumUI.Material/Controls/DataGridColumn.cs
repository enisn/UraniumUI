using System.ComponentModel;
using System.Globalization;

namespace UraniumUI.Material.Controls;

public class DataGridColumn : BindableObject
{
    public string CellStyleClass
    {
        get => (string)GetValue(CellStyleClassProperty);
        set => SetValue(CellStyleClassProperty, value);
    }

    public static readonly BindableProperty CellStyleClassProperty = BindableProperty.Create(
        nameof(CellStyleClass),
        typeof(string),
        typeof(DataGridColumn));

    public string HeaderStyleClass
    {
        get => (string)GetValue(HeaderStyleClassProperty);
        set => SetValue(HeaderStyleClassProperty, value);
    }

    public static readonly BindableProperty HeaderStyleClassProperty = BindableProperty.Create(
        nameof(HeaderStyleClass),
        typeof(string),
        typeof(DataGridColumn));

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

    [Obsolete("Use ValueBinding instead!")]
    public BindingBase Binding { get => ValueBinding; set => ValueBinding = value; }

    public BindingBase ValueBinding { get; set; }

    [TypeConverter(typeof(GridLengthTypeConverter))]
    public GridLength Width
	{
		get => (GridLength) GetValue(WidthProperty); set => SetValue(WidthProperty, value);
	}
    public static readonly BindableProperty WidthProperty = BindableProperty.Create(
	nameof(Width),
	typeof(GridLength),
	typeof(DataGridColumn),
	GridLength.Auto,
	propertyChanged: (bindable, oldValue, newValue) =>
	{
	    if (bindable is DataGridColumn column)
	    {
		    column.OnPropertyChanged(nameof(Width));
	    }
	});
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
