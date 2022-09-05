namespace UraniumApp.Pages.DataGrids;

public partial class DataGridsIndexPage : ContentPage
{
    public DataGridsIndexPage()
    {
        InitializeComponent();
    }

    private void GoToSimpleDataGrid(object sender, EventArgs e)
    {
        Navigation.PushAsync(new SimpleDataGridPage());
    }

    private void GoToCustomDataGrid(object sender, EventArgs e)
    {
        Navigation.PushAsync(new CustomDataGridPage());
    }
}
