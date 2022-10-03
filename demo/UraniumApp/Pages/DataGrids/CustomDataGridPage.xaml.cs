namespace UraniumApp.Pages.DataGrids;

public partial class CustomDataGridPage : ContentPage
{
    public CustomDataGridPage()
    {
        InitializeComponent();
    }

    private void Button_Clicked(object sender, EventArgs e)
    {
        if (sender is Button button)
        {
            Console.WriteLine(button.BindingContext);
        }
    }
}
