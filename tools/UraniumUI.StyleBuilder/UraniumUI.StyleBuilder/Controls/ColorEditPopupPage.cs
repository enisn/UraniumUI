using Mopups.Pages;
using Mopups.Services;
using UraniumUI.Resources;

namespace UraniumUI.StyleBuilder.Controls;
public class ColorEditPopupPage : PopupPage
{
    public ColorEditPopupPage(object source, string bindingPath)
    {
        this.BindingContext = source;
        this.BackgroundColor = Colors.Black.WithAlpha(0.6f);
        this.WidthRequest = 500;
        var grid = new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition(GridLength.Auto),
                new ColumnDefinition(GridLength.Star),
            },
        };

        var colorPreviewBox = new BoxView
        {
            HeightRequest = 200,
            WidthRequest = 200,
            HorizontalOptions = LayoutOptions.Center,
            StyleClass = new[] { "Elevation1" }
        };

        colorPreviewBox.SetBinding(BoxView.ColorProperty, bindingPath);

        var colorEditor = new ColorEditor
        {
            HorizontalOptions = LayoutOptions.Center,
        };

        colorEditor.SetBinding(ColorEditor.ColorProperty, bindingPath);

        grid.Add(new VerticalStackLayout
        {
            Padding = 20,
            Children =
            {
                colorPreviewBox,
                colorEditor
            }
        });

        var sliderColorPicker = new SliderColorPicker()
        {
            VerticalOptions = LayoutOptions.Center,
        };

        sliderColorPicker.SetBinding(SliderColorPicker.ColorProperty, bindingPath);

        grid.Add(sliderColorPicker, column: 1);

        var rootStackLayout = new VerticalStackLayout
        {
            Padding = 10,
            Children =
            {
                new Label
                {
                    Text = bindingPath.Split('.').Last(),
                    HorizontalOptions = LayoutOptions.Center,
                },
                new BoxView
                {
                    StyleClass = new []{ "Divider" }
                },
                grid,
                new Button
                {
                    StyleClass = new []{ "TextButton" },
                    Text = "OK",
                    Command = new Command(() =>
                    {
                        MopupService.Instance.PopAsync();
                    })
                }
            }
        };

        rootStackLayout.HorizontalOptions = LayoutOptions.Center;
        rootStackLayout.VerticalOptions = LayoutOptions.Center;
        rootStackLayout.WidthRequest = 560;
        rootStackLayout.SetAppThemeColor(
            StackLayout.BackgroundColorProperty,
            ColorResource.GetColor("Surface"),
            ColorResource.GetColor("SurfaceDark"));

        this.Content = rootStackLayout;
    }
}
