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

    protected override void ConnectHandler(AutoSuggestBox platformView)
    {
        PlatformView.TextChanged += PlatformView_TextChanged;
    }

    protected override void DisconnectHandler(AutoSuggestBox platformView)
    {
        PlatformView.TextChanged -= PlatformView_TextChanged;
    }

    private void PlatformView_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
    {
        if (VirtualView.Text != sender.Text)
        {
            VirtualView.Text = sender.Text;
        }

        PlatformView.ItemsSource = VirtualView.ItemsSource.Where(x => x.Contains(sender.Text));
    }

    public static void MapText(AutoCompleteViewHandler handler, AutoCompleteView view)
    {
        if (view.Text != handler.PlatformView.Text)
        {
            handler.PlatformView.Text = view.Text;
        }
    }

    public static void MapItemsSource(AutoCompleteViewHandler handler, AutoCompleteView view)
    {
        handler.PlatformView.ItemsSource = view.ItemsSource;
    }
}
#endif