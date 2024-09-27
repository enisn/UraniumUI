namespace UraniumApp.Pages.Blurs;

public partial class BlursPreviewPage : ContentPage
{
	public BlursPreviewPage()
	{
		try
		{
			InitializeComponent();
		}
		catch (Exception ex)
		{
			App.Current.MainPage.DisplayAlert("Error", ex.ToString(), "OK");
        }
    }
}