using Microsoft.Maui.Controls.Platform;
using Microsoft.Maui.Controls.Shapes;
using Plainer.Maui.Controls;
#if IOS || MACCATALYST
using UIKit;
#endif
using UraniumUI.Resources;

namespace UraniumUI.Material.Controls;
public partial class TextField : Grid
{
    Entry mainEntry = new EntryView
    {
        Margin = 1,
    };
    Label labelTitle = new Label()
    {
        Text = "Title",
        HorizontalOptions = LayoutOptions.Start,
        InputTransparent = true,
        Margin = 15,
        TextColor = ColorResource.GetColor("OnBackground", "OnBackgroundDark", Colors.Gray)
    };

    public Border border = new Border
    {
        Padding = 0,
        Stroke = ColorResource.GetColor("OnBackground", "OnBackgroundDark", Colors.Gray),
        StrokeThickness = 2,
        StrokeDashOffset = 0,
        BackgroundColor = Colors.Transparent,
        InputTransparent = true,
        StrokeShape = new RoundRectangle
        {
            CornerRadius = 8
        }
    };

    public TextField()
    {
        //this.Add(border);
        this.Add(mainEntry);
        this.Add(labelTitle);
        labelTitle.Scale = 1;

        mainEntry.Focused += MainEntry_Focused;
        mainEntry.Unfocused += MainEntry_Unfocused;
    }

    private void MainEntry_Focused(object sender, FocusEventArgs e)
    {
        border.Stroke = ColorResource.GetColor("Primary", "PrimaryDark", Colors.Purple);
        //AnimateBorderOffsetTo(0);
        border.StrokeDashOffset = 0.1;
        
        labelTitle.TranslateTo(labelTitle.TranslationX, labelTitle.TranslationY - 25, 90);
        labelTitle.TextColor = ColorResource.GetColor("Primary", "PrimaryDark", Colors.Purple);
        labelTitle.ScaleTo(.8, 90);
    }

    private void MainEntry_Unfocused(object sender, FocusEventArgs e)
    {
        border.Stroke = ColorResource.GetColor("OnBackground", "OnBackgroundDark", Colors.Gray);
        border.StrokeDashOffset = border.StrokeDashArray[0] + border.StrokeDashArray[1] + 5;
        
        labelTitle.TranslateTo(labelTitle.TranslationX, labelTitle.TranslationY + 25, 90);
        labelTitle.TextColor = ColorResource.GetColor("OnBackground", "OnBackgroundDark", Colors.Gray);
        labelTitle.ScaleTo(1, 90);
    }

    protected virtual void UpdateState()
    {
        
    }
    
    protected override async void OnSizeAllocated(double width, double height)
    {
        await Task.Delay(100);

        var space = (labelTitle.Width / 2) * 1.5;
        var perimeter = (width + height) * 2;

        if (border != null)
        {
            this.Remove(border);
        }
        border.StrokeDashArray = new double[] { 5, space, perimeter, 0};

        this.Add(border);
        border.StrokeThickness = 2;

        base.OnSizeAllocated(width, height);
    }

    private async void AnimateBorderOffsetTo(double to)
    {
        if (to > border.StrokeDashOffset)
        {
            for (var i = border.StrokeDashOffset; i < to ; i++)
            {
                await Task.Delay(10);
                border.StrokeDashOffset = i;
            }
        }
        else
        {
            for (var i = border.StrokeDashOffset; i >= to; i--)
            {
                await Task.Delay(10);
                border.StrokeDashOffset = i;
            }
        }
    }
}
