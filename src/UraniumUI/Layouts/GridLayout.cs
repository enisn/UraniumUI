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

    public GridLayout()
    {
        SetDefinitions();
    }

    protected override void OnChildAdded(Element child)
    {
        base.OnChildAdded(child);

        var view = child as View;
    }

    protected override void OnChildRemoved(Element child, int oldLogicalIndex)
    {
        base.OnChildRemoved(child, oldLogicalIndex);
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
            ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));
        }
    }

    protected virtual void SetRowDefinitions()
    {
        RowDefinitions.Clear();
        for (int i = 0; i < RowCount; i++)
        {
            RowDefinitions.Add(new RowDefinition(GridLength.Auto));
        }
    }

    protected virtual void ArrangeChildren()
    {
        for (int i = 0; i < Children.Count; i++)
        {

        }
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
