using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Input;
using UraniumUI.Extensions;
using UraniumUI.Resources;

namespace UraniumUI.Material.Controls;
public partial class TreeView : ContentView
{
    public static DataTemplate DefaultItemTemplate = new DataTemplate(() =>
    {
        var label = new Label { VerticalOptions = LayoutOptions.Center };
        label.SetBinding(Label.TextProperty, new Binding("Name"));
        return label;
    });

    private readonly List<TreeViewNodeHolderView> registeredNodes = new();
    public IEnumerable<TreeViewNodeHolderView> AllNodeViews => registeredNodes;
    private List<TreeViewNodeHolderView> GetChildViews() => registeredNodes.ToList();
    private readonly CollectionView rootView;
    private bool isTemplateUpdating;
    private DataTemplate nodeTemplate;

    internal void RegisterNode(TreeViewNodeHolderView node)
    {
        registeredNodes.Add(node);
    }

    internal void UnregisterNode(TreeViewNodeHolderView node)
    {
        registeredNodes.Remove(node);
    }

    internal void ReleaseRegisteredNodes()
    {
        foreach (var node in registeredNodes.ToList())
        {
            node.Release();
        }

        registeredNodes.Clear();
    }

    internal IEnumerable<TreeViewNodeHolderView> GetChildViewsOf(TreeViewNodeHolderView parent)
    {
        return registeredNodes.Where(x => x.ParentHolderView == parent);
    }

    public TreeView()
    {
        rootView = new CollectionView
        {
            ItemsLayout = new LinearItemsLayout(ItemsLayoutOrientation.Vertical),
            SelectionMode = SelectionMode.None
        };

        Content = rootView;

        OnItemTemplateChanged(DefaultItemTemplate);
    }

    protected override void OnHandlerChanged()
    {
        base.OnHandlerChanged();

        if (SelectedItems is INotifyCollectionChanged observableSelectedItems)
        {
            if (Handler is null)
                observableSelectedItems.CollectionChanged -= SelectedItemsChanged;
            else
                observableSelectedItems.CollectionChanged += SelectedItemsChanged;
        }
    }

    protected override void OnHandlerChanging(HandlerChangingEventArgs args)
    {
        if (args.NewHandler is null)
        {
            ReleaseRegisteredNodes();
        }

        base.OnHandlerChanging(args);
    }

    // TODO: Remove default value and make default value as null in the next major version.
    private BindingBase childrenBinding = new Binding("Children");
    public BindingBase ChildrenBinding
    {
        get => childrenBinding;
        set
        {
            childrenBinding = value;
            foreach (TreeViewNodeHolderView view in GetChildViews())
            {
                view.ChildrenBinding = value;
            }
        }
    }

    private string isExpandedPropertyName;
    public string IsExpandedPropertyName
    {
        get => isExpandedPropertyName;
        set
        {
            isExpandedPropertyName = value;
            foreach (TreeViewNodeHolderView view in GetChildViews())
            {
                view.ApplyIsExpandedPropertyBindings();
            }
        }
    }

    private string isLeafPropertyName;
    public string IsLeafPropertyName
    {
        get => isLeafPropertyName;
        set
        {
            isLeafPropertyName = value;
            foreach (TreeViewNodeHolderView view in GetChildViews())
            {
                view.ApplyIsLeafPropertyBindings();
            }
        }
    }

    private void OnItemTemplateChanged(DataTemplate newTemplate)
    {
        if (isTemplateUpdating || newTemplate == null)
            return;

        try
        {
            isTemplateUpdating = true;

            nodeTemplate = newTemplate;

            rootView.ItemTemplate = new DataTemplate(() =>
            {
                var holder = new TreeViewNodeHolderView(nodeTemplate, this, ChildrenBinding)
                {
                    TreeView = this
                };
                return holder;
            });
        }
        finally
        {
            isTemplateUpdating = false;
        }
    }

    private void OnExpanderTemplateChanged()
    {
        OnItemTemplateChanged(ItemTemplate);
    }

    protected virtual void SelectedItemChanged()
    {
        if (SelectionMode == SelectionMode.None)
            return;

        foreach (TreeViewNodeHolderView childHolder in GetChildViews())
        {
            childHolder.OnSelectedItemChanged();
        }
    }

    protected virtual void OnSelectedItemsChanged(IList oldValue, IList newValue)
    {
        if (oldValue is INotifyCollectionChanged observableOld)
            observableOld.CollectionChanged -= SelectedItemsChanged;

        if (newValue is INotifyCollectionChanged observableNew)
            observableNew.CollectionChanged += SelectedItemsChanged;

        foreach (TreeViewNodeHolderView childNode in registeredNodes)
        {
            if (newValue.Contains(childNode.BindingContext) && !childNode.IsSelected)
            {
                childNode.IsSelected = true;
            }
        }
    }

    private void SelectedItemsChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == NotifyCollectionChangedAction.Add)
        {
            foreach (var item in e.NewItems)
            {
                var node = registeredNodes
                    .FirstOrDefault(x => x.BindingContext == item);
                if (node != null && !node.IsSelected)
                    node.IsSelected = true;
            }
        }
        else if (e.Action == NotifyCollectionChangedAction.Remove)
        {
            foreach (var item in e.OldItems)
            {
                var node = registeredNodes
                    .FirstOrDefault(x => x.BindingContext == item);
                if (node != null && node.IsSelected)
                    node.IsSelected = false;
            }
        }
    }

    public SelectionMode SelectionMode
    {
        get => (SelectionMode)GetValue(SelectionModeProperty);
        set => SetValue(SelectionModeProperty, value);
    }

    public static readonly BindableProperty SelectionModeProperty = BindableProperty.Create(
        nameof(SelectionMode), typeof(SelectionMode), typeof(TreeView), SelectionMode.None);

    public bool UseAnimation
    {
        get => (bool)GetValue(UseAnimationProperty);
        set => SetValue(UseAnimationProperty, value);
    }

    public static readonly BindableProperty UseAnimationProperty = BindableProperty.Create(
        nameof(UseAnimation), typeof(bool), typeof(TreeView), true);

    public bool IsBusy { get; set; }

    public IEnumerable ItemsSource
    {
        get => (IEnumerable)GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(
        nameof(ItemsSource), typeof(IEnumerable), typeof(TreeView),
        propertyChanged: (bindable, oldValue, newValue) =>
        {
            if (bindable is TreeView tree)
            {
                tree.rootView.ItemsSource = (IEnumerable)newValue;
            }
        });

    public object SelectedItem
    {
        get => GetValue(SelectedItemProperty);
        set => SetValue(SelectedItemProperty, value);
    }

    public static readonly BindableProperty SelectedItemProperty = BindableProperty.Create(
        nameof(SelectedItem), typeof(object), typeof(TreeView), default, BindingMode.TwoWay,
        propertyChanged: (bo, ov, nv) => (bo as TreeView)?.SelectedItemChanged());

    public IList SelectedItems
    {
        get => (IList)GetValue(SelectedItemsProperty);
        set => SetValue(SelectedItemsProperty, value);
    }

    public static readonly BindableProperty SelectedItemsProperty = BindableProperty.Create(
        nameof(SelectedItems), typeof(IList), typeof(TreeView),
        defaultValueCreator: bindable => new ObservableCollection<object>(),
        propertyChanged: (bo, ov, nv) => (bo as TreeView)?.OnSelectedItemsChanged((IList)ov, (IList)nv));

    public DataTemplate ExpanderTemplate
    {
        get => (DataTemplate)GetValue(ExpanderTemplateProperty);
        set => SetValue(ExpanderTemplateProperty, value);
    }

    public static readonly BindableProperty ExpanderTemplateProperty = BindableProperty.Create(
        nameof(ExpanderTemplate), typeof(DataTemplate), typeof(TreeView), null,
        propertyChanged: (b, o, v) => (b as TreeView)?.OnExpanderTemplateChanged());

    public DataTemplate ItemTemplate
    {
        get => (DataTemplate)GetValue(ItemTemplateProperty);
        set => SetValue(ItemTemplateProperty, value);
    }

    public static readonly BindableProperty ItemTemplateProperty = BindableProperty.Create(
        nameof(ItemTemplate), typeof(DataTemplate), typeof(TreeView),
        defaultValue: DefaultItemTemplate,
        propertyChanged: (b, o, n) => (b as TreeView)?.OnItemTemplateChanged((DataTemplate)n));

    public ICommand LoadChildrenCommand
    {
        get => (ICommand)GetValue(LoadChildrenCommandProperty);
        set => SetValue(LoadChildrenCommandProperty, value);
    }

    public static readonly BindableProperty LoadChildrenCommandProperty = BindableProperty.Create(
        nameof(LoadChildrenCommand), typeof(ICommand), typeof(TreeView), null);

    public Color SelectionColor
    {
        get => (Color)GetValue(SelectionColorProperty);
        set => SetValue(SelectionColorProperty, value);
    }

    public static readonly BindableProperty SelectionColorProperty = BindableProperty.Create(
        nameof(SelectionColor), typeof(Color), typeof(TreeView),
        ColorResource.GetColor("Secondary", "SecondaryDark", Colors.Pink));

    public Brush SelectionBrush
    {
        get => (Brush)GetValue(SelectionBrushProperty);
        set => SetValue(SelectionBrushProperty, value);
    }

    public static readonly BindableProperty SelectionBrushProperty = BindableProperty.Create(
        nameof(SelectionBrush), typeof(Brush), typeof(TreeView), null);

    public double ItemSpacing
    {
        get => (double)GetValue(ItemSpacingProperty);
        set => SetValue(ItemSpacingProperty, value);
    }

    public static readonly BindableProperty ItemSpacingProperty = BindableProperty.Create(
        nameof(ItemSpacing), typeof(double), typeof(TreeView), 0.0);
}
