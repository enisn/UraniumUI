using Microsoft.Maui.Controls.Shapes;
using UraniumUI.Resources;

namespace UraniumApp;

[ContentProperty(nameof(ShowcaseView))]
public class ViewShowcaseView : Border
{
    protected Grid rootGrid = new Grid();
    protected ContentView contentView = new ContentView { HorizontalOptions = LayoutOptions.Start, Padding = 10 };
    protected ContentView sidePanelContentView = new ContentView();
    protected ContentView bottomContentView = new ContentView();
    public ViewShowcaseView()
    {
        this.SetAppThemeColor(
            BackgroundColorProperty,
            ColorResource.GetColor("Surface", Colors.Gray),
            ColorResource.GetColor("SurfaceDark", Colors.DarkGray));

        sidePanelContentView.SetAppThemeColor(ContentView.BackgroundProperty, Color.FromArgb("#25ffffff"), Color.FromArgb("#25000000"));

        StrokeShape = new RoundRectangle
        {
            CornerRadius = 8
        };

        if (DeviceInfo.Idiom == DeviceIdiom.Phone)
        {
            rootGrid.RowDefinitions = (RowDefinitionCollection)new RowDefinitionCollectionTypeConverter().ConvertFrom("Auto,Auto");
            rootGrid.Add(contentView);
            rootGrid.Add(sidePanelContentView, row: 1);
        }
        else
        {
            rootGrid.ColumnDefinitions = (ColumnDefinitionCollection)new ColumnDefinitionCollectionTypeConverter().ConvertFrom("7*,3*");

            rootGrid.Add(contentView);
            rootGrid.Add(sidePanelContentView, column: 1);
        }

        Content = new VerticalStackLayout
        {
            Children =
            {
                rootGrid,
                bottomContentView
            }
        };
    }

    public View ShowcaseView { get => contentView.Content; set => contentView.Content = value; }

    public View SidePanel { get => sidePanelContentView.Content; set => sidePanelContentView.Content = value; }

    public View BottomView { get => bottomContentView.Content; set => bottomContentView.Content = value; }
}
