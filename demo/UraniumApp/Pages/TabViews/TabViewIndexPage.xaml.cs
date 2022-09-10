namespace UraniumApp.Pages.TabViews;

public partial class TabViewIndexPage : ContentPage
{
    public TabViewIndexPage()
    {
        InitializeComponent();
    }

    private void GoToSimpleTabView(object sender, EventArgs e)
    {
        Navigation.PushAsync(new TabViewPage());
    }

    private void GoToCustomTabItemTabview(object sender, EventArgs e)
    {
        Navigation.PushAsync(new CustomTabItemTabView());
    }
}
