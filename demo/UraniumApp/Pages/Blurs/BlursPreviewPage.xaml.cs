namespace UraniumApp.Pages.Blurs;

public partial class BlursPreviewPage : ContentPage
{
	public BlursPreviewPage()
	{
		InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        collectionView.SelectedItem = collectionView.ItemsSource.Cast<object>().LastOrDefault();
    }
}