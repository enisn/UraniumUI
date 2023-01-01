#if WINDOWS
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UraniumUI.Material.Controls;

namespace UraniumUI.Material.Handlers;
public partial class AutoCompleteViewHandler : ViewHandler<IAutoCompleteView, AutoSuggestBox>
{
    protected override AutoSuggestBox CreatePlatformView()
    {
        var textBox = new AutoSuggestBox();

        textBox.ItemsSource = VirtualView.ItemsSource;
        textBox.Text = VirtualView.Text;
        
        var transparentBrush = new Microsoft.UI.Xaml.Media.SolidColorBrush(Windows.UI.Color.FromArgb(0, 0, 0, 0));
        textBox.BorderThickness = new Microsoft.UI.Xaml.Thickness(0);
        textBox.Background = transparentBrush;

        return textBox;
    }

    protected override void ConnectHandler(AutoSuggestBox platformView)
    {
        PlatformView.TextChanged += PlatformView_TextChanged;
        PlatformView.KeyDown += TextBox_KeyDown;
    }

    protected override void DisconnectHandler(AutoSuggestBox platformView)
    {
        PlatformView.TextChanged -= PlatformView_TextChanged;
        PlatformView.KeyDown -= TextBox_KeyDown;
    }

    private void PlatformView_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
    {
        if (VirtualView.Text != sender.Text)
        {
            VirtualView.Text = sender.Text;
        }

        if (VirtualView.ItemsSource != null)
        {
            PlatformView.ItemsSource = VirtualView.ItemsSource.Where(x => x.Contains(sender.Text));
        }
    }

    private void TextBox_KeyDown(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.Enter)
        {
            VirtualView.Completed();
        }
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