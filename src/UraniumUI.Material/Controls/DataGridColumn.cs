using System.Reflection;

namespace UraniumUI.Material.Controls;
public class DataGridColumn
{
    public string Title { get; set; }
    public DataTemplate CellItemTemplate { get; set; }
    public string PropertyName { get; set; }
    internal PropertyInfo PropertyInfo { get; set; }
}
