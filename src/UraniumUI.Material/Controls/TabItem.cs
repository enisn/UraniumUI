using System.Windows.Input;

namespace UraniumUI.Material.Controls;

[ContentProperty(nameof(Content))]
public class TabItem : UraniumBindableObject
{
    public string Title { get => (string)GetValue(TitleProperty); set => SetValue(TitleProperty, value); }

    public static readonly BindableProperty TitleProperty = BindableProperty.Create(nameof(Title), typeof(string), typeof(TabItem));

    public DataTemplate HeaderTemplate { get => (DataTemplate)GetValue(HeaderTemplateProperty); set => SetValue(HeaderTemplateProperty, value); }

    public static readonly BindableProperty HeaderTemplateProperty = 
        BindableProperty.Create(
            nameof(HeaderTemplate),typeof(DataTemplate), typeof(TabItem), 
            defaultBindingMode: BindingMode.TwoWay, 
            propertyChanged: (bindable, oldValue, newValue) => (bindable as TabItem).TabView.RenderHeaders());

    public DataTemplate ContentTemplate { get => (DataTemplate)GetValue(ContentTemplateProperty); set => SetValue(ContentTemplateProperty, value); }

    public static readonly BindableProperty ContentTemplateProperty = BindableProperty.Create(
        nameof(ContentTemplate), typeof(DataTemplate), typeof(TabItem),
        defaultBindingMode: BindingMode.TwoWay,
        propertyChanged: (bindable, oldValue, newValue) => (bindable as TabItem).TabView.InvalidateTabItemContents());

    public View Content { get; set; }
    public View Header { get; set; }
    public object Data { get; set; }
    public TabView TabView { get; internal set; }
    public bool IsSelected { get => TabView.SelectedTab == this || (TabView.CurrentItem != null && TabView.CurrentItem == Data); }

    public bool IsVisible { get => (bool)GetValue(IsVisibleProperty); set => SetValue(IsVisibleProperty, value); }

    public static readonly BindableProperty IsVisibleProperty = BindableProperty.Create(nameof(IsVisible), typeof(bool), typeof(TabItem), true,
        propertyChanged: (bindable, oldValue, newValue) =>
        {
            if (bindable is TabItem tabItem && tabItem.Header is not null)
            {
                tabItem.Header.IsVisible = (bool)newValue;
            }
        });

    public ICommand Command { get; private set; }

    public TabItem()
    {
        Command = new Command(ChangedCommand);
    }

    private void ChangedCommand(object obj)
    {
        if (this.Data != null)
        {
            TabView.CurrentItem = this.Data;
        }
        else
        {
            TabView.SelectedTab = this;
        }
    }

    protected internal void NotifyIsSelectedChanged()
    {
        OnPropertyChanged(nameof(IsSelected));
    }
}