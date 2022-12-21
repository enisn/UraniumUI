using System.ComponentModel;
using System.Reflection;

namespace UraniumUI.Material.Controls;
public class DataGridColumn
{
    public string Title { get; set; }

    public View TitleView { get; set; }

    public DataTemplate CellItemTemplate { get; set; }

    [Obsolete("Use Binding property instead. Replace 'PropertyName=\"Name\"' with 'Binding=\"{Binding Name}\"'. Visit https://github.com/enisn/UraniumUI/blob/develop/docs/en/themes/material/components/DataGrid.md for the usage.")]
    public string PropertyName { get; set; }

    public BindingBase Binding { get; set; }

    [TypeConverter(typeof(GridLengthTypeConverter))]
    public GridLength Width { get; set; } = GridLength.Auto;

    [Obsolete]
    internal PropertyInfo PropertyInfo { get; set; }
}
