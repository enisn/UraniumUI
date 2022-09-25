using InputKit.Shared.Controls;
using System.Windows.Input;
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
    };

    public VerticalStackLayout NodeChildren => nodeChildren;

    internal protected VerticalStackLayout nodeChildren = new VerticalStackLayout
    {
        Margin = new Thickness(10, 0, 0, 0),
        IsVisible = false
    };

    protected HorizontalStackLayout rowStack;

    public DataTemplate DataTemplate { get; }

    private BindingBase childrenBinding;
    public BindingBase ChildrenBinding
    {
        get => childrenBinding;
        internal set
        {
            childrenBinding = value;
            nodeChildren.SetBinding(BindableLayout.ItemsSourceProperty, new Binding((ChildrenBinding as Binding)?.Path));
        }
    }

    public TreeViewNodeHolderView(DataTemplate dataTemplate, TreeView treeView, BindingBase childrenBinding)
    {
        if (treeView is null)
        {
            throw new ArgumentNullException(nameof(treeView));
        }

        TreeView = treeView;
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
        this.Add(nodeChildren);

        if (!string.IsNullOrEmpty(TreeView.IsExpandedPropertyName))
        {
            this.SetBinding(TreeView.IsExpandedProperty, new Binding(TreeView.IsExpandedPropertyName, BindingMode.TwoWay));
        }

        if (!string.IsNullOrEmpty(TreeView.IsLeafPropertyName))
        {
            this.SetBinding(IsLeafProperty, new Binding(TreeView.IsLeafPropertyName, BindingMode.TwoWay));
        }

        BindableLayout.SetItemTemplate(nodeChildren, new DataTemplate(() =>
        {
            var node = new TreeViewNodeHolderView(DataTemplate, TreeView, childrenBinding);
            node.ParentHolderView = this;
            node.TreeView = TreeView;
            return node;
        }));

        ChildrenBinding = childrenBinding;

        nodeChildren.ChildAdded += (s, e) => OnPropertyChanged(nameof(IsLeaf));
        nodeChildren.ChildRemoved += (s, e) => OnPropertyChanged(nameof(IsLeaf));

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

        iconArrow.Content = new Path
        {
            Data = UraniumShapes.ArrowLeft,
            Fill = TreeView.ArrowColor,
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

    public virtual void ReFillArrowColor()
    {
        if (iconArrow.Content is Path iconPath)
        {
            iconPath.Fill = TreeView.ArrowColor;
        }

        foreach (var childHolder in nodeChildren.Children.OfType<TreeViewNodeHolderView>())
        {
            childHolder.ReFillArrowColor();
        }
    }

    public virtual void ApplyIsExpandedPropertyBindings()
    {
        this.SetBinding(TreeView.IsExpandedProperty, new Binding(this.TreeView.IsExpandedPropertyName, BindingMode.TwoWay));
        foreach (TreeViewNodeHolderView item in this.Children.Where(x => x is TreeViewNodeHolderView))
        {
            item.ApplyIsExpandedPropertyBindings();
        }
    }

    public virtual void ApplyIsLeafPropertyBindings()
    {
        this.SetBinding(IsLeafProperty, new Binding(this.TreeView.IsLeafPropertyName, BindingMode.TwoWay));
        foreach (TreeViewNodeHolderView item in this.Children.Where(x => x is TreeViewNodeHolderView))
        {
            item.ApplyIsLeafPropertyBindings();
        }
    }

    protected internal virtual async void OnIsExpandedChanged()
    {
        if (TreeView.GetIsExpanded(this))
        {
            if (TreeView.UseAnimation)
            {
                iconArrow.RotateTo(90, 90, easing: Easing.BounceOut);

                nodeChildren.IsVisible = true;
                nodeChildren.TranslateTo(0, 0, 50);
                nodeChildren.ScaleTo(1, 50);
                nodeChildren.FadeTo(1);
            }
            else
            {
                iconArrow.Rotation = 90;
                nodeChildren.IsVisible = true;
            }
        }
        else
        {
            if (TreeView.UseAnimation)
            {
                iconArrow.RotateTo(0, 90, easing: Easing.BounceOut);
                nodeChildren.TranslateTo(0, -nodeChildren.Height);
                nodeChildren.ScaleTo(0);
                nodeChildren.AnchorX = 0;
                nodeChildren.AnchorY = 0;

                await nodeChildren.FadeTo(0, 50);
            }
            else
            {
                iconArrow.Rotation = 0;
            }

            nodeChildren.IsVisible = false;
        }

        LoadChildrenIfNecessary();
    }

    protected virtual void LoadChildrenIfNecessary()
    {
        if (!IsLeaf && !NodeChildren.Children.Any()) // TODO: And children is empty
        {
            TreeView.LoadChildrenCommand?.Execute(this.BindingContext);
        }
    }

    public bool IsLeaf
    {
        get => (bool?)GetValue(IsLeafProperty) ?? !nodeChildren.Children.Any();
        set => SetValue(IsLeafProperty, value);
    }

    public static readonly BindableProperty IsLeafProperty = BindableProperty.Create(
        nameof(IsLeaf), typeof(bool?), typeof(TreeViewNodeHolderView), null, BindingMode.TwoWay);
}