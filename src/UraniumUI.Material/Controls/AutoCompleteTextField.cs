using Plainer.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UraniumUI.Material.Controls;
public class AutoCompleteTextField : InputField
{
    public AutoCompleteView AutoCompleteView => Content as AutoCompleteView;

    public override View Content { get; set; } = new AutoCompleteView
    {
        Margin = new Thickness(10, 0),
        BackgroundColor = Colors.Transparent,
        VerticalOptions = LayoutOptions.Center
    };

    public AutoCompleteTextField()
    {
        AutoCompleteView.SetBinding(AutoCompleteView.TextProperty, new Binding(nameof(Text), source: this));
        AutoCompleteView.SetBinding(AutoCompleteView.ItemsSourceProperty, new Binding(nameof(ItemsSource), source: this));
    }

    protected override object GetValueForValidator()
    {
        return AutoCompleteView.Text;
    }

    public override bool HasValue => !string.IsNullOrEmpty(Text);

    public string Text { get => (string)GetValue(TextProperty); set => SetValue(TextProperty, value); }

    public static readonly BindableProperty TextProperty = BindableProperty.Create(
        nameof(Text),
        typeof(string),
        typeof(AutoCompleteView),
        string.Empty,
        BindingMode.TwoWay,
        propertyChanged: (bindable, oldValue, newValue)=> (bindable as AutoCompleteTextField).OnPropertyChanged(nameof(Text)));

    public IList<string> ItemsSource { get => (IList<string>)GetValue(ItemsSourceProperty); set => SetValue(ItemsSourceProperty, value); }

    public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(nameof(ItemsSource),
            typeof(IList<string>),
            typeof(AutoCompleteView),
            null,
        propertyChanged: (bindable, oldValue, newValue)=> (bindable as AutoCompleteTextField).OnPropertyChanged(nameof(ItemsSource)));
}
