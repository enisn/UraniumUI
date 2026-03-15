using InputKit.Shared.Helpers;
using System.Collections;
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

    protected TreeViewNodeItemContentView nodeContainer = new TreeViewNodeItemContentView();

    public CollectionView NodeChildren => nodeChildren;

    internal protected CollectionView nodeChildren;
    private Grid childContainer;
    private bool isReleased;

    public DataTemplate DataTemplate { get; }

    protected Grid rowStack = new Grid
    {
        ColumnDefinitions =
        {
            new ColumnDefinition(40),
            new ColumnDefinition(GridLength.Star),
        }
    };
    private readonly int indentLevel;
    private bool hasLoadedChildren;
    private BindingBase childrenBinding;
    public BindingBase ChildrenBinding
    {
        get => childrenBinding;
        internal set
        {
            childrenBinding = value;
            if (nodeChildren != null)
            {
                if (ChildrenBinding is Binding binding)
                {
                    nodeChildren.SetBinding(ItemsView.ItemsSourceProperty, new Binding(binding.Path));
                }
                else
                {
                    nodeChildren.RemoveBinding(ItemsView.ItemsSourceProperty);
                }
            }
        }
    }

    private protected View expanderView;
    private protected ButtonView iconArrow;

    public TreeViewNodeHolderView(DataTemplate dataTemplate, TreeView treeView, BindingBase childrenBinding, int indentLevel = 0)
    {
        if (indentLevel > 20) 
            throw new ArgumentException("Indent level exceeds the maximum allowed value of 20, which may indicate runaway recursion.", nameof(indentLevel));

        TreeView = treeView ?? throw new ArgumentNullException(nameof(treeView));
        treeView.RegisterNode(this);

        this.indentLevel = indentLevel;

        DataTemplate = dataTemplate;

        nodeContainer.ItemTemplate = DataTemplate;
        nodeContainer.SetBinding(TreeViewNodeItemContentView.ItemProperty, ".");

        expanderView = TreeView.ExpanderTemplate?.CreateContent() as View ?? InitializeArrowExpander();
        expanderView.BindingContext = this;

        rowStack.Add(expanderView);
        rowStack.Add(nodeContainer, column: 1);

        var button = new StatefulContentView
        {
            Content = rowStack,
            Padding = new Thickness(indentLevel * 16, 0, 0, 0),
#if WINDOWS
            GestureRecognizers = { new TapGestureRecognizer { Command = new Command(ItemClicked) } },
#else
            TappedCommand = new Command(ItemClicked),
#endif
        };

        this.Add(button);
        if (childrenBinding is Binding binding && !string.IsNullOrEmpty(binding.Path) &&
            TryGetInitialChildren(binding.Path, out var children) && 
            children is IEnumerable enumerable && enumerable.Cast<object>().Any())
        {
            CreateChildContainer(binding);
        }
        ChildrenBinding = childrenBinding;
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
#if WINDOWS
            GestureRecognizers = { new TapGestureRecognizer { Command = new Command(() => IsExpanded = !IsExpanded) } },
#else
            PressedCommand = new Command(() => IsExpanded = !IsExpanded),
#endif
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
                        view.RotateToSafely(rotation, 90, easing: Easing.BounceOut);
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
                        view.RotateToSafely(rotation, 90, easing: Easing.BounceOut);
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

    protected override void OnBindingContextChanged()
    {
        base.OnBindingContextChanged();
        OnSelectedItemChanged();

        if (nodeChildren == null && ChildrenBinding is Binding binding &&
        !string.IsNullOrEmpty(binding.Path) &&
        TryGetInitialChildren(binding.Path, out var children) &&
        children is IEnumerable enumerable && enumerable.Cast<object>().Any())
        {
            CreateChildContainer(binding);
            // 🔥 Now that nodeChildren is created, we can safely expand
            if (IsExpanded)
            {
                OnIsExpandedChanged(true);
            }
        }
        if (!string.IsNullOrEmpty(TreeView.IsExpandedPropertyName))
        {
            this.SetBinding(IsExpandedProperty, new Binding(TreeView.IsExpandedPropertyName, BindingMode.TwoWay));
        }

        if (!string.IsNullOrEmpty(TreeView.IsLeafPropertyName))
        {
            this.SetBinding(IsLeafProperty, new Binding(TreeView.IsLeafPropertyName, BindingMode.TwoWay));
        }
    }

    protected override void OnHandlerChanging(HandlerChangingEventArgs args)
    {
        if (args.NewHandler is null)
        {
            Release();
        }

        base.OnHandlerChanging(args);
    }

    private void CreateChildContainer(Binding binding)
    {
        nodeChildren = new CollectionView
        {
            IsVisible = false,
            ItemsLayout = new LinearItemsLayout(ItemsLayoutOrientation.Vertical)
            {
                ItemSpacing = TreeView.ItemSpacing
            },
            SelectionMode = SelectionMode.None
        };

        childContainer = new Grid
        {
            HeightRequest = 0,
            IsVisible = false
        };
        childContainer.Children.Add(nodeChildren);
        this.Add(childContainer);


        nodeChildren.ItemTemplate = new DataTemplate(() =>
        {
            var childNode = new TreeViewNodeHolderView(DataTemplate, TreeView, ChildrenBinding, this.indentLevel + 1)
            {
                ParentHolderView = this
            };
            return childNode;
        });

        nodeChildren.SetBinding(CollectionView.ItemsSourceProperty, new Binding(binding.Path));
        nodeChildren.ChildAdded += (s, e) => OnPropertyChanged(nameof(IsLeaf));
        nodeChildren.ChildRemoved += (s, e) => OnPropertyChanged(nameof(IsLeaf));
    }

    internal void Release()
    {
        if (isReleased)
        {
            return;
        }

        isReleased = true;

        if (nodeChildren is not null)
        {
            nodeChildren.IsVisible = false;
            nodeChildren.RemoveBinding(ItemsView.ItemsSourceProperty);
            nodeChildren.ClearValue(ItemsView.ItemsSourceProperty);
            nodeChildren.ItemsSource = null;
        }

        if (childContainer is not null)
        {
            childContainer.IsVisible = false;
            childContainer.HeightRequest = 0;
        }

        TreeView?.UnregisterNode(this);
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
            if (NodeChildren?.ItemsSource is IEnumerable items)
            {
                foreach (var item in items)
                {
                    var holder = FindHolderView(item);
                    holder?.OnSelectedItemChanged();
                }
            }
        }
    }
    protected TreeViewNodeHolderView FindHolderView(object item)
    {
        return NodeChildren.Handler?.PlatformView is not null
            ? TreeView.FindManyInChildrenHierarchy<TreeViewNodeHolderView>()
                .FirstOrDefault(x => x.BindingContext == item)
            : null;
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
        if (isExpanded && !IsLeaf)
        {
            if (!hasLoadedChildren && ChildrenBinding is Binding binding)
            {
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    if (nodeChildren == null)
                    {
                        CreateChildContainer(binding);
                    }

                    if (nodeChildren.ItemsSource is IEnumerable items && !items.Cast<object>().Any())
                    {
                        LoadChildrenIfNecessary();
                    }

                    this.InvalidateMeasure();
                });
            }

            if (TreeView.UseAnimation)
            {
                nodeChildren.IsVisible = true;
                nodeChildren.TranslateToSafely(0, 0, 50).FireAndForget();
                nodeChildren.ScaleToSafely(1, 50).FireAndForget();
                nodeChildren.FadeToSafely(1).FireAndForget();
            }
            else
            {
                nodeChildren.IsVisible = true;
            }

            childContainer.IsVisible = true;
            childContainer.ClearValue(HeightRequestProperty); // Let it auto-resize naturally
        }
        else
        {
            if (nodeChildren != null && childContainer != null)
            {
                if (TreeView.UseAnimation)
                {
                    nodeChildren.TranslateToSafely(0, -nodeChildren.Height).FireAndForget();
                    nodeChildren.ScaleToSafely(0).FireAndForget();
                    nodeChildren.AnchorX = 0;
                    nodeChildren.AnchorY = 0;

                    await nodeChildren.FadeToSafely(0, 50);
                }
                childContainer.IsVisible = false;
                childContainer.HeightRequest = 0;
                nodeChildren.IsVisible = false;
            }
        }
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
        if (!IsLeaf && !hasLoadedChildren && (NodeChildren == null || (NodeChildren.ItemsSource is IEnumerable items && !items.Cast<object>().Any())))
        {
            TreeView.LoadChildrenCommand?.Execute(BindingContext);
            hasLoadedChildren = true;
        }
    }

    public bool IsLeaf
    {
        get => (bool?)GetValue(IsLeafProperty) ?? nodeChildren == null || nodeChildren.ItemsSource is not IEnumerable items || !items.Cast<object>().Any();
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

    private bool TryGetInitialChildren(string path, out IEnumerable children)
    {
        children = null;

        if (BindingContext == null)
            return false;

        try
        {
            var prop = BindingContext.GetType().GetProperty(path);
            var value = prop?.GetValue(BindingContext);

            if (value is IEnumerable list)
            {
                children = list;
                return true;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error in TryGetInitialChildren: {ex.Message}");
            System.Diagnostics.Debug.WriteLine(ex.StackTrace);
        }
        return false;
    }
}
