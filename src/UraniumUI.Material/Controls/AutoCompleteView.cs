using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UraniumUI.Material.Controls;
public class AutoCompleteView : View
{
    public string Text { get; set; }

    public string Placeholder { get; set; }
    
    public Color TextColor { get; set; } = Colors.DarkGray;

    public Color PlaceholderColor { get; set; } = Colors.Gray;

    public IList<string> ItemsSource { get => (IList<string>)GetValue(ItemsSourceProperty); set => SetValue(ItemsSourceProperty, value); }

    public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(nameof(ItemsSource),
            typeof(IList<string>),
            typeof(AutoCompleteView),
            null);

    public AutoCompleteView()
    {
        ItemsSource = new List<string>();
    }
}
