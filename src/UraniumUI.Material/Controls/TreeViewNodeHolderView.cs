using InputKit.Shared.Controls;
using UraniumUI.Pages;
using UraniumUI.Resources;
using UraniumUI.Triggers;
using Path = Microsoft.Maui.Controls.Shapes.Path;

namespace UraniumUI.Material.Controls;
public class TreeViewNodeHolderView : VerticalStackLayout
{
    public View NodeView { get => nodeContainer.Content; set => nodeContainer.Content = value; }

    public TreeViewNodeHolderView ParentHolderView { get; private set; }

    public TreeView TreeView { get; internal set; }

    protected ContentView nodeContainer = new ContentView
    {
        HorizontalOptions = LayoutOptions.Fill,
    };

    protected ContentView iconArrow = new ContentView
    {
        VerticalOptions = LayoutOptions.Center,
        HorizontalOptions = LayoutOptions.Start,
        Padding = new Thickness(15, 10),
        Content = new Path
        {
            Data = UraniumShapes.ArrowLeft,
            Fill = ColorResource.GetColor("OnBackground", "OnBackgroundDark", Colors.DarkGray),
        }
    };

    public VerticalStackLayout NodeChildrens => nodeChildrens;

    internal protected VerticalStackLayout nodeChildrens = new VerticalStackLayout
    {
        Margin = new Thickness(10, 0, 0, 0),
        IsVisible = false
    };

    protected HorizontalStackLayout rowStack;

    public DataTemplate DataTemplate { get; }
    public BindingBase ChildrenBinding
    {
        get => childrenBinding;
        internal set
        {
            childrenBinding = value;
            nodeChildrens.SetBinding(BindableLayout.ItemsSourceProperty, new Binding((ChildrenBinding as Binding)?.Path));
        }
    }
    public bool IsLeaf => !nodeChildrens.Children.Any();

    private string isTextendedProperty;
    private BindingBase childrenBinding;

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

    public TreeViewNodeHolderView(DataTemplate dataTemplate, string isTextendedProperty, BindingBase childrenBinding)
    {
        DataTemplate = dataTemplate;
        nodeContainer.Content = DataTemplate.CreateContent() as View;
        this.Add(rowStack = new HorizontalStackLayout
        {
            Children =
            {
                iconArrow,
                nodeContainer
            }
        });
        this.Add(nodeChildrens);

        if (!string.IsNullOrEmpty(isTextendedProperty))
        {
            this.SetBinding(TreeView.IsExpandedProperty, new Binding(isTextendedProperty, BindingMode.TwoWay));
        }

        BindableLayout.SetItemTemplate(nodeChildrens, new DataTemplate(() =>
        {
            var node = new TreeViewNodeHolderView(DataTemplate, isTextendedProperty, childrenBinding);

            node.ParentHolderView = this;
            node.TreeView = TreeView;
            return node;
        }));

        ChildrenBinding = childrenBinding;

        nodeChildrens.ChildAdded += (s, e) => OnPropertyChanged(nameof(IsLeaf));
        nodeChildrens.ChildRemoved += (s, e) => OnPropertyChanged(nameof(IsLeaf));

        InitializeArrowButton();
    }

    private void InitializeArrowButton()
    {
        var tapGestureRecognizer = new TapGestureRecognizer();
        iconArrow.GestureRecognizers.Add(tapGestureRecognizer);
        rowStack.GestureRecognizers.Add(tapGestureRecognizer);
        tapGestureRecognizer.Tapped += (s, e) =>
        {
            TreeView.SetIsExpanded(this, !TreeView.GetIsExpanded(this));
        };

        iconArrow.Triggers.Add(new DataTrigger(typeof(View))
        {
            Binding = new Binding(nameof(IsLeaf), source: this),
            Value = true,
            EnterActions =
            {
                new GenericTriggerAction<View>((view) =>
                {
                    view.Opacity = 0;
                    view.InputTransparent = true;
                })
            },
            ExitActions =
            {
                new GenericTriggerAction<View>((view) =>
                {
                    view.Opacity = 1;
                    view.InputTransparent = false;
                })
            }
        });
    }

    protected internal virtual async void OnIsExpandedChanged()
    {

        if (TreeView.GetIsExpanded(this))
        {
            iconArrow.RotateTo(90, 90, easing: Easing.BounceOut);

            nodeChildrens.IsVisible = true;
            nodeChildrens.TranslateTo(0, 0, 50);
            nodeChildrens.ScaleTo(1, 50);
            nodeChildrens.FadeTo(1);
        }
        else
        {
            iconArrow.RotateTo(0, 90, easing: Easing.BounceOut);
            nodeChildrens.TranslateTo(0, -nodeChildrens.Height);
            nodeChildrens.ScaleTo(0);
            nodeChildrens.AnchorX = 0;
            nodeChildrens.AnchorY = 0;

            await nodeChildrens.FadeTo(0, 50);
            nodeChildrens.IsVisible = false;
        }
    }
}
