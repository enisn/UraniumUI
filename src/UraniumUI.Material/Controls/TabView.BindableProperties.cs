using System.Collections.ObjectModel;

namespace UraniumUI.Material.Controls;
public partial class TabView
{
    public DataTemplate TabHeaderItemTemplate { get => (DataTemplate)GetValue(TabHeaderItemTemplateProperty); set => SetValue(TabHeaderItemTemplateProperty, value); }

    public static readonly BindableProperty TabHeaderItemTemplateProperty =
        BindableProperty.Create(nameof(TabHeaderItemTemplate), typeof(DataTemplate), typeof(TabView), defaultValue: TabView.DefaultTabHeaderItemTemplate,
            propertyChanged: (bo, ov, nv) => (bo as TabView).RenderHeaders());

    public IList<TabItem> Items { get => (IList<TabItem>)GetValue(ItemsProperty); set => SetValue(ItemsProperty, value); }

    public static readonly BindableProperty ItemsProperty =
    BindableProperty.Create(nameof(Items), typeof(IList<TabItem>), typeof(TabView), defaultValue: new ObservableCollection<TabItem>(),
        propertyChanged: (bo, ov, nv) => (bo as TabView).Render());

    public TabItem CurrentItem { get; set; }

    public static readonly BindableProperty CurrentItemProperty =
        BindableProperty.Create(nameof(Items), typeof(TabItem), typeof(TabView),
            propertyChanged: (bo, ov, nv)=> (bo as TabView).OnCurrentItemChanged((TabItem)nv));
}