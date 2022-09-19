using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;

namespace UraniumUI.Material.Controls;
public partial class TreeView : VerticalStackLayout
{

    public TreeView()
    {
        BindableLayout.SetItemTemplate(this, new DataTemplate(() =>
        {
            return new TreeViewNodeHolderView();
        }));
    }

    public BindingBase ChildrenBinding { get; set; } = new Binding("Children");

    protected virtual void OnItemsSourceSet()
    {
        BindableLayout.SetItemsSource(this, ItemsSource);
    }

    private void OnItemTemplateChanged()
    {

    }

    public IList ItemsSource { get => (IList)GetValue(ItemsSourceProperty); set => SetValue(ItemsSourceProperty, value); }

    public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(
        nameof(ItemsSource), typeof(IList), typeof(TreeView), null,
        propertyChanged: (b, o, v) => (b as TreeView).OnItemsSourceSet());

    public DataTemplate ItemTemplate { get => (DataTemplate)GetValue(ItemTemplateProperty); set => SetValue(ItemTemplateProperty, value); }

    public static readonly BindableProperty ItemTemplateProperty = BindableProperty.Create(
        nameof(ItemTemplate), typeof(DataTemplate), typeof(TreeView),
        new DataTemplate(typeof(TreeViewNodeHolderView)), propertyChanged: (b, o, n) => (b as TreeView).OnItemTemplateChanged());

    public static readonly BindableProperty IsExpandedProperty =
        BindableProperty.CreateAttached("IsExpanded", typeof(bool), typeof(TreeViewNodeHolderView), false,
            propertyChanged: (bindable, oldValue, newValue) => (bindable as TreeViewNodeHolderView).OnIsExpandedChanged());

    public static bool GetIsExpanded(BindableObject view) => (bool)view.GetValue(IsExpandedProperty);

    public static void SetIsExpanded(BindableObject view, bool value) => view.SetValue(IsExpandedProperty, value);
}
