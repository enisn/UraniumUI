using UraniumApp.Pages.TreeViews;

namespace UraniumApp.Pages;

public partial class TreeViewIndexPage : ContentPage
{
    public TreeViewIndexPage()
    {
        InitializeComponent();
    }

    private void GoToRegularTreeViews(object sender, EventArgs e)
    {
        Navigation.PushAsync(new TreeViewPage());
    }

    private void GoToTreeViewFileSystem(object sender, EventArgs e)
    {
        Navigation.PushAsync(new TreeViewFileSystemPage());
    }
}
