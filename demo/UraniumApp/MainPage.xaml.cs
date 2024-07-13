using UraniumUI.Pages;

namespace UraniumApp;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    private void Button_Clicked(object sender, EventArgs e)
    {
        dropdown.SelectedItem = null;
    }
}