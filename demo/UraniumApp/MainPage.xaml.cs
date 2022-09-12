using CommunityToolkit.Maui.Views;
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


        var result = await this.DisplayTextPromptAsync("Your Name", "What is your name?", placeholder: "Uvuvwevwevwe...Osas");

        await DisplayAlert("Result:", result, "OK");

        SemanticScreenReader.Announce(CounterBtn.Text);
	}
}