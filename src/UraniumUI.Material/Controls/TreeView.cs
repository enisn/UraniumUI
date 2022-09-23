using System.Collections;
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

    private string isExpandedBinding;
    public string IsExpandedPropertyName
    {
        get => isExpandedBinding;
        set
        {
            isExpandedBinding = value;
            foreach (TreeViewNodeHolderView view in this.Children.Where(x => x is TreeViewNodeHolderView))
            {
                view.ApplyIsExpandedPropertyBindings();
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
    protected virtual void OnArrowColorChanged()
    {
        foreach (var childHolder in Children.OfType<TreeViewNodeHolderView>())
        {
            childHolder.ReFillArrowColor();
        }
    }

    public bool UseAnimation { get; set; } = true;

    /// <summary>
    /// Only indicates if TreeView is busy or not. Doesn't affect anythig visually.
    /// </summary>
    public bool IsBusy { get; set; }

    public IList ItemsSource { get => (IList)GetValue(ItemsSourceProperty); set => SetValue(ItemsSourceProperty, value); }

    public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(
        nameof(ItemsSource), typeof(IList), typeof(TreeView), null,
        propertyChanged: (b, o, v) => (b as TreeView).OnItemsSourceSet());

    public DataTemplate ItemTemplate { get => (DataTemplate)GetValue(ItemTemplateProperty); set => SetValue(ItemTemplateProperty, value); }

    public static readonly BindableProperty ItemTemplateProperty = BindableProperty.Create(
        nameof(ItemTemplate), typeof(DataTemplate), typeof(TreeView),
        defaultValue: DefaultItemTemplate, propertyChanged: (b, o, n) => (b as TreeView).OnItemTemplateChanged());

    public Color ArrowColor { get => (Color)GetValue(ArrowColorProperty); set => SetValue(ArrowColorProperty, value); }

    public static readonly BindableProperty ArrowColorProperty = BindableProperty.Create(
        nameof(ArrowColor), typeof(Color), typeof(TreeView), ColorResource.GetColor("OnBackground", "OnBackgroundDark", Colors.DarkGray),
            propertyChanged: (bindable, oldValue, newValue)=> (bindable as TreeView).OnArrowColorChanged());

    public static readonly BindableProperty IsExpandedProperty =
        BindableProperty.CreateAttached("IsExpanded", typeof(bool), typeof(TreeViewNodeHolderView), false,
            propertyChanged: (bindable, oldValue, newValue) => (bindable as TreeViewNodeHolderView).OnIsExpandedChanged());

    public static bool GetIsExpanded(BindableObject view) => (bool)view.GetValue(IsExpandedProperty);

    public static void SetIsExpanded(BindableObject view, bool value) => view.SetValue(IsExpandedProperty, value);
}