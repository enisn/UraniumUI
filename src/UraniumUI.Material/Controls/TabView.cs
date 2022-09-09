using System.Collections.Specialized;
using UraniumUI.Material.Resources;
using UraniumUI.Triggers;

namespace UraniumUI.Material.Controls;

[ContentProperty(nameof(Items))]
public partial class TabView : Grid
{
    public static DataTemplate DefaultTabHeaderItemTemplate => new DataTemplate(() =>
    {
        var grid = new Grid();
        grid.AddRowDefinition(new RowDefinition(GridLength.Auto));
        grid.AddRowDefinition(new RowDefinition(GridLength.Auto));

        var tabButton = new Button
        {
            StyleClass = new[] { "TextButton" },
        };
        tabButton.CornerRadius = 0;
        tabButton.SetBinding(Button.TextProperty, new Binding(nameof(TabItem.Title)));
        tabButton.Clicked += (s, e) =>
        {
            if (s is View view && view.BindingContext is TabItem tabItem)
            {
                tabItem.InvokeOnSelected();
            }
        };

        grid.Add(tabButton, 0, 0);
        grid.Triggers.Add(new DataTrigger(typeof(Grid))
        {
            Binding = new Binding(nameof(TabItem.IsSelected), BindingMode.TwoWay),
            Value = true,
            EnterActions =
            {
                new GenericTriggerAction<Grid>((sender) =>
                {
                    var box = (sender.Children.FirstOrDefault(x => x is BoxView) as BoxView);

                    sender.BackgroundColor = ColorResource.GetColor("SurfaceTint0");

                    box.FadeTo(1, easing: Easing.SpringIn);
                })
            },
            ExitActions =
            {
                new GenericTriggerAction<Grid>((sender) =>
                {
                    var box = (sender.Children.FirstOrDefault(x => x is BoxView) as BoxView);

                    sender.BackgroundColor = Colors.Transparent;
                    
                    box.FadeTo(0, easing: Easing.SpringIn);
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
            ColorResource.GetColor("SurfaceTint2"),
            ColorResource.GetColor("PrimaryDark"));

        grid.Add(selectionIndicator, row: 0);

        return grid;
    });

    protected readonly StackLayout _headerContainer = new StackLayout
    {
        HorizontalOptions = LayoutOptions.Fill
    };

    protected readonly ContentView _contentContainer = new ContentView
    {
        HorizontalOptions = LayoutOptions.Fill,
        VerticalOptions = LayoutOptions.Fill
    };

    public TabView()
    {
        this.RowDefinitions.Add(new RowDefinition(GridLength.Star));
        this.RowDefinitions.Add(new RowDefinition(GridLength.Star));
        _headerContainer.Orientation = StackOrientation.Horizontal;

        this.Add(_headerContainer);
        this.Add(_contentContainer, row: 1);

        if (Items is INotifyCollectionChanged observable)
        {
            observable.CollectionChanged -= Items_CollectionChanged;
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
                    }
                }
                break;
            default:
                // TODO: Optimize
                Render();
                break;
        }
    }

    protected virtual void Render()
    {
        if (Items?.Count > 0)
        {
            RenderHeaders();

            if (CurrentItem is null)
            {
                ResetSelectedItem();
            }
        }
    }

    protected virtual void RenderHeaders()
    {
        foreach (var item in Items)
        {
            AddHeaderFor(item);
        }
    }

    protected void ResetSelectedItem()
    {
        CurrentItem = Items.FirstOrDefault();
    }

    protected virtual void AddHeaderFor(TabItem tabItem)
    {
        var view = TabHeaderItemTemplate?.CreateContent() as View;
        view.BindingContext = tabItem;
        tabItem.OnSelected += (s, e) =>
        {
            OnCurrentItemChanged(s as TabItem);
        };
        _headerContainer.Add(view);
    }

    protected virtual void RemoveHeaderFor(TabItem tabItem)
    {
        var existing = _headerContainer.Children.FirstOrDefault(x => x is View view && view.BindingContext == tabItem);

        if (CurrentItem == tabItem)
        {
            ResetSelectedItem();
        }

        _headerContainer.Children.Remove(existing);
    }

    protected virtual void OnCurrentItemChanged(TabItem newCurrentTabItem)
    {
        var content = newCurrentTabItem.Content ??= (View)newCurrentTabItem.ContentTemplate?.CreateContent();
        content.Opacity = 0;
        _contentContainer.Content = content;

        foreach (var tab in Items)
        {
            tab.IsSelected = tab == newCurrentTabItem;
        }

        content.FadeTo(1);
    }
}

public class TabItem : UraniumBindableObject
{
    private bool isSelected;

    public event EventHandler OnSelected;

    public string Title { get; set; }
    public ImageSource Icon { get; set; }
    public DataTemplate ContentTemplate { get; set; }
    public bool IsSelected { get => isSelected; internal set => SetProperty(ref isSelected, value); }
    public View Content { get; set; }

    internal void InvokeOnSelected() => OnSelected?.Invoke(this, new EventArgs());
}
