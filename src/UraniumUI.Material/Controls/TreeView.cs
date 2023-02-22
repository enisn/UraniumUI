using System.Collections;
using System.Windows.Input;
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
        Spacing = 10;
        BindableLayout.SetItemTemplate(this, new DataTemplate(() =>
        {
            var holder = new TreeViewNodeHolderView(ItemTemplate, this, ChildrenBinding);
            holder.TreeView = this;
            return holder;
        }));
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

    protected virtual void OnArrowColorChanged()
    {
        foreach (var childHolder in Children.OfType<TreeViewNodeHolderView>())
        {
            childHolder.ReFillArrowColor();
        }
    }

    public bool UseAnimation { get; set; } = true;

    /// <summary>
    /// Only indicates if TreeView is busy or not. Doesn't affect anything visually.
    /// </summary>
    public bool IsBusy { get; set; }

    public IList ItemsSource { get => (IList)GetValue(ItemsSourceProperty); set => SetValue(ItemsSourceProperty, value); }

    public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(
        nameof(ItemsSource), typeof(IList), typeof(TreeView), null,
        propertyChanged: (b, o, v) => (b as TreeView).OnItemsSourceSet());

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

    public Color ArrowColor { get => (Color)GetValue(ArrowColorProperty); set => SetValue(ArrowColorProperty, value); }

    public static readonly BindableProperty ArrowColorProperty = BindableProperty.Create(
        nameof(ArrowColor), typeof(Color), typeof(TreeView), ColorResource.GetColor("OnBackground", "OnBackgroundDark", Colors.DarkGray),
            propertyChanged: (bindable, oldValue, newValue)=> (bindable as TreeView).OnArrowColorChanged());
}