using UraniumUI.Pages;

namespace UraniumApp.Pages.BottomSheets;

public partial class ExpandingBottomSheetPage : UraniumContentPage
{
    public ExpandingBottomSheetPage()
    {
        InitializeComponent();
    }

    private void OnCounterClicked(object sender, EventArgs e)
    {
        bottomPage.IsPresented = !bottomPage.IsPresented;
    }
}
