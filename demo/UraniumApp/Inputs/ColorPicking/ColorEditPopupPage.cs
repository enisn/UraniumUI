using Mopups.Pages;
using Mopups.Services;
using UraniumUI.Resources;

namespace UraniumApp.Inputs.ColorPicking;

public class ColorEditPopupPage : PopupPage
{
    public ColorEditPopupPage(object source, string bindingPath)
    {
        this.BindingContext = source;
        this.BackgroundColor = Colors.Black.WithAlpha(0.6f);
        this.WidthRequest = 500;
        var isVertical = Microsoft.Maui.Devices.DeviceInfo.Idiom != DeviceIdiom.Desktop;

        var grid = CreateGrid(isVertical);

        var boxSize = DeviceInfo.Idiom == DeviceIdiom.Phone ? 100 : 200;

        var colorPreviewBox = new BoxView
        {
            HeightRequest = boxSize,
            WidthRequest = boxSize,
            HorizontalOptions = LayoutOptions.Center,
            StyleClass = new[] { "Elevation1" }
        };

        colorPreviewBox.SetBinding(BoxView.ColorProperty, bindingPath);

        var colorEditor = new ColorEditor
        {
            HorizontalOptions = LayoutOptions.Center,
            Placeholder = "#ffffff",
            PlaceholderColor = Colors.Grey.WithAlpha(.3f)
        };

        colorEditor.SetBinding(ColorEditor.ColorProperty, bindingPath);

        grid.Add(new VerticalStackLayout
        {
            Padding = 20,
            Children =
            {
                colorPreviewBox,
                new Border { Content = colorEditor }
            }
        });

        var sliderColorPicker = new SliderColorPicker()
        {
            VerticalOptions = LayoutOptions.Center,
        };

        sliderColorPicker.SetBinding(SliderColorPicker.ColorProperty, bindingPath);

        if (isVertical)
        {
            grid.Add(sliderColorPicker, row: 1);
        }
        else
        {
            grid.Add(sliderColorPicker, column: 1);
        }

        var rootStackLayout = new VerticalStackLayout
        {
            Padding = 10,
            Children =
            {
                new Label
                {
                    Text = bindingPath,
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
        rootStackLayout.WidthRequest = DeviceInfo.Idiom == DeviceIdiom.Phone ? App.Current.MainPage.Width : 560;
        rootStackLayout.SetAppThemeColor(
            StackLayout.BackgroundColorProperty,
            ColorResource.GetColor("Surface"),
            ColorResource.GetColor("SurfaceDark"));

        this.Content = rootStackLayout;
    }

    private static Grid CreateGrid(bool isVertical)
    {
        if (isVertical)
        {
            return new Grid
            {
                RowDefinitions =
                {
                    new RowDefinition(GridLength.Auto),
                    new RowDefinition(GridLength.Auto),
                }
            };
        }
        else
        {
            return new Grid
            {
                ColumnDefinitions =
                {
                    new ColumnDefinition(GridLength.Auto),
                    new ColumnDefinition(GridLength.Star),
                },
            };
        }
    }
}
