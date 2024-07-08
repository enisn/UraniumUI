using ReactiveUI;
using UraniumUI.Dialogs;

namespace UraniumApp.Pages;

public partial class DialogsPage : ContentPage
{
    public IDialogService DialogService { get; private set; }

    private readonly IDialogService[] dialogServices;

    public DialogsPage(IEnumerable<IDialogService> dialogServices)
    {
        InitializeComponent();
        this.dialogServices = dialogServices.ToArray();
        this.DialogService = this.dialogServices.FirstOrDefault();

        implementationSelectionView.ItemsSource = dialogServices.Select(x => x.GetType().Name.Replace("DialogService", string.Empty)).ToList();
        implementationSelectionView.WhenAnyValue(x => x.SelectedIndex)
            .Subscribe(x =>
            {
                if (x >= 0)
                {
                    DialogService = this.dialogServices[x];
                }
            });

        implementationSelectionView.SelectedIndex = 0;
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
            , checkBoxResultListView.ItemsSource as IEnumerable<string>
            );

        checkBoxResultListView.ItemsSource = result;
    }

    private string lastTextInput = string.Empty;
    private async void AskTextPrompt(object sender, EventArgs e)
    {
        lastTextInput = await DialogService.DisplayTextPromptAsync(
            "Your Name",
            "What is your name?",
            placeholder: "Uvuvwevwevwe...Osas",
            initialValue: lastTextInput);

        labelTextPrompt.Text = "Result: " + lastTextInput;
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

    private async void DisplayProgress(object sender, EventArgs e)
    {
        using (await DialogService.DisplayProgressAsync("Loading", "Work in progress, please wait..."))
        {
            await Task.Delay((int)sliderForProgress.Value);
        }
    }

    private async void DisplayProgressCancellable(object sender, EventArgs e)
    {
        var tokenSource = new CancellationTokenSource();

        using (await DialogService.DisplayProgressCancellableAsync("Loading", "Work in progress, please wait...", tokenSource: tokenSource))
        {
            try
            {
                // Long operation...
                await Task.Delay((int)sliderForProgress.Value, tokenSource.Token);
            }
            catch (TaskCanceledException)
            {
            }
        }

        labelProgressCancellable.Text = tokenSource.Token.IsCancellationRequested ? "Cancelled" : "Completed!";
    }
}
