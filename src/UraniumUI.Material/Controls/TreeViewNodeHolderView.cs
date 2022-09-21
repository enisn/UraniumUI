using InputKit.Shared.Controls;
using UraniumUI.Pages;
using UraniumUI.Resources;
using UraniumUI.Triggers;
using Path = Microsoft.Maui.Controls.Shapes.Path;

namespace UraniumUI.Material.Controls;
public class TreeViewNodeHolderView : VerticalStackLayout
{
    public View NodeView { get => nodeContainer.Content; set => nodeContainer.Content = value; }

    public TreeViewNodeHolderView Parent { get; private set; }

    protected ContentView nodeContainer = new ContentView
    {
        HorizontalOptions = LayoutOptions.Fill,
    };

    protected ContentView iconArrow = new ContentView
    {
        VerticalOptions = LayoutOptions.Center,
        HorizontalOptions = LayoutOptions.Start,
        Padding = 15,
        Content = new Path
        {
            Data = UraniumShapes.ArrowLeft,
            Fill = ColorResource.GetColor("OnBackground", "OnBackgroundDark", Colors.DarkGray),
        }
    };

    protected VerticalStackLayout stackLayoutChildren = new VerticalStackLayout
    {
        Margin = new Thickness(10, 0, 0, 0),
        IsVisible = false
    };

    public DataTemplate DataTemplate { get; }

    public bool IsLeaf => !stackLayoutChildren.Children.Any();

    private string isTextendedProperty;

    public string IsTextendedProperty
    {
        get => isTextendedProperty;
        set
        {
            isTextendedProperty = value;
            this.SetBinding(TreeView.IsExpandedProperty, new Binding(value, BindingMode.TwoWay));
            foreach (TreeViewNodeHolderView item in this.Children.Where(x => x is TreeViewNodeHolderView))
            {
                item.IsTextendedProperty = value;
            }
        }
    }

    public string isSelectedProperty;

    public string IsSelectedProperty
    {
        get => isTextendedProperty;
        set
        {
            isSelectedProperty = value;
            this.SetBinding(TreeView.IsSelectedProperty, new Binding(value, BindingMode.TwoWay));
            foreach (TreeViewNodeHolderView item in this.Children.Where(x => x is TreeViewNodeHolderView))
            {
                item.IsTextendedProperty = value;
            }
        }
    }

    public TreeViewNodeHolderView(DataTemplate dataTemplate, string isTextendedProperty)
    {
        DataTemplate = dataTemplate;
        nodeContainer.Content = DataTemplate.CreateContent() as View;

        this.Add(new HorizontalStackLayout
        {
            Children =
            {
                iconArrow,
                nodeContainer
            }
        });
        this.Add(stackLayoutChildren);

        this.SetBinding(TreeView.IsExpandedProperty, new Binding(isTextendedProperty, BindingMode.TwoWay));
        this.SetBinding(TreeView.IsSelectedProperty, new Binding("IsSelected", BindingMode.TwoWay));

        BindableLayout.SetItemTemplate(stackLayoutChildren, new DataTemplate(() =>
        {
            var node = new TreeViewNodeHolderView(DataTemplate, isTextendedProperty);

            node.Parent = this;

            return node;
        }));

        stackLayoutChildren.SetBinding(BindableLayout.ItemsSourceProperty, new Binding("Children"));

        stackLayoutChildren.ChildAdded += (s, e) => OnPropertyChanged(nameof(IsLeaf));
        stackLayoutChildren.ChildRemoved += (s, e) => OnPropertyChanged(nameof(IsLeaf));

        InitializeArrowButton();

        NodeView.Triggers.Add(new DataTrigger(typeof(View))
        {
            Binding = new Binding("IsSelected", source: this),
            Value = true,
            EnterActions =
            {
                new GenericTriggerAction<View>((_) =>
                {
                    OnIsSelectedChanged();
                })
            },
            ExitActions =
            {
                new GenericTriggerAction<View>((_) =>
                {
                    OnIsSelectedChanged();
                })
            }
        });
    }

    private void InitializeArrowButton()
    {
        var tapGestureRecognizer = new TapGestureRecognizer();
        iconArrow.GestureRecognizers.Add(tapGestureRecognizer);
        tapGestureRecognizer.Tapped += (s, e) =>
        {
            TreeView.SetIsExpanded(this, !TreeView.GetIsExpanded(this));
        };

        iconArrow.Triggers.Add(new DataTrigger(typeof(ContentView))
        {
            Binding = new Binding(nameof(IsLeaf), source: this),
            Value = true,
            EnterActions =
            {
                new GenericTriggerAction<ContentView>((view) =>
                {
                    view.Opacity = 0;
                    view.InputTransparent = true;
                })
            },
            ExitActions =
            {
                new GenericTriggerAction<ContentView>((view) =>
                {
                    view.Opacity = 1;
                    view.InputTransparent = false;
                })
            }
        });
    }

    protected internal virtual void OnIsExpandedChanged()
    {

        if (TreeView.GetIsExpanded(this))
        {
            iconArrow.RotateTo(90);
            stackLayoutChildren.IsVisible = true;
        }
        else
        {
            iconArrow.RotateTo(0);
            stackLayoutChildren.IsVisible = false;
        }
    }

    protected internal virtual void OnIsSelectedChanged()
    {
        var isSelected = TreeView.GetIsSelected(NodeView);

        VisualStateManager.GoToState(NodeView, isSelected == null ? "SemiSelected" : isSelected == true ? "Selected" : "UnSelected");

        if (IsLeaf)
        {
            Parent?.OnChildSelectionChanged();
            return;
        }
    }

    protected void OnChildSelectionChanged()
    {
        var query = stackLayoutChildren.Children.OfType<TreeViewNodeHolderView>();

        var first = query.FirstOrDefault(x => TreeView.GetIsSelected(x) != null);

        if (first == null)
        {
            TreeView.SetIsSelected(NodeView, null);
        }

        var firstIsSelected = TreeView.GetIsSelected(first);

        foreach (var item in query)
        {
            if (firstIsSelected != TreeView.GetIsSelected(item))
            {
                TreeView.SetIsSelected(NodeView, null);
                break;
            }
        }

        TreeView.SetIsSelected(NodeView, firstIsSelected);

        Parent?.OnChildSelectionChanged();
    }
}
