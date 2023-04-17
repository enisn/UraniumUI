using System.Collections;
using System.Collections.ObjectModel;

namespace UraniumUI.Material.Controls;
public partial class TabView
{
    public IList<TabItem> Items { get => (IList<TabItem>)GetValue(ItemsProperty); set => SetValue(ItemsProperty, value); }

    public static BindableProperty ItemsProperty = BindableProperty.Create(
        nameof(Items),
        typeof(IList<TabItem>),
        typeof(TabView), null,
        defaultBindingMode: BindingMode.TwoWay,
        propertyChanged: (bindable, oldValue, newValue) => (bindable as TabView).OnItemsChanged((IList<TabItem>)oldValue, (IList<TabItem>)newValue));

    public DataTemplate TabHeaderItemTemplate { get => (DataTemplate)GetValue(TabHeaderItemTemplateProperty); set => SetValue(TabHeaderItemTemplateProperty, value); }

    public static readonly BindableProperty TabHeaderItemTemplateProperty =
        BindableProperty.Create(nameof(TabHeaderItemTemplate), typeof(DataTemplate), typeof(TabView), defaultValue: TabView.DefaultTabHeaderItemTemplate,
            propertyChanged: (bo, ov, nv) => (bo as TabView).RenderHeaders());

    public object CurrentItem { get => GetValue(CurrentItemProperty); set => SetValue(CurrentItemProperty, value); }

    public static readonly BindableProperty CurrentItemProperty =
        BindableProperty.Create(nameof(Items), typeof(object), typeof(TabView),
            propertyChanged: (bo, ov, nv) => (bo as TabView).OnCurrentItemChanged(nv));

    public TabViewTabPlacement TabPlacement { get => (TabViewTabPlacement)GetValue(TabPlacementProperty); set => SetValue(TabPlacementProperty, value); }

    public static readonly BindableProperty TabPlacementProperty =
        BindableProperty.Create(nameof(TabPlacement), typeof(TabViewTabPlacement), typeof(TabView), defaultValue: TabViewTabPlacement.Top,
            propertyChanged: (bo, ov, nv) => (bo as TabView).OnTabPlacementChanged());

    public IList ItemsSource { get => (IList)GetValue(ItemsSourceProperty); set => SetValue(ItemsSourceProperty, value); }

    public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(
        nameof(ItemsSource),
        typeof(IList),
        typeof(TabView),
        propertyChanged: (bindable, oldValue, newValue) => (bindable as TabView).OnItemsSourceChanged((IList)oldValue, (IList)newValue));

    public DataTemplate ItemTemplate { get => (DataTemplate)GetValue(ItemTemplateProperty); set => SetValue(ItemTemplateProperty, value); }

    public static readonly BindableProperty ItemTemplateProperty = BindableProperty.Create(
        nameof(ItemTemplate),
        typeof(DataTemplate),
        typeof(TabView),
        propertyChanged: (bindable, oldValue, newValue) => (bindable as TabView).OnItemTemplateChanged());

    public TabItem SelectedTab { get => (TabItem)GetValue(SelectedTabProperty); set => SetValue(SelectedTabProperty, value); }

    public static readonly BindableProperty SelectedTabProperty = BindableProperty.Create(
        nameof(SelectedTab),
        typeof(TabItem),
        typeof(TabView),
        propertyChanged: (bindable, oldValue, newValue) 
            => (bindable as TabView).OnSelectedTabChanged((TabItem)oldValue, (TabItem)newValue));
}
