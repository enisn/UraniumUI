using System.Net.WebSockets;
using UraniumUI.Dialogs;
using UraniumUI.Extensions;

namespace UraniumApp.Pages;

public partial class DialogsPage : ContentPage
{
    public IDialogService DialogService { get; }

    public DialogsPage(IDialogService dialogService)
    {
        InitializeComponent();
        DialogService = dialogService;
    }

    private async void AskRadioButtons(object sender, EventArgs e)
    {
        var count = Convert.ToInt32(sliderForRadioButtons.Value);

        var options = GenerateOptions(count);

        var result = await DialogService.DisplayRadioButtonPromptAsync(
            "Pick one of them below",
            options,
            labelSelected.Text ?? options.FirstOrDefault());

        labelSelected.Text = result;
    }

    private async void AskCheckBoxes(object sender, EventArgs e)
    {
        var count = Convert.ToInt32(sliderForCheckBoxes.Value);

        var options = GenerateOptions(count);

        var result = await DialogService.DisplayCheckBoxPromptAsync(
            "Pick some of them below",
            options
            ,checkBoxResultListView.ItemsSource as IEnumerable<string>
            );

        checkBoxResultListView.ItemsSource = result;
    }

    private async void AskTextPrompt(object sender, EventArgs e)
    {
        var result = await DialogService.DisplayTextPromptAsync("Your Name", "What is your name?", placeholder: "Uvuvwevwevwe...Osas");

        labelTextPrompt.Text = "Result: " + result;
    }

    private async void AskConfirmation(object sender, EventArgs e)
    {
        var result = await DialogService.ConfirmAsync("Confirmation", "Are you sure?", "Yes", "No");

        labelConfirmation.Text = "Result: " + result;
    }

    private static IEnumerable<string> GenerateOptions(int count)
    {
        for (int i = 1; i <= count; i++)
        {
            yield return "Option " + i;
        }
    }
}
