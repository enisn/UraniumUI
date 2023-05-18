using DotNurse.Injector.Attributes;
using InputKit.Shared.Controls;
using InputKit.Shared.Helpers;
using Microsoft.Maui.Controls.Shapes;
using Mopups.Pages;
using Mopups.Services;
using System.Reflection;
using UraniumUI.Pages;
using UraniumUI.Resources;
using Path = Microsoft.Maui.Controls.Shapes.Path;

namespace UraniumApp.Inputs.GeometryPicking;

[RegisterAs(typeof(IGeometryPicker))]
public class GeometryPicker : IGeometryPicker
{
    public Task<string> PickGeometryForAsync()
    {
        var tcs = new TaskCompletionSource<string>();

        var collectionView = new CollectionView
        {
            ItemsSource = 
                GetGeometriesFromType(typeof(UraniumShapes.Paths))
                    .Union(GetGeometriesFromType(typeof(PredefinedShapes.Paths))),
            SelectionMode = SelectionMode.Single,
            ItemsLayout = new GridItemsLayout(4, ItemsLayoutOrientation.Vertical),
            ItemTemplate = new DataTemplate(() =>
            {
                var pathItem = new Path { Fill = ColorResource.GetColor("OnBackground", "OnBackgroundDark", Colors.Black) };

                pathItem.SetBinding(Path.DataProperty, new Binding(nameof(PathNamePair.Geometry)));

                var nameLabel = new Label { HorizontalTextAlignment = TextAlignment.Center, FontSize = 10, TextColor = ColorResource.GetColor("OnBackground", "OnBackgroundDark", Colors.Black) };

                nameLabel.SetBinding(Label.TextProperty, new Binding(nameof(PathNamePair.Name)));

                return new VerticalStackLayout
                {
                    Children =
                    {
                        new ContentView { Content = pathItem, HeightRequest= 25, WidthRequest = 25, HorizontalOptions = LayoutOptions.Center },
                        nameLabel
                    }
                };
            })
        };

        var bottomStackLayout = new HorizontalStackLayout
        {
            Spacing = 10,
            HorizontalOptions = LayoutOptions.End,
            Children =
            {
                new Button
                {
                    Text = "Cancel",
                    StyleClass = new[] { "TextButton" },
                    Command = new Command(() =>
                    {
                        tcs.TrySetResult(null);
                        MopupService.Instance.PopAsync();
                    })
                },
                new Button
                {
                    Text = "OK",
                    StyleClass = new[] { "TextButton" },
                    Command = new Command(() =>
                    {
                        tcs.TrySetResult((collectionView.SelectedItem as PathNamePair)?.Path);
                        MopupService.Instance.PopAsync();
                    })
                }
            }
        };

        var rootStackLayout = new VerticalStackLayout
        {
            Padding = 10,
            VerticalOptions = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.Center,
            BackgroundColor = ColorResource.GetColor("Surface", "SurfaceDark", Colors.White),
            WidthRequest = 400,
            Children =
            {
                collectionView,
                new BoxView { StyleClass = new[] { "Divider" }},
                bottomStackLayout
            }
        };

        MopupService.Instance.PushAsync(new PopupPage
        {
            BackgroundColor = Colors.Black.WithAlpha(.6f),
            Content = rootStackLayout,
        });

        return tcs.Task;
    }

    private List<PathNamePair> GetGeometriesFromType(Type type)
    {
        return type.GetFields(BindingFlags.Static | BindingFlags.Public)
                .Select(fieldInfo => new PathNamePair
                {
                    Name = fieldInfo.Name,
                    Path = (string)fieldInfo.GetValue(null)
                }).ToList();
    }
}

internal class PathNamePair
{
    public string Name { get; set; }
    public string Path { get; set; }
    public Geometry Geometry => GeometryConverter.FromPath(Path);
}
