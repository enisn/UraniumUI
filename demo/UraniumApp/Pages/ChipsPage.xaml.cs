using UraniumUI.Dialogs.Mopups;
using UraniumUI.Material.Controls;

namespace UraniumApp.Pages;

public partial class ChipsPage : ContentPage
{
	public ChipsPage()
	{
		InitializeComponent();
	}

    private void Chip_Destroyed(object sender, EventArgs e)
    {
        if (sender is View view && view.Parent is Layout parentLayout)
        {
            parentLayout.Children.Remove(view);
        }
    }

    private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        await this.DisplayCheckBoxPromptAsync("Pick some of them", new[] { "Chip A", "Chip B", "Chip C" });
    }
}