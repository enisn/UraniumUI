using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Reflection;

namespace UraniumUI.Material.Controls;

public partial class DataGrid : Frame
{
    private Grid _rootGrid = new Grid();
    public Type CurrentType { get; protected set; }

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

        var columnsAreReady = Columns?.Any() ?? false;

        SetAutoColumns();

        if (oldSource is INotifyCollectionChanged oldObservable)
        {
            oldObservable.CollectionChanged -= ItemsSource_CollectionChanged;
        }

        if (newSource is INotifyCollectionChanged newObservable)
        {
            newObservable.CollectionChanged += ItemsSource_CollectionChanged;
        }

        if (columnsAreReady)
        {
            Render();
        }
    }

    private void ItemsSource_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        Render();
    }

    protected virtual void OnColumnsSet(IList<DataGridColumn> oldValue, IList<DataGridColumn> newValue)
    {
        if (oldValue == newValue || CurrentType == null)
        {
            return;
        }

        if (oldValue is INotifyCollectionChanged oldObservable)
        {
            oldObservable.CollectionChanged -= Columns_CollectionChanged;
        }

        if (newValue is INotifyCollectionChanged newObservable)
        {
            newObservable.CollectionChanged += Columns_CollectionChanged;
        }

        Render();
    }

    private void Columns_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                {
                    SlideRow(e.NewStartingIndex, e.NewItems.Count);

                    for (int i = 0; i < e.NewItems.Count; i++)
                    {
                        AddRow(e.NewStartingIndex, e.NewItems[i], e.NewStartingIndex + i == ItemsSource.Count);
                    }
                }
                break;
            case NotifyCollectionChangedAction.Remove:
                {
                    for (int i = 0; i < e.OldItems.Count; i++)
                    {
                        RemoveRow(i);
                    }

                    SlideRow(e.OldStartingIndex, -1 * (e.OldStartingIndex + e.OldItems.Count));
                }
                break;
            default:
                // TODO: Optimize
                Render();
                break;
        }


    }

    protected virtual void SetAutoColumns()
    {
        if (UseAutoColumns)
        {
            Columns = CurrentType?.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Select(s => new DataGridColumn
                {
                    Title = s.PropertyType.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName ?? s.Name,
                    PropertyName = s.Name,
                    PropertyInfo = s,
                }).ToList();
        }
    }

    protected virtual void Render()
    {
        if (Columns == null || Columns.Count == 0 || CurrentType == null)
        {
            return; // Not ready yet.
        }

        EnsurePropertyInfosAreSet();

        var tableHeaderRows = 1;
        ResetGrid();
        ConfigureGridDefinitions(ItemsSource.Count + tableHeaderRows, Columns.Count);

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
        if(this.Content != _rootGrid )
        {
            this.Content = _rootGrid;
        }
    }

    protected virtual void AddTableHeaders()
    {
        for (int i = 0; i < Columns.Count; i++)
        {
            var label = LabelFactory() ?? CreateLabel();
            label.FontAttributes = FontAttributes.Bold;
            // TODO: Use an attribute to localize it.
            label.BindingContext = new CellBindingContext
            {
                Value = Columns[i].Title
            };

            _rootGrid.Add(label, column: i, row: 0);
        }

        AddSeparator(1);
    }

    protected virtual void AddRow(int row, object item, bool isLastRow)
    {
        var actualRow = row * 2;

        for (int columnNumber = 0; columnNumber < Columns.Count; columnNumber++)
        {
            var view = (View)Columns[columnNumber].CellItemTemplate?.CreateContent()
                ?? (View)CellItemTemplate?.CreateContent()
                ?? LabelFactory() ?? CreateLabel();

            view.BindingContext = new CellBindingContext
            {
                Column = columnNumber,
                Row = row,
                Data = item,
                Value = Columns[columnNumber].PropertyInfo?.GetValue(item)
            };

            _rootGrid.Add(view, columnNumber, row: actualRow);
        }

        if (!isLastRow)
        {
            AddSeparator(actualRow + 1);
        }
    }

    protected virtual void RemoveRow(int row)
    {
        
    }
    protected virtual void AddSeparator(int row)
    {
        var line = HorizontalLineFactory() ?? CreateHorizontalLine();

        Grid.SetColumnSpan(line, Columns.Count);
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

    protected virtual void SlideRow(int row, int amount = 1)
    {
        var actualRow = row * 2;

        foreach (View item in _rootGrid.Children.Where(x => x is View view && Grid.GetRow(view) >= actualRow))
        {
            Grid.SetRow(item, row + amount);
        }
    }

    protected virtual void EnsurePropertyInfosAreSet()
    {
        foreach (var column in Columns.Where(x => x.PropertyInfo == null && x.PropertyName != null))
        {
            column.PropertyInfo = CurrentType.GetProperty(column.PropertyName);
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