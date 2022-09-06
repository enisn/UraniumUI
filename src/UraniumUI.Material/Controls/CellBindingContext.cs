using System.Linq.Expressions;
using System.Xml;

namespace UraniumUI.Material.Controls;

public class CellBindingContext : BindableObject
{
    public int Row { get; set; }
    public int Column { get; set; }
    public object Data { get; set; }
    public object Value { get; set; }

    private bool isSelected;
    public bool IsSelected { get => isSelected; set { isSelected = value; OnPropertyChanged(); } }
}
