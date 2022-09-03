namespace UraniumApp.Pages.Backdrops;

public partial class BackdropsIndexPage : ContentPage
{
    public BackdropsIndexPage()
    {
        InitializeComponent();
    }

    private void GoToSimpleBackdrop(object sender, EventArgs e)
    {
        Navigation.PushAsync(new SimpleBackdropPage());
    }
}
