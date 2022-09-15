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

        SemanticScreenReader.Announce(CounterBtn.Text);
    }
}