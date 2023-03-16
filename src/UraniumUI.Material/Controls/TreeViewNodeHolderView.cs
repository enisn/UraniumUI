using UraniumUI.Extensions;
using UraniumUI.Pages;
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

    public VerticalStackLayout NodeChildren => nodeChildren;

    internal protected VerticalStackLayout nodeChildren = new VerticalStackLayout
    {
        Margin = new Thickness(10, 0, 0, 0),
        IsVisible = false
    };

    protected Grid rowStack = new Grid
    {
        ColumnDefinitions =
        {
            new ColumnDefinition(GridLength.Auto),
            new ColumnDefinition(GridLength.Star),
        }
    };

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

    private View expanderView;

    public TreeViewNodeHolderView(DataTemplate dataTemplate, TreeView treeView, BindingBase childrenBinding)
    {
        if (treeView is null)
        {
            throw new ArgumentNullException(nameof(treeView));
        }

        TreeView = treeView;
        DataTemplate = dataTemplate;
        nodeContainer.Content = DataTemplate.CreateContent() as View;
        expanderView = TreeView.ExpanderTemplate?.CreateContent() as View ?? InitializeArrowExpander();
        expanderView.BindingContext = this;

        this.SetBinding(SpacingProperty, new Binding(nameof(TreeView.Spacing), source: treeView));
        nodeChildren.SetBinding(VerticalStackLayout.SpacingProperty, new Binding(nameof(TreeView.Spacing), source: treeView));

        rowStack.Add(expanderView);
        rowStack.Add(nodeContainer, column: 1);
        this.Add(rowStack);
        this.Add(nodeChildren);

        if (!string.IsNullOrEmpty(TreeView.IsExpandedPropertyName))
        {
            this.SetBinding(IsExpandedProperty, new Binding(TreeView.IsExpandedPropertyName, BindingMode.TwoWay));
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
    }

    protected virtual View InitializeArrowExpander()
    {
        var iconArrow = new ButtonView
        {
            VerticalOptions = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.Start,
            StyleClass = new[] { "TreeViewExpandButton" },
            Padding = 0,
            Margin = 0,
            PressedCommand = new Command(() => IsExpanded = !IsExpanded),
        };

        iconArrow.Content = new ContentView
        {
            Margin = new Thickness(0, 0, 5, 0),
            Content = new Path
            {
                Data = UraniumShapes.ArrowRight,
                Fill = TreeView.ArrowColor,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
            }
        };

        iconArrow.Triggers.Add(new DataTrigger(typeof(View))
        {
            Binding = new Binding(nameof(IsLeaf)),
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

        iconArrow.Triggers.Add(new DataTrigger(typeof(View))
        {
            Binding = new Binding(IsExpandedProperty.PropertyName),
            Value = true,
            EnterActions =
            {
                new GenericTriggerAction<View>((view) =>
                {
                    if (TreeView.UseAnimation)
                    {
                        view.RotateTo(90, 90, easing: Easing.BounceOut);
                    }
                    else
                    {
                        iconArrow.Rotation = 90;
                    }
                })
            },
            ExitActions =
            {
                new GenericTriggerAction<ButtonView>((view) =>
                {
                    if (TreeView.UseAnimation)
                    {
                        view.RotateTo(0, 90, easing: Easing.BounceOut);
                    }
                    else
                    {
                        iconArrow.Rotation = 0;
                    }
                })
            }
        });

        return iconArrow;
    }

    public virtual void ReFillArrowColor()
    {
        Layout expanderLayout = null;

        if (expanderView is ContentView contentView && contentView.Content is Layout)
        {
            expanderLayout = contentView.Content as Layout;
        }
        else if (expanderView is Layout layout)
        {
            expanderLayout = layout;
        }

        if (expanderLayout is not null)
        {
            foreach (var path in expanderLayout.FindManyInChildrenHierarchy<Path>())
            {
                path.Fill = TreeView.ArrowColor;
            }
        }

        foreach (var childHolder in nodeChildren.Children.OfType<TreeViewNodeHolderView>())
        {
            childHolder.ReFillArrowColor();
        }
    }

    public virtual void ApplyIsExpandedPropertyBindings()
    {
        this.SetBinding(IsExpandedProperty, new Binding(TreeView.IsExpandedPropertyName, BindingMode.TwoWay));
        foreach (TreeViewNodeHolderView item in Children.Where(x => x is TreeViewNodeHolderView))
        {
            item.ApplyIsExpandedPropertyBindings();
        }
    }

    public virtual void ApplyIsLeafPropertyBindings()
    {
        this.SetBinding(IsLeafProperty, new Binding(this.TreeView.IsLeafPropertyName, BindingMode.TwoWay));
        foreach (TreeViewNodeHolderView item in Children.Where(x => x is TreeViewNodeHolderView))
        {
            item.ApplyIsLeafPropertyBindings();
        }
    }

    protected internal virtual async void OnIsExpandedChanged(bool isExpanded)
    {
        if (isExpanded)
        {
            if (TreeView.UseAnimation)
            {
                nodeChildren.IsVisible = true;
                nodeChildren.TranslateTo(0, 0, 50);
                nodeChildren.ScaleTo(1, 50);
                nodeChildren.FadeTo(1);
            }
            else
            {
                nodeChildren.IsVisible = true;
            }
        }
        else
        {
            if (TreeView.UseAnimation)
            {
                nodeChildren.TranslateTo(0, -nodeChildren.Height);
                nodeChildren.ScaleTo(0);
                nodeChildren.AnchorX = 0;
                nodeChildren.AnchorY = 0;

                await nodeChildren.FadeTo(0, 50);
            }

            nodeChildren.IsVisible = false;
        }

        LoadChildrenIfNecessary();
    }

    protected virtual void OnIsLeafChanged(bool? newValue)
    {
        if (newValue == true && IsExpanded)
        {
            IsExpanded = false;
        }
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
        nameof(IsLeaf), typeof(bool?), typeof(TreeViewNodeHolderView), null, BindingMode.TwoWay,
        propertyChanged: (bindable, oldValue, newValue) => (bindable as TreeViewNodeHolderView).OnIsLeafChanged((bool?)newValue));

    public bool IsExpanded { get => (bool)GetValue(IsExpandedProperty); set => SetValue(IsExpandedProperty, value); }

    public static readonly BindableProperty IsExpandedProperty =
        BindableProperty.Create(nameof(IsExpanded), typeof(bool), typeof(TreeViewNodeHolderView), false,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                if (bindable is TreeViewNodeHolderView holderView)
                {
                    holderView.OnPropertyChanged(nameof(IsExpanded));
                    holderView.OnIsExpandedChanged((bool)newValue);
                }
            });
}