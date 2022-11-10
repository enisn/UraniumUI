using System.Collections.ObjectModel;

namespace UraniumUI.Material.Controls;
public partial class TabView
{
	public IList<TabItem> Items { get => (IList<TabItem>)GetValue(ItemsProperty); set => SetValue(ItemsProperty, value); }

	public static BindableProperty ItemsProperty = BindableProperty.Create(
		nameof(Items),
		typeof(IList<TabItem>),
		typeof(TabView), new ObservableCollection<TabItem>(),
		defaultBindingMode: BindingMode.TwoWay,
		propertyChanged: (bindable, oldValue, newValue) => (bindable as TabView).OnItemsChanged((IList<TabItem>)oldValue, (IList<TabItem>)newValue));

	public DataTemplate TabHeaderItemTemplate { get => (DataTemplate)GetValue(TabHeaderItemTemplateProperty); set => SetValue(TabHeaderItemTemplateProperty, value); }

	public static readonly BindableProperty TabHeaderItemTemplateProperty =
		BindableProperty.Create(nameof(TabHeaderItemTemplate), typeof(DataTemplate), typeof(TabView), defaultValue: TabView.DefaultTabHeaderItemTemplate,
			propertyChanged: (bo, ov, nv) => (bo as TabView).RenderHeaders());

	public TabItem CurrentItem { get => (TabItem)GetValue(CurrentItemProperty); set => SetValue(CurrentItemProperty, value); }

	public static readonly BindableProperty CurrentItemProperty =
		BindableProperty.Create(nameof(Items), typeof(TabItem), typeof(TabView),
			propertyChanged: (bo, ov, nv) => (bo as TabView).OnCurrentItemChanged((TabItem)nv));

	public TabViewTabPlacement TabPlacement { get => (TabViewTabPlacement)GetValue(TabPlacementProperty); set => SetValue(TabPlacementProperty, value); }

	public static readonly BindableProperty TabPlacementProperty =
		BindableProperty.Create(nameof(TabPlacement), typeof(TabViewTabPlacement), typeof(TabView), defaultValue: TabViewTabPlacement.Top,
			propertyChanged: (bo, ov, nv) => (bo as TabView).OnTabPlacementChanged());
}
