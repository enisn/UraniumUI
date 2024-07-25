using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Input;
using UraniumUI.Extensions;
using UraniumUI.Resources;
using UraniumUI.Triggers;

namespace UraniumUI.Material.Controls;

[ContentProperty(nameof(Items))]
public partial class TabView : Grid
{
    public TabViewCachingStrategy CachingStrategy { get; set; }

    public bool UseAnimation { get; set; } = true;

    public event EventHandler<object> CurrentItemChanged;
    public event EventHandler<TabItem> SelectedTabChanged;

    public static DataTemplate DefaultTabHeaderItemTemplate => new DataTemplate(() =>
    {
        var grid = new Grid();
        grid.AddRowDefinition(new RowDefinition(GridLength.Auto));
        grid.AddRowDefinition(new RowDefinition(GridLength.Auto));
        grid.Opacity = .5;

        var tabButton = new Button
        {
            StyleClass = new[] { "TextButton" },
        };
        tabButton.CornerRadius = 0;
        tabButton.SetAppThemeColor(Button.TextColorProperty, ColorResource.GetColor("OnBackground"), ColorResource.GetColor("OnBackgroundDark"));
        tabButton.SetBinding(Button.TextProperty, new Binding(nameof(TabItem.Title)));
        tabButton.SetBinding(Button.CommandProperty, new Binding(nameof(TabItem.Command)));

        grid.Add(tabButton, 0, 0);
        grid.Triggers.Add(new DataTrigger(typeof(Grid))
        {
            Binding = new Binding(nameof(TabItem.IsSelected), BindingMode.OneWay),
            Value = true,
            EnterActions =
            {
                new GenericTriggerAction<Grid>((sender) =>
                {
                    sender.SetAppThemeColor(
                                Grid.BackgroundColorProperty,
                                ColorResource.GetColor("Primary").WithAlpha(.2f),
                                ColorResource.GetColor("PrimaryDark").WithAlpha(.2f)
                            );

                    var box = (sender.Children.FirstOrDefault(x => x is BoxView) as BoxView);

                    box.FadeToSafely(1, easing: Easing.SpringIn);
                    sender.FadeToSafely(1);

                    var button = sender.Children.FirstOrDefault(x=>x is Button) as Button;
                    button?.SetAppThemeColor(Button.TextColorProperty, ColorResource.GetColor("Primary"), ColorResource.GetColor("PrimaryDark"));
                })
            }
        });

        grid.Triggers.Add(new DataTrigger(typeof(Grid))
        {
            Binding = new Binding(nameof(TabItem.IsSelected), BindingMode.OneWay),
            Value = false,
            EnterActions =
            {
                new GenericTriggerAction<Grid>((sender) =>
                {
                    var box = (sender.Children.FirstOrDefault(x => x is BoxView) as BoxView);

                    sender.BackgroundColor = Colors.Transparent;

                    box.FadeToSafely(0, easing: Easing.SpringIn);
                    sender.FadeToSafely(.5);

                    var button = sender.Children.FirstOrDefault(x=>x is Button) as Button;
                    button?.SetAppThemeColor(Button.TextColorProperty, ColorResource.GetColor("OnBackground"), ColorResource.GetColor("OnBackgroundDark"));
                    // TODO: Find a way to set app theme color repeatedly.
                    //button.TextColor = ColorResource.GetColor("OnBackground", "OnBackgroundDark");
                })
            }
        });

        var selectionIndicator = new BoxView
        {
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.End,
            HeightRequest = 5,
            CornerRadius = 1,
            Opacity = 0,
        };

        selectionIndicator.SetAppThemeColor(BoxView.ColorProperty,
            ColorResource.GetColor("Primary").WithAlpha(.2f),
            ColorResource.GetColor("PrimaryDark").WithAlpha(.2f));

        grid.Add(selectionIndicator, row: 0);

        return grid;
    });

    protected readonly Grid _headerContainer = new Grid
    {
        StyleClass = new[] { "TabView.Header" },
    };

    protected readonly ContentView _contentContainer = new ContentView
    {
        StyleClass = new[] { "TabView.Content" },
    };

    protected readonly ScrollView _headerScrollView = new ScrollView
    {
        Orientation = ScrollOrientation.Horizontal,
    };

    public TabView()
    {
        Items = new ObservableCollection<TabItem>();

        _headerScrollView.Content = _headerContainer;
        this.Add(_headerScrollView);
        this.Add(_contentContainer);
        InitializeLayout();
        if (Items is INotifyCollectionChanged observable)
        {
            observable.CollectionChanged -= Items_CollectionChanged;
            observable.CollectionChanged += Items_CollectionChanged;
        }
        Render();
    }

    protected virtual void OnItemsSourceChanged(IList oldValue, IList newValue)
    {
        if (oldValue is INotifyCollectionChanged oldObservable)
        {
            oldObservable.CollectionChanged -= Items_CollectionChanged;
        }

        if (newValue is INotifyCollectionChanged newObservable)
        {
            newObservable.CollectionChanged += Items_CollectionChanged;
        }

        Render();
    }

    private void OnItemTemplateChanged()
    {
        Render();
    }

    protected virtual void InitializeLayout()
    {
        this.ColumnDefinitions.Clear();
        this.RowDefinitions.Clear();

        _headerScrollView.Orientation = AreTabsVertical ? ScrollOrientation.Vertical : ScrollOrientation.Horizontal;

        AlignTabPlacement();
        AlignHeaderGridItems();
    }

    protected virtual void AlignTabPlacement()
    {
        switch (TabPlacement)
        {
            case TabViewTabPlacement.Top:
                {
                    this.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
                    this.RowDefinitions.Add(new RowDefinition(GridLength.Star));

                    Grid.SetRow(_headerScrollView, 0);
                    Grid.SetColumn(_headerScrollView, 0);

                    Grid.SetRow(_contentContainer, 1);
                    Grid.SetColumn(_contentContainer, 0);
                }
                break;
            case TabViewTabPlacement.Bottom:
                {
                    this.RowDefinitions.Add(new RowDefinition(GridLength.Star));
                    this.RowDefinitions.Add(new RowDefinition(GridLength.Auto));


                    Grid.SetRow(_headerScrollView, 1);
                    Grid.SetColumn(_headerScrollView, 0);

                    Grid.SetRow(_contentContainer, 0);
                    Grid.SetColumn(_contentContainer, 0);
                }
                break;
            case TabViewTabPlacement.Start:
                {
                    this.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));
                    this.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));


                    Grid.SetRow(_headerScrollView, 0);
                    Grid.SetColumn(_headerScrollView, 0);

                    Grid.SetRow(_contentContainer, 0);
                    Grid.SetColumn(_contentContainer, 1);
                }
                break;
            case TabViewTabPlacement.End:
                {
                    this.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
                    this.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));

                    Grid.SetRow(_headerScrollView, 0);
                    Grid.SetColumn(_headerScrollView, 1);

                    Grid.SetRow(_contentContainer, 0);
                    Grid.SetColumn(_contentContainer, 0);
                }
                break;
        }
    }

    protected virtual void AlignHeaderGridItems()
    {
        if (AreTabsVertical)
        {
            _headerContainer.RowDefinitions.Clear();
            _headerContainer.ColumnDefinitions.Clear();

            for (int i = 0; i < _headerContainer.Children.Count; i++)
            {
                _headerContainer.RowDefinitions.Add(new RowDefinition(GridLength.Auto));

                Grid.SetRow(_headerContainer.Children[i] as View, i);
            }
        }
        else // Horizontal
        {
            _headerContainer.RowDefinitions.Clear();
            _headerContainer.ColumnDefinitions.Clear();

            for (int i = 0; i < _headerContainer.Children.Count; i++)
            {
                _headerContainer.ColumnDefinitions.Add(new ColumnDefinition(TabHeaderItemColumnWidth));

                Grid.SetColumn(_headerContainer.Children[i] as View, i);
            }
        }
    }

    public bool AreTabsVertical => TabPlacement == TabViewTabPlacement.Start || TabPlacement == TabViewTabPlacement.End;

    protected virtual void OnItemsChanged(IList<TabItem> oldValue, IList<TabItem> newValue) // TODO: Test it and prevent multiple initializations.
    {
        if (oldValue is INotifyCollectionChanged oldObservable)
        {
            oldObservable.CollectionChanged -= Items_CollectionChanged;

        }

        if (newValue is INotifyCollectionChanged observable)
        {
            observable.CollectionChanged += Items_CollectionChanged;
        }

        Render();
    }

    private void Items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                {
                    foreach (var item in e.NewItems)
                    {
                        if (item is TabItem tabItem)
                        {
                            AddHeaderFor(tabItem);
                        }
                        else
                        {
                            AddHeaderForItem(item);
                        }
                    }
                }
                break;
            case NotifyCollectionChangedAction.Remove:
                {
                    foreach (var item in e.OldItems)
                    {
                        if (item is TabItem tabItem)
                        {
                            RemoveHeaderFor(tabItem);
                        }
                        else
                        {
                            tabItem = Items.FirstOrDefault(x => x.Data == item);
                            if (tabItem != null)
                            {
                                Items.Remove(tabItem);
                            }
                        }
                    }
                }
                break;
            default:
                // TODO: Optimize
                Render();
                break;
        }
    }

    internal virtual void Render()
    {
        if (Items?.Count > 0 || ItemsSource?.Count > 0)
        {
            RenderHeaders();

            if (SelectedTab is null)
            {
                ResetSelectedTab();
            }
        }
    }

    internal virtual void RenderHeaders()
    {
        _headerContainer.Children.Clear();
        foreach (var item in Items)
        {
            AddHeaderFor(item);
        }

        if (ItemsSource is not null)
        {
            foreach (var item in ItemsSource)
            {
                AddHeaderForItem(item);
            }
        }
    }

    internal virtual void InvalidateTabItemContents()
    {
        foreach (var tabItem in this.Items)
        {
            tabItem.Content = null;
            tabItem.Header = null;
        }

        ResetSelectedTab();
    }

    protected void ResetSelectedTab()
    {
        if (SelectedTab is null)
        {
            SelectedTab = Items.FirstOrDefault();
        }
        else
        {
            // Send previous selected tab to null, to force re-rendering.
            // TODO: Create an API to force re-rendering for header, content or both.
            OnSelectedTabChanged(null, SelectedTab).FireAndForget();
        }
    }

    protected virtual void AddHeaderFor(TabItem tabItem)
    {
        tabItem.TabView = this;
        tabItem.Header =
            tabItem.HeaderTemplate?.CreateContent() as View
            ?? TabHeaderItemTemplate?.CreateContent() as View
            //?? DefaultTabHeaderItemTemplate.CreateContent() as View
            ?? throw new InvalidOperationException("TabView requires a HeaderTemplate or TabHeaderItemTemplate to be set.");

        tabItem.Header.BindingContext = tabItem;
        tabItem.Header.GestureRecognizers.Add(new TapGestureRecognizer { Command = new Command(() => SelectedTab = tabItem) });

        if (!_headerContainer.Children.Any())
        {
            SelectedTab = tabItem;
        }

        if (AreTabsVertical)
        {
            _headerContainer.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
            Grid.SetRow(tabItem.Header, _headerContainer.Children.Count);
        }
        else
        {
            _headerContainer.ColumnDefinitions.Add(new ColumnDefinition(TabHeaderItemColumnWidth));
            Grid.SetColumn(tabItem.Header, _headerContainer.Children.Count);
        }
        _headerContainer.Add(tabItem.Header);
    }

    protected virtual void AddHeaderForItem(object item)
    {
        var tabItem = new TabItem { Data = item, Title = item?.ToString() };

        Items.Add(tabItem);
    }

    protected virtual void RemoveHeaderFor(TabItem tabItem)
    {
        var existing = _headerContainer.Children.FirstOrDefault(x => x is View view && view.BindingContext == tabItem);

        if (tabItem == SelectedTab)
        {
            ResetSelectedTab();
        }

        if (AreTabsVertical)
        {
            _headerContainer.RowDefinitions.RemoveAt(0);
        }
        else
        {
            _headerContainer.ColumnDefinitions.RemoveAt(0);
        }

        _headerContainer.Children.Remove(existing);
    }

    protected virtual void OnCurrentItemChanged(object newItem)
    {
        if (newItem == null)
        {
            SelectedTab = null;
        }

        if (SelectedTab?.Data == newItem)
        {
            return;
        }

        CurrentItemChanged?.Invoke(this, newItem);
        ExecuteCommandIfCan(CurrentItemChangedCommand, newItem);

        SelectedTab = Items.FirstOrDefault(x => x.Data == newItem);
    }

    protected virtual async Task OnSelectedTabChanged(TabItem oldValue, TabItem newValue)
    {
        if (newValue == null)
        {
            _contentContainer.Content = null;
            CurrentItem = null;
            return;
        }

        if (oldValue == newValue)
        {
            return;
        }

        var content = newValue.Content ??=
            ((View)newValue.ContentTemplate?.CreateContent()
                ?? (View)ItemTemplate?.CreateContent());

        if (content.BindingContext is null && newValue.Data is not null && content.BindingContext != newValue.Data)
        {
            content.BindingContext = newValue.Data;
        }

        foreach (var item in Items)
        {
            item.NotifyIsSelectedChanged();
        }

        if (CachingStrategy == TabViewCachingStrategy.RecreateAlways && oldValue is not null)
        {
            oldValue.Content = null; // Make it null, in the next visit of this method, a new instance will be created.
        }

        if (CachingStrategy == TabViewCachingStrategy.CacheOnLayout)
        {
            if (_contentContainer.Content is not Layout layout)
            {
                layout = new Grid();
                _contentContainer.Content = layout;
            }

            var existing = layout.Children.FirstOrDefault(x => x == content);
            if (existing == null)
            {
                layout.Children.Add(content);
            }

            foreach (var child in layout.Children)
            {
                (child as View).IsVisible = content == child;
            }
        }
        else
        {
            if (_contentContainer.Content != null)
            {
                if (UseAnimation)
                {
                    await _contentContainer.Content?.FadeToSafely(0, 60);
                }
            }

            content.Opacity = 0;

            _contentContainer.Content = content;
            if (UseAnimation)
            {
                await content.FadeToSafely(1, 60);
            }
        }

        content.Opacity = 1;

        if (SelectedTab?.Data != null)
        {
            CurrentItem = SelectedTab.Data;
        }

        SelectedTabChanged?.Invoke(this, newValue);
        ExecuteCommandIfCan(SelectedTabChangedCommand, newValue);
    }

    protected virtual void OnTabPlacementChanged()
    {
        InitializeLayout();
    }

    protected virtual void ExecuteCommandIfCan(ICommand command, object parameter)
    {
        if (command?.CanExecute(parameter) == true)
        {
            command.Execute(parameter);
        }
    }
}
