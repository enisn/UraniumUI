using InputKit.Shared.Helpers;
using UraniumUI.Extensions;
using UraniumUI.Pages;
using UraniumUI.Triggers;
using UraniumUI.Views;
using static Microsoft.Maui.Controls.VisualStateManager;
using Path = Microsoft.Maui.Controls.Shapes.Path;

namespace UraniumUI.Material.Controls;

[ContentProperty(nameof(NodeView))]
public class TreeViewNodeHolderView : VerticalStackLayout
{
    public View NodeView { get => nodeContainer.Content; set => nodeContainer.Content = value; }

    public TreeViewNodeHolderView ParentHolderView { get; private set; }

    public TreeView TreeView { get; internal set; }

    protected TreeViewNodeItemContentView nodeContainer = new TreeViewNodeItemContentView
    {
        HorizontalOptions = LayoutOptions.Fill,
    };

    public VerticalStackLayout NodeChildren => nodeChildren;

    internal protected VerticalStackLayout nodeChildren = new VerticalStackLayout
    {
        IsVisible = false
    };

    public DataTemplate DataTemplate { get; }

    protected Grid rowStack = new Grid
    {
        ColumnDefinitions =
        {
            new ColumnDefinition(GridLength.Auto),
            new ColumnDefinition(GridLength.Star),
        }
    };

    private BindingBase childrenBinding;
    public BindingBase ChildrenBinding
    {
        get => childrenBinding;
        internal set
        {
            childrenBinding = value;
            if (ChildrenBinding is not null)
            {
                nodeChildren.SetBinding(BindableLayout.ItemsSourceProperty, new Binding((ChildrenBinding as Binding)?.Path));
            }
            else
            {
                nodeChildren.RemoveBinding(BindableLayout.ItemsSourceProperty);
            }
        }
    }

    private protected View expanderView;
    private protected ButtonView iconArrow;

    public TreeViewNodeHolderView(DataTemplate dataTemplate, TreeView treeView, BindingBase childrenBinding, int indentLevel = 0)
    {
        if (treeView is null)
        {
            throw new ArgumentNullException(nameof(treeView));
        }

        TreeView = treeView;
        DataTemplate = dataTemplate;

        nodeContainer.ItemTemplate = DataTemplate;
        nodeContainer.SetBinding(TreeViewNodeItemContentView.ItemProperty, ".");

        expanderView = TreeView.ExpanderTemplate?.CreateContent() as View ?? InitializeArrowExpander();
        expanderView.BindingContext = this;

        this.SetBinding(SpacingProperty, new Binding(nameof(TreeView.Spacing), source: treeView));
        nodeChildren.SetBinding(VerticalStackLayout.SpacingProperty, new Binding(nameof(TreeView.Spacing), source: treeView));

        rowStack.Add(expanderView);
        rowStack.Add(nodeContainer, column: 1);

        var button = new StatefulContentView
        {
            Content = rowStack,
            TappedCommand = new Command(ItemClicked),
            Padding = new Thickness(indentLevel * 16, 0, 0, 0)
        };

        this.Add(button);
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
            var node = new TreeViewNodeHolderView(DataTemplate, TreeView, childrenBinding, indentLevel + 1);
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
        iconArrow = new ButtonView
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
                StyleClass = new[] { "TreeView.Arrow" },
                Data = UraniumShapes.ArrowRight,
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
                    var rotation = 90;
                    if (TreeView.UseAnimation)
                    {
                        view.RotateTo(rotation, 90, easing: Easing.BounceOut);
                    }
                    else
                    {
                        iconArrow.Rotation = rotation;
                    }
                })
            },
            ExitActions =
            {
                new GenericTriggerAction<ButtonView>((view) =>
                {
                    var rotation = 0;
                    if (TreeView.UseAnimation)
                    {
                        view.RotateTo(rotation, 90, easing: Easing.BounceOut);
                    }
                    else
                    {
                        iconArrow.Rotation = rotation;
                    }
                })
            }
        });

        return iconArrow;
    }

    protected override void OnSizeAllocated(double width, double height)
    {
        base.OnSizeAllocated(width, height);

        if (iconArrow is not null)
        {
            iconArrow.RotationY = this.IsRtl() ? 180 : 0;
        }
    }

    protected virtual void ItemClicked()
    {
        if (TreeView.SelectionMode == SelectionMode.Single)
        {
            if (TreeView.SelectedItem == BindingContext)
            {
                TreeView.SelectedItem = null;
            }
            else
            {
                TreeView.SelectedItem = BindingContext;
            }
        }
        else if (TreeView.SelectionMode == SelectionMode.Multiple)
        {
            if (TreeView.SelectedItems.Contains(BindingContext))
            {
                TreeView.SelectedItems.Remove(BindingContext);
            }
            else
            {
                TreeView.SelectedItems.Add(BindingContext);
            }
        }
    }

    protected internal virtual void OnSelectedItemChanged()
    {
        if (TreeView.SelectionMode == SelectionMode.Single)
        {
            if (BindingContext == TreeView.SelectedItem)
            {
                IsSelected = !IsSelected;
            }
            else
            {
                IsSelected = false;
            }

            foreach (var childHolder in nodeChildren.Children.OfType<TreeViewNodeHolderView>())
            {
                childHolder.OnSelectedItemChanged();
            }
        }
    }

    protected virtual void IsSelectedChanged()
    {
        var button = this.FindInChildrenHierarchy<StatefulContentView>();

        if (IsSelected)
        {
            VisualStateManager.GoToState(this, CommonStates.Selected);
            if (TreeView.SelectionBrush is not null)
            {
                button.Background = TreeView.SelectionBrush;
            }
            else
            {
                button.BackgroundColor = TreeView.SelectionColor;
            }

            foreach (var item in button.FindManyInChildrenHierarchy<Path>())
            {
                item.StyleClass = new[] { "TreeView.Arrow.Selected" };
            }

            foreach (var item in button.FindManyInChildrenHierarchy<Label>())
            {
                item.StyleClass = new[] { "TreeView.Label.Selected" };
            }
        }
        else
        {
            VisualStateManager.GoToState(button, CommonStates.Normal);
            button.BackgroundColor = Colors.Transparent;
            button.Background = Brush.Default;

            foreach (var item in button.FindManyInChildrenHierarchy<Path>())
            {
                item.StyleClass = new[] { "TreeView.Arrow" };
            }

            foreach (var item in button.FindManyInChildrenHierarchy<Label>())
            {
                item.StyleClass = new[] { "TreeView.Label" };
            }
        }
    }

    protected virtual void AddSelectedResources(StatefulContentView button)
    {
        var surfaceColor = TreeView.SelectionColor.ToSurfaceColor();

        button.Resources.Clear();
        button.Resources.Add(new Style(typeof(Label))
        {
            CanCascade = true,
            Setters =
                {
                    new Setter
                    {
                        Property = Label.TextColorProperty, Value = surfaceColor
                    }
                }
        });

        button.Resources.Add(new Style(typeof(Path))
        {
            CanCascade = true,
            Setters =
            {
                new Setter
                {
                    Property = Path.FillProperty, Value = surfaceColor
                }
            }
        });
    }

    public virtual void ApplyIsExpandedPropertyBindings()
    {
        if (TreeView.IsExpandedPropertyName is null)
        {
            this.RemoveBinding(IsExpandedProperty);
        }
        else
        {
            this.SetBinding(IsExpandedProperty, new Binding(TreeView.IsExpandedPropertyName, BindingMode.TwoWay));
        }

        foreach (TreeViewNodeHolderView item in Children.Where(x => x is TreeViewNodeHolderView))
        {
            item.ApplyIsExpandedPropertyBindings();
        }
    }

    public virtual void ApplyIsLeafPropertyBindings()
    {
        if (TreeView.IsLeafPropertyName is null)
        {
            this.RemoveBinding(IsLeafProperty);
        }
        else
        {
            this.SetBinding(IsLeafProperty, new Binding(this.TreeView.IsLeafPropertyName, BindingMode.TwoWay));
        }

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
                nodeChildren.TranslateTo(0, 0, 50).FireAndForget();
                nodeChildren.ScaleTo(1, 50).FireAndForget();
                nodeChildren.FadeTo(1).FireAndForget();
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
                nodeChildren.TranslateTo(0, -nodeChildren.Height).FireAndForget();
                nodeChildren.ScaleTo(0).FireAndForget();
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

    public bool IsSelected { get => (bool)GetValue(IsSelectedProperty); set => SetValue(IsSelectedProperty, value); }

    public static readonly BindableProperty IsSelectedProperty = BindableProperty.Create(
        nameof(IsSelected), typeof(bool), typeof(TreeViewNodeHolderView), false,
            propertyChanged: (bindable, oldValue, newValue) => (bindable as TreeViewNodeHolderView).IsSelectedChanged());
}