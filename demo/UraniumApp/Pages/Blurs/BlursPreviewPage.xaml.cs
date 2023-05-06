namespace UraniumApp.Pages.Blurs;

public partial class BlursPreviewPage : ContentPage
{
	public BlursPreviewPage()
	{
		InitializeComponent();

		img.Source = "https://images.unsplash.com/photo-1470058869958-2a77ade41c02?ixlib=rb-4.0.3&ixid=MnwxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8&auto=format&fit=crop&w=1170&q=80";
    }

    private void rb1_SelectedItemChanged(object sender, EventArgs e)
    {
        blur1.Mode = (UraniumUI.Blurs.BlurMode)rb1.SelectedIndex;
    }
}