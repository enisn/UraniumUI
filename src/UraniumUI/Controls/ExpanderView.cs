using UraniumUI.Extensions;
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
        StyleClass = new[] { "ExpanderView.Header" },
        ColumnDefinitions =
        {
            new ColumnDefinition(GridLength.Star),
            new ColumnDefinition(GridLength.Auto),
        }
    };

    protected Path arrowIcon = new Path
    {
        StyleClass = new[] { "ExpanderView.Arrow" },
        Data = UraniumShapes.ArrowDown,
        VerticalOptions = LayoutOptions.Center,
        HorizontalOptions = LayoutOptions.Center,
    };

    protected ContentView actualContentContainer = new ContentView
    {
        StyleClass = new[] { "ExpanderView.Content" },
        AnchorY = 0,
        ScaleY = 0,
        IsVisible = false,
    };

    public ExpanderView()
    {
        headerContainer.Content = headerGrid;
        headerGrid.Add(new ContentView
        {
            Padding = new Thickness(10, 0),
            Content = arrowIcon
        }, column: 1) ;

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
                arrowIcon.RotateToSafely(180, ANIMATION_LENGTH)
                );

        }
        else
        {
            await Task.WhenAll(
                actualContentContainer.ScaleYTo(0, ANIMATION_LENGTH, Easing.BounceIn),
                arrowIcon.RotateToSafely(0, ANIMATION_LENGTH)
                );

            actualContentContainer.IsVisible = false;
        }
    }
}
