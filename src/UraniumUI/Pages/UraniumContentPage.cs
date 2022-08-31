using Microsoft.Maui.Controls;
using UraniumUI.Pages.Views;

namespace UraniumUI.Pages;

[ContentProperty(nameof(PageContent))]
public class UraniumContentPage : ContentPage
{
    protected Grid _contentGrid;
    private View bottomSheet;
    public View PageContent { get => MainContent.Content; set => MainContent.Content = value; }

    public View BottomSheet { get => bottomSheet; set { bottomSheet = value; OnBottomSheetSet(); } }

    protected ContentView MainContent { get; } = new ContentView();

    public UraniumContentPage()
    {
        Content = _contentGrid = new Grid
        {
            Children =
            {
                MainContent
            }
        };
    }

    protected virtual void OnBottomSheetSet()
    {
        _contentGrid.Add(new BottomSheetView(BottomSheet, this), 0, 0);
    }
}
