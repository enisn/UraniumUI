using System.Collections;
using System.Collections.Specialized;

namespace UraniumUI.Controls;
public class Dropdown : Button, IDropdown
{
    public object SelectedItem { get => GetValue(SelectedItemProperty); set => SetValue(SelectedItemProperty, value); }

    public static readonly BindableProperty SelectedItemProperty = BindableProperty.Create(
        nameof(SelectedItem), typeof(object), typeof(Dropdown));

    public IList ItemsSource { get => (IList)GetValue(ItemsSourceProperty); set => SetValue(ItemsSourceProperty, value); } 

    public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(
        nameof(ItemsSource), typeof(IList), typeof(Dropdown),
        propertyChanged: (bo, ov, nv)=> (bo as Dropdown).OnItemsSourceChanged((IList)ov, (IList)nv));

    protected virtual void OnItemsSourceChanged(IList oldValue, IList newValue)
    {
        if (oldValue is INotifyCollectionChanged oldObservable)
        {
            oldObservable.CollectionChanged -= ItemsSource_CollectionChanged;
        }

        if (newValue is INotifyCollectionChanged newObservable)
        {
            newObservable.CollectionChanged += ItemsSource_CollectionChanged;
        }

        if (SelectedItem is null && ItemsSource.Count > 0)
        {
            SelectedItem = ItemsSource[0];
        }
    }

    private void ItemsSource_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {

    }
}
