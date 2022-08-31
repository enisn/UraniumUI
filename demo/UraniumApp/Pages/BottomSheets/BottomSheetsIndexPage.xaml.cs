namespace UraniumApp.Pages.BottomSheets;

public partial class BottomSheetsIndexPage : ContentPage
{
    public BottomSheetsIndexPage()
    {
        InitializeComponent();
    }

    private void GoToRegular(object sender, EventArgs e)
    {
        Navigation.PushAsync(new RegularBottomSheetPage());
    }
    private void GoToExpanding(object sender, EventArgs e)
    {
        Navigation.PushAsync(new ExpandingBottomSheetPage());
    }
}
