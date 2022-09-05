using System.Collections;
using System.Collections.Specialized;
using System.Reflection;

namespace UraniumUI.Material.Controls;

public partial class DataGrid : Frame
{
    private Grid _rootGrid = new Grid();
    public Type CurrentType { get; protected set; }

    public PropertyInfo[] Columns { get; protected set; }

    public DataGrid()
    {
        InitializeFactoryMethods();
        _rootGrid.HorizontalOptions = LayoutOptions.Fill;
        this.Padding = new Thickness(0, 10);
        this.Content = _rootGrid;
    }

    private void OnItemSourceSet(IList oldSource, IList newSource)
    {
        var sourceType = newSource.GetType();
        if (sourceType.GenericTypeArguments.Length != 1)
        {
            throw new InvalidOperationException("DataGrid collection must be a generic typed collection like List<T>.");
        }

        CurrentType = sourceType.GenericTypeArguments.First();
        Columns = CurrentType.GetProperties(BindingFlags.Instance | BindingFlags.Public);

        if (oldSource is INotifyCollectionChanged oldObservable)
        {
            oldObservable.CollectionChanged -= ItemsSource_CollectionChanged;
        }

        if (newSource is INotifyCollectionChanged newObservable)
        {
            newObservable.CollectionChanged += ItemsSource_CollectionChanged;
        }

        Render();
    }

    private void ItemsSource_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        Render();
    }

    protected virtual void Render()
    {
        if (Columns == null || Columns.Length == 0)
        {
            return; // Not ready yet.
        }

        var tableHeaderRows = 1;
        ResetGrid();
        ConfigureGridDefinitions(ItemsSource.Count + tableHeaderRows, Columns.Length);

        AddTableHeaders();

        for (int i = 0; i < ItemsSource.Count; i++)
        {
            var row = i + tableHeaderRows; // +1 is table header.

            AddRow(row, ItemsSource[i], i == ItemsSource.Count - 1);
        }
    }

    private void ResetGrid()
    {
        _rootGrid.Children.Clear();
        _rootGrid.RowDefinitions.Clear();
        _rootGrid.ColumnDefinitions.Clear();
    }

    protected virtual void AddTableHeaders()
    {
        for (int i = 0; i < Columns.Length; i++)
        {
            var label = LabelFactory() ?? CreateLabel();
            label.FontAttributes = FontAttributes.Bold;
            // TODO: Use an attribute to localize it.
            label.BindingContext = new CellBindingContext
            {
                Value = Columns[i].Name
            };

            _rootGrid.Add(label, column: i, row: 0);
        }

        AddSeparator(1);
    }

    protected virtual void AddRow(int row, object item, bool isLastRow)
    {
        var actualRow = row * 2;

        for (int columnNumber = 0; columnNumber < Columns.Length; columnNumber++)
        {
            var view = (View)CellItemTemplate?.CreateContent() ?? LabelFactory() ?? CreateLabel();

            view.BindingContext = new CellBindingContext
            {
                Column = columnNumber,
                Row = row,
                Data = item,
                Value = Columns[columnNumber].GetValue(item)
            };

            _rootGrid.Add(view, columnNumber, row: actualRow);
        }

        if (!isLastRow)
        {
            AddSeparator(actualRow + 1);
        }
    }

    protected virtual void AddSeparator(int row)
    {
        var line = HorizontalLineFactory() ?? CreateHorizontalLine();

        Grid.SetColumnSpan(line, Columns.Length);
        _rootGrid.Add(line, 0, row);
    }

    private void ConfigureGridDefinitions(int rows, int columns)
    {
        for (int i = 0; i < (rows + (rows - 1)); i++)
        {
            _rootGrid.AddRowDefinition(new RowDefinition(GridLength.Star));
        }

        for (int i = 0; i < columns; i++)
        {
            _rootGrid.AddColumnDefinition(new ColumnDefinition(GridLength.Auto));
        }
    }

    public class CellBindingContext
    {
        public int Row { get; set; }
        public int Column { get; set; }
        public object Data { get; set; }
        public object Value { get; set; }
    }
}