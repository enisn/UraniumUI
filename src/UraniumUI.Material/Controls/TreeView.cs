using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Input;
using UraniumUI.Extensions;
using UraniumUI.Resources;

namespace UraniumUI.Material.Controls;
public partial class TreeView : VerticalStackLayout
{
    public static DataTemplate DefaultItemTemplate = new DataTemplate(() =>
    {
        var label = new Label { VerticalOptions = LayoutOptions.Center };
        label.SetBinding(Label.TextProperty, new Binding("Name"));
        return label;
    });

    public TreeView()
    {
        BindableLayout.SetItemTemplate(this, new DataTemplate(() =>
        {
            var holder = new TreeViewNodeHolderView(ItemTemplate, this, ChildrenBinding);
            holder.TreeView = this;
            return holder;
        }));
    }

    protected override void OnHandlerChanged()
    {
        base.OnHandlerChanged();

        if (SelectedItems is INotifyCollectionChanged observableSelectedItems)
        {
            if (Handler is null)
            {
                observableSelectedItems.CollectionChanged -= SelectedItemsChanged;
            }
            else
            {
                observableSelectedItems.CollectionChanged += SelectedItemsChanged;
            }
        }
    }

    private BindingBase childrenBinding = new Binding("Children");

    public BindingBase ChildrenBinding
    {
        get => childrenBinding; set
        {
            childrenBinding = value;
            foreach (TreeViewNodeHolderView view in this.Children.Where(x => x is TreeViewNodeHolderView))
            {
                view.ChildrenBinding = value;
            }
        }
    }

    private string isExpandedPropertyName = "IsExpanded";

    public string IsExpandedPropertyName
    {
        get => isExpandedPropertyName;
        set
        {
            isExpandedPropertyName = value;
            foreach (TreeViewNodeHolderView view in this.Children.Where(x => x is TreeViewNodeHolderView))
            {
                view.ApplyIsExpandedPropertyBindings();
            }
        }
    }

    private string isLeafPropertyName = "IsLeaf";

    public string IsLeafPropertyName
    {
        get => isLeafPropertyName;
        set
        {
            isLeafPropertyName = value;
            foreach (TreeViewNodeHolderView view in this.Children.Where(x => x is TreeViewNodeHolderView))
            {
                view.ApplyIsLeafPropertyBindings();
            }
        }
    }

    protected virtual void OnItemsSourceSet()
    {
        BindableLayout.SetItemsSource(this, ItemsSource);
    }

    private void OnItemTemplateChanged()
    {
        BindableLayout.SetItemTemplate(this, new DataTemplate(() =>
        {
            var holder = new TreeViewNodeHolderView(ItemTemplate, this, ChildrenBinding);
            holder.TreeView = this;
            return holder;
        }));

        OnItemsSourceSet();
    }

    private void OnExpanderTemplateChanged()
    {
        // Same logic (for now)
        OnItemTemplateChanged();
    }

    protected virtual void SelectedItemChanged()
    {
        if (SelectionMode == SelectionMode.None)
        {
            return;
        }

        foreach (var childHolder in Children.OfType<TreeViewNodeHolderView>())
        {
            childHolder.OnSelectedItemChanged();
        }
    }

    protected virtual void OnSelectedItemsChanged(IList oldValue, IList newValue)
    {
        if (oldValue is INotifyCollectionChanged observableOld)
        {
            observableOld.CollectionChanged -= SelectedItemsChanged;
        }

        if (newValue is INotifyCollectionChanged observableCollectionNew)
        {
            observableCollectionNew.CollectionChanged += SelectedItemsChanged;
        }

        foreach (var childNode in this.FindManyInChildrenHierarchy<TreeViewNodeHolderView>())
        {
            if (newValue.Contains(childNode.BindingContext) && !childNode.IsSelected)
            {
                childNode.IsSelected = true;
            }
        }
    }

    private void SelectedItemsChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                foreach (var item in e.NewItems)
                {
                    var node = this.FindManyInChildrenHierarchy<TreeViewNodeHolderView>().FirstOrDefault(x => x.BindingContext == item);
                    if (node is not null && !node.IsSelected)
                    {
                        node.IsSelected = true;
                    }
                }
                break;
            case NotifyCollectionChangedAction.Remove:
                foreach (var item in e.OldItems)
                {
                    var node = this.FindManyInChildrenHierarchy<TreeViewNodeHolderView>().FirstOrDefault(x => x.BindingContext == item);
                    if (node is not null && node.IsSelected)
                    {
                        node.IsSelected = false;
                    }
                }
                break;
        }
    }

    public SelectionMode SelectionMode { get; set; }

    public bool UseAnimation { get; set; } = true;

    /// <summary>
    /// Only indicates if TreeView is busy or not. Doesn't affect anything visually.
    /// </summary>
    public bool IsBusy { get; set; }

    public IList ItemsSource { get => (IList)GetValue(ItemsSourceProperty); set => SetValue(ItemsSourceProperty, value); }

    public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(
        nameof(ItemsSource), typeof(IList), typeof(TreeView), null,
        propertyChanged: (b, o, v) => (b as TreeView).OnItemsSourceSet());

    public object SelectedItem { get => GetValue(SelectedItemProperty); set => SetValue(SelectedItemProperty, value); }

    public static readonly BindableProperty SelectedItemProperty = BindableProperty.Create(
        nameof(SelectedItem), typeof(object), typeof(TreeView), default, propertyChanged: (bo, ov, nv) => (bo as TreeView).SelectedItemChanged());

    public IList SelectedItems { get => (IList)GetValue(SelectedItemsProperty); set => SetValue(SelectedItemsProperty, value); }

    public static readonly BindableProperty SelectedItemsProperty = BindableProperty.Create(
        nameof(SelectedItems), typeof(IList), typeof(TreeView), defaultValueCreator: bindable => new ObservableCollection<object>(),
        propertyChanged: (bo, ov, nv) => (bo as TreeView).OnSelectedItemsChanged((IList)ov, (IList)nv));

    public DataTemplate ExpanderTemplate { get => (DataTemplate)GetValue(ExpanderTemplateProperty); set => SetValue(ExpanderTemplateProperty, value); }

    public static readonly BindableProperty ExpanderTemplateProperty = BindableProperty.Create(
        nameof(ExpanderTemplate), typeof(DataTemplate), typeof(TreeView), null,
        propertyChanged: (b, o, v) => (b as TreeView).OnExpanderTemplateChanged());

    public DataTemplate ItemTemplate { get => (DataTemplate)GetValue(ItemTemplateProperty); set => SetValue(ItemTemplateProperty, value); }

    public static readonly BindableProperty ItemTemplateProperty = BindableProperty.Create(
        nameof(ItemTemplate), typeof(DataTemplate), typeof(TreeView),
        defaultValue: DefaultItemTemplate, propertyChanged: (b, o, n) => (b as TreeView).OnItemTemplateChanged());

    public ICommand LoadChildrenCommand { get => (ICommand)GetValue(LoadChildrenCommandProperty); set => SetValue(LoadChildrenCommandProperty, value); }

    public static readonly BindableProperty LoadChildrenCommandProperty = BindableProperty.Create(
        nameof(LoadChildrenCommand), typeof(ICommand), typeof(TreeView), null);

    public Color SelectionColor { get => (Color)GetValue(SelectionColorProperty); set => SetValue(SelectionColorProperty, value); }

    public static readonly BindableProperty SelectionColorProperty = BindableProperty.Create(
        nameof(SelectionColor), typeof(Color), typeof(TreeView), ColorResource.GetColor("Secondary", "SecondaryDark", Colors.Pink));
}