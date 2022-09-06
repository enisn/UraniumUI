using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Reflection;

namespace UraniumUI.Material.Controls;

public partial class DataGrid : Frame
{
    private Grid _rootGrid;

    public Type CurrentType { get; protected set; }

    public DataGrid()
    {
        this.Content = _rootGrid = new Grid
        {
            HorizontalOptions = LayoutOptions.Fill
        };
        InitializeFactoryMethods();
        this.Padding = new Thickness(0, 10);
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
        //switch (e.Action)
        //{
        //    case NotifyCollectionChangedAction.Add:
        //        {
        //            SlideRow(e.NewStartingIndex, e.NewItems.Count);

        //            for (int i = 0; i < e.NewItems.Count; i++)
        //            {
        //                AddRow(e.NewStartingIndex, e.NewItems[i], e.NewStartingIndex + i == ItemsSource.Count);
        //            }

        //            ConfigureGridRowDefinitions(ItemsSource.Count + 1);
        //        }
        //        break;
        //    case NotifyCollectionChangedAction.Remove:
        //        {

        //            for (int i = 0; i < e.OldItems.Count; i++)
        //            {
        //                RemoveRow(i);
        //            }

        //           /SlideRow(e.OldStartingIndex + e.OldItems.Count, (-1 * e.OldItems.Count));

        //            ConfigureGridRowDefinitions(ItemsSource.Count + 1);
        //        }
        //        break;
        //    default:
        //        // TODO: Optimize
        //        Render();
        //        break;
        //}
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
        Render();
    }

    protected virtual void SetAutoColumns()
    {
        if (UseAutoColumns)
        {
            Columns = CurrentType?.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Select(s => new DataGridColumn
                {
                    Title = s.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName ?? s.Name,
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
        ConfigureGridColumnDefinitions(Columns.Count);
        ConfigureGridRowDefinitions(ItemsSource.Count + tableHeaderRows);

        AddTableHeaders();

        for (int i = 0; i < ItemsSource.Count; i++)
        {
            var row = i + tableHeaderRows; // +1 is table header.

            AddRow(row, ItemsSource[i], i == ItemsSource.Count - 1);
        }

        RegisterSelectionChanges();
    }

    private void ResetGrid()
    {
        _rootGrid.Clear();
        _rootGrid.Children.Clear();

        if (this.Content != _rootGrid)
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
            var created = (View)Columns[columnNumber].CellItemTemplate?.CreateContent()
                ?? (View)CellItemTemplate?.CreateContent()
                ?? LabelFactory() ?? CreateLabel();

            var view = new ContentView
            {
                Content = created
            };

            view.BindingContext = new CellBindingContext
            {
                Column = columnNumber,
                Row = row,
                Data = item,
                Value = Columns[columnNumber].PropertyInfo?.GetValue(item)
            };

            SetSelectionVisualStates(view);

            _rootGrid.Add(view, columnNumber, row: actualRow);
        }

        if (!isLastRow)
        {
            AddSeparator(actualRow + 1);
        }
    }

    protected virtual void RemoveRow(int row)
    {
        var actualRow = row * 2;

        for (int i = 0; i < _rootGrid.Children.Count; i++)
        {
            if (Grid.GetRow(_rootGrid.Children[i] as View) == actualRow)
            {
                _rootGrid.Children.RemoveAt(i);
                i--;
            }
        }
        _rootGrid.RowDefinitions.RemoveAt(0);

        if (_rootGrid.LastOrDefault() is BoxView box)
        {
            _rootGrid.Remove(box);
            _rootGrid.RowDefinitions.RemoveAt(0);
        }

    }

    protected virtual void AddSeparator(int row)
    {
        var line = HorizontalLineFactory() ?? CreateHorizontalLine();

        Grid.SetColumnSpan(line, Columns.Count);
        _rootGrid.Add(line, 0, row);
    }

    private void ConfigureGridColumnDefinitions(int columns)
    {
        _rootGrid.ColumnDefinitions.Clear();
        for (int i = 0; i < columns; i++)
        {
            _rootGrid.AddColumnDefinition(new ColumnDefinition(GridLength.Auto));
        }
    }

    private void ConfigureGridRowDefinitions(int rows)
    {
        _rootGrid.RowDefinitions.Clear();
        for (int i = 0; i < (rows + (rows - 1)); i++)
        {
            _rootGrid.AddRowDefinition(new RowDefinition(GridLength.Star));
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

    protected virtual void OnSelectedItemsSet()
    {
        UpdateSelections();

        if (SelectedItems is INotifyCollectionChanged observable)
        {
            observable.CollectionChanged += (s, e) => UpdateSelections();
        }
    }

    protected void UpdateSelections()
    {
        foreach (View child in _rootGrid.Children)
        {
            if (child.BindingContext is CellBindingContext cellBindingContext)
            {
                SetSelected(child, SelectedItems.Contains(cellBindingContext.Data));
            }
        }
    }

    private void RegisterSelectionChanges()
    {
        foreach (IDataGridSelectionColumn selection in Columns.Where(x => x is IDataGridSelectionColumn))
        {
            selection.SelectionChanged += SelectionChanged;
        }
    }

    private void SelectionChanged(object sender, bool isSelected)
    {
        if (sender is View view && view.BindingContext is CellBindingContext cellContext)
        {
            var actualRow = cellContext.Row * 2;

            foreach (View child in _rootGrid.Children.Where(x => Grid.GetRow(x as View) == actualRow))
            {
                SetSelected(child, isSelected);
            }

            if (isSelected)
            {
                SelectedItems?.Add(cellContext.Data);
            }
            else
            {
                SelectedItems?.Remove(cellContext.Data);
            }

            OnPropertyChanged(nameof(SelectedItems));
        }
    }

    protected virtual void SetSelected(View view, bool isSelected)
    {
        VisualStateManager.GoToState(view, isSelected ? "Selected" : "Unselected");
    }

    protected void SetSelectionVisualStatesForAll()
    {
        if (_rootGrid is null)
        {
            return;
        }

        foreach (View child in _rootGrid.Children)
        {
            SetSelectionVisualStates(child);
        }
    }

    protected virtual void SetSelectionVisualStates(View view)
    {
        VisualStateManager.SetVisualStateGroups(view, new VisualStateGroupList
        {
            new VisualStateGroup
            {
                Name = "DataGridSelectionStates",
                States =
                {
                    new VisualState
                    {
                        Name = "Selected",
                        Setters =
                        {
                            new Setter
                            {
                                Property = View.BackgroundColorProperty,
                                Value = SelectionColor.MultiplyAlpha(0.2f)
                            }
                        }
                    },
                    new VisualState
                    {
                        Name = "Unselected",
                        Setters =
                        {
                            new Setter
                            {
                                Property = View.BackgroundColorProperty,
                                Value = Colors.Transparent
                            }
                        }
                    }
                }
            }
        });
    }
}