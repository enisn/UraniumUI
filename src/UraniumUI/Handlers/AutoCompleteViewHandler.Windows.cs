#if WINDOWS
using Microsoft.Maui.Handlers;
using Microsoft.UI.Xaml.Controls;
using UraniumUI.Controls;

namespace UraniumUI.Handlers;
public partial class AutoCompleteViewHandler : ViewHandler<IAutoCompleteView, AutoSuggestBox>
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
        platformView.BorderThickness = new Microsoft.UI.Xaml.Thickness(0);
        platformView.FocusVisualPrimaryThickness = new Microsoft.UI.Xaml.Thickness(0);
        platformView.FocusVisualSecondaryThickness = new Microsoft.UI.Xaml.Thickness(0);

        platformView.TextBoxStyle = null;

        platformView.TextChanged += PlatformView_TextChanged;
        platformView.GotFocus += PlatformView_GotFocus;
        platformView.KeyDown += TextBox_KeyDown;
        platformView.SuggestionChosen += PlatformView_SuggestionChosen;
    }

    protected override void DisconnectHandler(AutoSuggestBox platformView)
    {
        platformView.TextChanged -= PlatformView_TextChanged;
        platformView.GotFocus -= PlatformView_GotFocus;
        platformView.KeyDown -= TextBox_KeyDown;
        platformView.SuggestionChosen -= PlatformView_SuggestionChosen;
    }

    private void PlatformView_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
    {
        if (VirtualView.Text != sender.Text)
        {
            VirtualView.Text = sender.Text;
        }

        if (VirtualView.ItemsSource != null && !string.IsNullOrEmpty(sender.Text))
        {
            PlatformView.ItemsSource = VirtualView.ItemsSource.Where(x => x.Contains(sender.Text));
        }

        PlatformView.IsSuggestionListOpen = sender.Text.Length < VirtualView.Threshold;
    }

    private void PlatformView_GotFocus(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        PlatformView.IsSuggestionListOpen = PlatformView.Text.Length >= VirtualView.Threshold;
    }

    private void TextBox_KeyDown(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.Enter)
        {
            VirtualView.Completed();
        }
    }

    private void PlatformView_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
    {
        if (VirtualView.SelectedText != PlatformView.Text)
        {
            VirtualView.SelectedText = args.SelectedItem?.ToString();
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

    public static void MapThreshold(AutoCompleteViewHandler handler, AutoCompleteView view)
    {
        // Not supported, handled manually
    }
}
#endif