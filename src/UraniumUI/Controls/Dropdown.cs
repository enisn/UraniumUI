using System.Collections;
using System.Collections.Specialized;

namespace UraniumUI.Controls;
public class Dropdown : Button, IDropdown
{
    public Dropdown()
    {
        this.SetAppThemeColor(Button.TextColorProperty, Colors.Black, Colors.White);
        this.SetAppThemeColor(Button.BackgroundColorProperty, Colors.Transparent, Colors.Transparent);
    }

    private BindingBase itemDisplayBinding;
    public BindingBase ItemDisplayBinding { get => itemDisplayBinding; set
        {
            itemDisplayBinding = value;
            OnPropertyChanged();
        }
    }

    public TextAlignment HorizontalTextAlignment { get => (TextAlignment)GetValue(HorizontalTextAlignmentProperty); set => SetValue(HorizontalTextAlignmentProperty, value); }

    public static readonly BindableProperty HorizontalTextAlignmentProperty = BindableProperty.Create(
        nameof(HorizontalTextAlignment), typeof(TextAlignment), typeof(Dropdown),
        defaultValue: TextAlignment.Start);

    public Color PlaceholderColor { get => (Color)GetValue(PlaceholderColorProperty); set => SetValue(PlaceholderColorProperty, value); }

    public static readonly BindableProperty PlaceholderColorProperty = BindableProperty.Create(
        nameof(PlaceholderColor), typeof(Color), typeof(Dropdown),
        defaultValue: Colors.Gray);

    public string Placeholder { get => (string)GetValue(PlaceholderProperty); set => SetValue(PlaceholderProperty, value); }

    public static readonly BindableProperty PlaceholderProperty = BindableProperty.Create(
        nameof(Placeholder), typeof(string), typeof(Dropdown));

    public object SelectedItem { get => GetValue(SelectedItemProperty); set => SetValue(SelectedItemProperty, value); }

    public static readonly BindableProperty SelectedItemProperty = BindableProperty.Create(
        nameof(SelectedItem), typeof(object), typeof(Dropdown));

    public IList ItemsSource { get => (IList)GetValue(ItemsSourceProperty); set => SetValue(ItemsSourceProperty, value); }
    internal Action<NotifyCollectionChangedEventArgs> ItemsSourceCollectionChangedCallback { get; set; }

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
    }

    private void ItemsSource_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        ItemsSourceCollectionChangedCallback(e);
    }
}
