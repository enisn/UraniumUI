using Microsoft.Maui.Controls.Shapes;
using UraniumUI.Resources;

namespace UraniumUI.Material.Controls;
public partial class TextField : Grid
{
    Entry mainEntry = new Entry
    {
        Margin = 1,
    };
    Label labelTitle = new Label()
    {
        Text = "Title",
        HorizontalOptions = LayoutOptions.Start,
        InputTransparent = true,
        Margin = 15,
    };

    Border borderUnfocused = new Border
    {
        Padding = 0,
        //Stroke = ColorResource.GetColor("Surface", "SurfaceDark", Colors.Gray),
        StrokeThickness = 2,
        InputTransparent = true,
        StrokeShape = new RoundRectangle
        {
            CornerRadius = 8
        }
    };

    Border borderFocused = new Border
    {
        Stroke = ColorResource.GetColor("Primary", "PrimaryDark", Colors.Purple),
        InputTransparent = true,
        StrokeThickness = 2.5,
        IsVisible = false,
        StrokeShape = new RoundRectangle
        {
            CornerRadius = 8
        }
    };

    public TextField()
    {
        this.Add(borderUnfocused);
        //this.Add(borderFocused);
        this.Add(mainEntry);
        this.Add(labelTitle);
        labelTitle.Scale = 1.2;
        //borderFocused.Content = mainEntry;

        mainEntry.Focused += MainEntry_Focused;
        mainEntry.Unfocused += MainEntry_Unfocused;
    }

    private void MainEntry_Focused(object sender, FocusEventArgs e)
    {
        borderFocused.IsVisible = true;
        borderUnfocused.IsVisible = false;

        labelTitle.TranslateTo(labelTitle.TranslationX, labelTitle.TranslationY - 25, 90);
        labelTitle.TextColor = ColorResource.GetColor("Primary", "PrimaryDark", Colors.Purple);
        labelTitle.ScaleTo(1, 90);
    }

    private void MainEntry_Unfocused(object sender, FocusEventArgs e)
    {
        borderFocused.IsVisible = false;
        borderUnfocused.IsVisible = true;

        labelTitle.TranslateTo(labelTitle.TranslationX, labelTitle.TranslationY + 25, 90);
        labelTitle.TextColor = ColorResource.GetColor("OnBackground", "OnBackgroundDark", Colors.Gray);
        labelTitle.ScaleTo(1.2, 90);
    }

    protected override SizeRequest OnMeasure(double widthConstraint, double heightConstraint)
    {
        //var space = (labelTitle.Width / 2) * 1.13;
        //var perimeter = (widthConstraint + height) * 2;
        return base.OnMeasure(widthConstraint, heightConstraint);
    }

    protected override async void OnSizeAllocated(double width, double height)
    {
        await Task.Delay(100);

        var space = (labelTitle.Width / 2) * 1.13;
        var perimeter = (width + height) * 2;

        if (borderFocused != null)
        {
            this.Remove(borderFocused);
        }

        borderFocused.StrokeDashArray = new double[] { 5, space, perimeter };
        //borderFocused.StrokeDashArray.Add(5);
        //borderFocused.StrokeDashArray.Add(space);
        //borderFocused.StrokeDashArray.Add(perimeter);

        this.Add(borderFocused);

        base.OnSizeAllocated(width, height);
    }
}
