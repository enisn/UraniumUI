using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Animations;
using UraniumUI.Extensions;
using UraniumUI.Pages;

namespace UraniumApp;

public partial class MainPage : UraniumContentPage
{
    int count = 0;

    public MainPage()
    {
        InitializeComponent();
    }

    private async void OnCounterClicked(object sender, EventArgs e)
    {
        count++;

        if (count == 1)
            CounterBtn.Text = $"Clicked {count} time";
        else
            CounterBtn.Text = $"Clicked {count} times";


        var fullWidth = (border.Width + border.Height) * 3 / 7;

        var rateOfLabel = title.Width / border.Width;

        var finalValue = fullWidth * rateOfLabel;


        //border.StrokeDashArray = new double[] { 0, finalValue, 500 };

        await DisplayAlert("Calculated",
            $"fullWidth: {fullWidth}\nRateOfLabel: {rateOfLabel}\nFinalValue: {finalValue}",
            "ok");


        //      var result = await this.DisplayTextPromptAsync("Your Name", "What is your name?", placeholder: "Uvuvwevwevwe...Osas");

        //await DisplayAlert("Result:", result, "OK");

        SemanticScreenReader.Announce(CounterBtn.Text);
    }

    private void offsetChanged(object sender, ValueChangedEventArgs e)
    {
        //borderInner.StrokeDashOffset = e.NewValue;
        textField.border.StrokeDashOffset = e.NewValue;
    }

    private void entry_Focused(object sender, FocusEventArgs e)
    {


        //title.TranslateTo(title.TranslationX, title.TranslationY - 25);

        //border.Stroke = Brush.Transparent;
        //borderInner.Stroke = Colors.White;
    }

    private void entry_Unfocused(object sender, FocusEventArgs e)
    {

        //title.TranslateTo(title.TranslationX, title.TranslationY + 25);
        //border.Stroke = Colors.White;
        //borderInner.Stroke = Brush.Transparent;
    }
}