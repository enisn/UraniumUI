using System.ComponentModel;
using System.Reflection;

namespace UraniumUI.Material.Controls;
public class DataGridColumn
{
    public string Title { get; set; }

    public View TitleView { get; set; }

    public DataTemplate CellItemTemplate { get; set; }

    [Obsolete]
    public string PropertyName { get; set; }

    public BindingBase Binding { get; set; }

    [TypeConverter(typeof(GridLengthTypeConverter))]
    public GridLength Width { get; set; } = GridLength.Auto;

    [Obsolete]
    internal PropertyInfo PropertyInfo { get; set; }
}
