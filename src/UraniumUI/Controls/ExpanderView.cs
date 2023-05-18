using UraniumUI.Pages;
using UraniumUI.Resources;
using UraniumUI.Views;
using Path = Microsoft.Maui.Controls.Shapes.Path;

namespace UraniumUI.Controls;

[ContentProperty(nameof(ActualContent))]
public class ExpanderView : ContentView
{
    private const int ANIMATION_LENGTH = 125;
    protected StatefulContentView headerContainer = new StatefulContentView();
    protected Grid headerGrid = new Grid
    {
        Padding = 5,
        ColumnDefinitions =
        {
            new ColumnDefinition(GridLength.Star),
            new ColumnDefinition(GridLength.Auto),
        }
    };

    protected Path arrowIcon = new Path
    {
        Data = UraniumShapes.ArrowDown,
        VerticalOptions = LayoutOptions.Center,
    };

    protected ContentView actualContentContainer = new ContentView
    {
        AnchorY = 0,
        ScaleY = 0,
        IsVisible = false,
    };

    public ExpanderView()
    {
        headerContainer.Content = headerGrid;
        headerContainer.StyleClass = new[] { "Elevation1" };
        Grid.SetColumn(arrowIcon, 1);
        headerGrid.Children.Add(arrowIcon);

        arrowIcon.SetAppThemeColor(Path.FillProperty,
            ColorResource.GetColor("OnBackground", Colors.Gray),
            ColorResource.GetColor("OnBackgroundDark", Colors.Gray));

        actualContentContainer.IsVisible = false;
        Content = new VerticalStackLayout
        {
            Children = {
                headerContainer,
                actualContentContainer
            }
        };

        headerContainer.TappedCommand = new Command(() => IsExpanded = !IsExpanded);
    }

    public View Header
    {
        get => (View)headerGrid.Children.FirstOrDefault(x => Grid.GetColumn((View)x) == 0);
        set
        {
            headerGrid.Add(value, column: 0);
        }
    }

    public View ActualContent { get => actualContentContainer.Content; set => actualContentContainer.Content = value; }

    public bool IsExpanded { get => (bool)GetValue(IsExpandedProperty); set => SetValue(IsExpandedProperty, value); }

    public static readonly BindableProperty IsExpandedProperty = BindableProperty.Create(
        nameof(IsExpanded), typeof(bool), typeof(ExpanderView), propertyChanged: (bo, ov, nv) => (bo as ExpanderView).OnIsExpandedChanged());

    protected async void OnIsExpandedChanged()
    {
        if (IsExpanded)
        {
            actualContentContainer.IsVisible = true;

            await Task.WhenAll(
                actualContentContainer.ScaleYTo(1, ANIMATION_LENGTH, Easing.BounceIn),
                arrowIcon.RotateTo(180, ANIMATION_LENGTH)
                );

        }
        else
        {
            await Task.WhenAll(
                actualContentContainer.ScaleYTo(0, ANIMATION_LENGTH, Easing.BounceIn),
                arrowIcon.RotateTo(0, ANIMATION_LENGTH)
                );

            actualContentContainer.IsVisible = false;
        }
    }
}
