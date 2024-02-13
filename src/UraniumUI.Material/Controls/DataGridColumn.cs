using System.ComponentModel;

namespace UraniumUI.Material.Controls;
public class DataGridColumn
{
    public string Title { get; set; }

    public View TitleView { get; set; }

    public DataTemplate CellItemTemplate { get; set; }

    public BindingBase Binding { get; set; }

    [TypeConverter(typeof(GridLengthTypeConverter))]
    public GridLength Width { get; set; } = GridLength.Auto;
}
