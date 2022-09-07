using System.Linq.Expressions;
using System.Xml;

namespace UraniumUI.Material.Controls;

public class CellBindingContext : UraniumBindableObject
{
    private int row;
    public int Row { get => row; set => SetProperty(ref row, value); }
    private int column;
    public int Column { get => column; set => SetProperty(ref column, value); }
    public object Data { get; set; }
    public object Value { get; set; }

    private bool isSelected;

    public bool IsSelected { get => isSelected; set { isSelected = value; OnPropertyChanged(); } }
}
