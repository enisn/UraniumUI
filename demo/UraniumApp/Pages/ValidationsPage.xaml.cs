namespace UraniumApp.Pages;

public partial class ValidationsPage : ContentPage
{
    public ValidationsPage()
    {
        InitializeComponent();
    }

    private void ToolbarItem_Clicked(object sender, EventArgs e)
    {
        Navigation.PushAsync(new ValidationsPage());
    }
}