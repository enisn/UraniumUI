using System.ComponentModel;
using UraniumUI.Extensions;

namespace UraniumUI.Layouts;
public class GridLayout : Grid
{
    public int RowCount { get => (int)GetValue(RowCountProperty); set => SetValue(RowCountProperty, value); }

    public static readonly BindableProperty RowCountProperty = BindableProperty.Create(
        nameof(RowCount), typeof(int), typeof(GridLayout),
        defaultValue: 2,
        propertyChanged: (bindable, oldValue, newValue) =>
        {
            if (bindable is GridLayout grid)
            {
                grid.SetRowDefinitions();
            }
        });

    public int ColumnCount { get => (int)GetValue(ColumnCountProperty); set => SetValue(ColumnCountProperty, value); }

    public static readonly BindableProperty ColumnCountProperty = BindableProperty.Create(
        nameof(ColumnCount), typeof(int), typeof(GridLayout),
        defaultValue: 2,
        propertyChanged: (bindable, oldValue, newValue) =>
        {
            if (bindable is GridLayout grid)
            {
                grid.SetColumnDefinitions();
            }
        });

    [TypeConverter(typeof(GridLengthTypeConverter))]
    public GridLength ColumnGridLength { get => (GridLength)GetValue(ColumnGridLengthProperty); set => SetValue(ColumnGridLengthProperty, value); }

    public static readonly BindableProperty ColumnGridLengthProperty = BindableProperty.Create(
        nameof(ColumnGridLength), typeof(GridLength), typeof(GridLayout), GridLength.Auto,
        propertyChanged: (bindable, oldValue, newValue) =>
        {
            if (bindable is GridLayout grid)
            {
                grid.SetColumnDefinitions();
            }
        });

    [TypeConverter(typeof(GridLengthTypeConverter))]
    public GridLength RowGridLength { get => (GridLength)GetValue(RowGridLengthProperty); set => SetValue(RowGridLengthProperty, value); }

    public static readonly BindableProperty RowGridLengthProperty = BindableProperty.Create(
        nameof(RowGridLength), typeof(GridLength), typeof(GridLayout), GridLength.Auto,
        propertyChanged: (bindable, oldValue, newValue) =>
        {
            if (bindable is GridLayout grid)
            {
                grid.SetRowDefinitions();
            }
        });

    protected override void OnParentSet()
    {
        base.OnParentSet();
        SetDefinitions();
    }

    protected override void OnChildAdded(Element child)
    {
        base.OnChildAdded(child);

        var view = child as View;
        //this.Children.Remove(view);

        var (row, column) = GetNextAvailablePosition();

        Grid.SetRow(view, row);
        Grid.SetColumn(view, column);
        //this.Add(view);
    }

    protected virtual void SetDefinitions()
    {
        SetColumnDefinitions();
        SetRowDefinitions();
    }

    protected virtual void SetColumnDefinitions()
    {
        ColumnDefinitions.Clear();
        for (int i = 0; i < ColumnCount; i++)
        {
            ColumnDefinitions.Add(new ColumnDefinition(ColumnGridLength));
        }
    }

    protected virtual void SetRowDefinitions()
    {
        RowDefinitions.Clear();
        for (int i = 0; i < RowCount; i++)
        {
            RowDefinitions.Add(new RowDefinition(RowGridLength));
        }
    }

    private (int, int) GetNextAvailablePosition()
    {
        for (int row = 0; row < RowCount; row++)
        {
            for (int column = 0; column < ColumnCount; column++)
            {
                if (IsCellAvailable(row, column))
                {
                    return (row, column);
                }
            }
        }

        return (0, 0);
    }

    private bool IsCellAvailable(int row, int column)
    {
        foreach (var child in Children.Take(Children.Count - 1))
        {
            if (child is View view)
            {
                int childRow = (int)view.GetValue(RowProperty);
                int childColumn = (int)view.GetValue(ColumnProperty);
                int childRowSpan = (int)view.GetValue(RowSpanProperty);
                int childColumnSpan = (int)view.GetValue(ColumnSpanProperty);

                if (row >= childRow && row < childRow + childRowSpan &&
                                       column >= childColumn && column < childColumn + childColumnSpan)
                {
                    return false;
                }
            }
        }

        return true;
    }

    private static void FindEndPoint(View view, out int row, out int column)
    {
        row = (int)view.GetValue(RowProperty);
        column = (int)view.GetValue(ColumnProperty);
        int rowSpan = (int)view.GetValue(RowSpanProperty);
        int columnSpan = (int)view.GetValue(ColumnSpanProperty);

        row += rowSpan;
        column += columnSpan;
    }
}
