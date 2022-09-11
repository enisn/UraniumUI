using System.Net.WebSockets;
using UraniumUI.Extensions;

namespace UraniumApp.Pages;

public partial class DialogsPage : ContentPage
{
    public DialogsPage()
    {
        InitializeComponent();
    }

    private async void AskRadioButtons(object sender, EventArgs e)
    {
        var count = Convert.ToInt32(sliderForRadioButtons.Value);

        var options = GenerateOptions(count);

        var result = await this.DisplayRadioButtonPromptAsync(
            "Pick one of them below", 
            options,
            labelSelected.Text ?? options.FirstOrDefault());

        labelSelected.Text = "Selected: " + result;
    }

    private async void AskCheckBoxes(object sender, EventArgs e)
    {
        var count = Convert.ToInt32(sliderForCheckBoxes.Value);

        var options = GenerateOptions(count);

        var result = await this.DisplayCheckBoxPromptAsync(
            "Pick some of them below",
            options
            ,checkBoxResultListView.ItemsSource as IEnumerable<string>
            );

        checkBoxResultListView.ItemsSource = result;
    }

    private async void AskTextPrompt(object sender, EventArgs e)
    {
        var result = await this.DisplayTextPromptAsync("Your Name", "What is your name?", placeholder: "Uvuvwevwevwe...Osas");

        await DisplayAlert("Result:", result, "OK");
    }

    private static IEnumerable<string> GenerateOptions(int count)
    {
        for (int i = 1; i <= count; i++)
        {
            yield return "Option " + i;
        }
    }
}
