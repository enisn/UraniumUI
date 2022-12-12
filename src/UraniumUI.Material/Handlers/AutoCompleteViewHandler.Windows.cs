#if WINDOWS
using Microsoft.Maui.Handlers;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UraniumUI.Material.Controls;

namespace UraniumUI.Material.Handlers;
public partial class AutoCompleteViewHandler : ViewHandler<AutoCompleteView, AutoSuggestBox>
{
    protected override AutoSuggestBox CreatePlatformView()
    {
        var textBox = new AutoSuggestBox();
        
        textBox.ItemsSource = VirtualView.ItemsSource;
        textBox.Text = VirtualView.Text;
        
        return textBox;
    }

    public static void MapText(AutoCompleteViewHandler handler, AutoCompleteView view)
    {
        handler.PlatformView.Text = view.Text;
    }

    public static void MapItemsSource(AutoCompleteViewHandler handler, AutoCompleteView view)
    {
        handler.PlatformView.ItemsSource = view.ItemsSource;
    }
}
#endif