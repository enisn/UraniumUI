using System.Collections;
using System.Collections.Specialized;

namespace UraniumUI.Material.Controls;
public partial class DataGrid
{
    public IList ItemsSource { get => (IList)GetValue(ItemsSourceProperty); set => SetValue(ItemsSourceProperty, value); }

    public static readonly BindableProperty ItemsSourceProperty =
        BindableProperty.Create(nameof(ItemsSource), typeof(IList), typeof(DataGrid), propertyChanged: (bo, ov, nv) => (bo as DataGrid).OnItemSourceSet(ov as IList, nv as IList));

    public DataTemplate CellItemTemplate { get => (DataTemplate)GetValue(CellItemTemplateProperty); set => SetValue(CellItemTemplateProperty, value); }

    public static readonly BindableProperty CellItemTemplateProperty =
        BindableProperty.Create(nameof(CellItemTemplate), typeof(DataTemplate), typeof(DataGrid), defaultValue: null,
            propertyChanged: (bo, ov, nv) => (bo as DataGrid).Render());

    public Color LineSeperatorColor { get => (Color)GetValue(LineSeperatorColorProperty); set => SetValue(LineSeperatorColorProperty, value); }
    
    public static readonly BindableProperty LineSeperatorColorProperty =
        BindableProperty.Create(nameof(LineSeperatorColor), typeof(Color), typeof(DataGrid), defaultValue: Colors.Gray,
            propertyChanged: (bo,ov,nv)=> (bo as DataGrid).OnPropertyChanged(nameof(LineSeperatorColor)));
}