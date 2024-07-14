namespace UraniumApp.Pages;

public partial class DropdownFieldPage : ContentPage
{
	public DropdownFieldPage()
	{
		InitializeComponent();
	}

    private void Button_Clicked(object sender, EventArgs e)
    {
        dropdown.SelectedItem = null;
    }
}