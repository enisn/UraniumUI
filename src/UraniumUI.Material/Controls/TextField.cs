using Microsoft.Maui.Controls.Shapes;
using Plainer.Maui.Controls;
using UraniumUI.Resources;

namespace UraniumUI.Material.Controls;
public partial class TextField : Grid
{
    internal const double FirstDash = 5;
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
        StrokeShape = new RoundRectangle
        {
            CornerRadius = 8
        }
    };

    public TextField()
    {
        this.Add(border);
        border.Content = mainEntry;
        this.Add(labelTitle);
        labelTitle.Scale = 1;

        mainEntry.Focused += MainEntry_Focused;
        mainEntry.Unfocused += MainEntry_Unfocused;

        mainEntry.SetBinding(Entry.TextProperty, new Binding(nameof(Text), source: this));
        labelTitle.SetBinding(Label.TextProperty, new Binding(nameof(Title), source: this));
    }

    private void MainEntry_Focused(object sender, FocusEventArgs e)
    {
        border.Stroke = AccentColor;
        labelTitle.TextColor = AccentColor;
        UpdateState();
    }

    private void MainEntry_Unfocused(object sender, FocusEventArgs e)
    {
        border.Stroke = BorderColor;
        labelTitle.TextColor = TextColor;

        UpdateState();
    }

    protected override async void OnSizeAllocated(double width, double height)
    {
        base.OnSizeAllocated(width, height);
        await Task.Delay(100);
        InitializeBorder();
    }

    protected virtual void UpdateState(bool animate = true)
    {
        if (border.StrokeDashArray == null || border.StrokeDashArray.Count == 0)
        {
            return;
        }

        if (!string.IsNullOrEmpty(Text) || mainEntry.IsFocused)
        {
            UpdateOffset(0.01, animate);
            labelTitle.TranslateTo(labelTitle.TranslationX, -25, 90);
            labelTitle.AnchorX = 0;
            labelTitle.ScaleTo(.8, 90);
        }
        else
        {
            var offsetToGo = border.StrokeDashArray[0] + border.StrokeDashArray[1] + FirstDash;
            UpdateOffset(offsetToGo, animate);
            labelTitle.TranslateTo(labelTitle.TranslationX, 0, 90);
            labelTitle.AnchorX = 0;
            labelTitle.ScaleTo(1, 90);
        }
    }

    private void InitializeBorder()
    {
        var perimeter = (this.Width + this.Height) * 2;
        var space = labelTitle.Width * .60;

#if WINDOWS
        if (space <= 0 || perimeter <= 0)
        {
            return;
        }

        border.Content = null;
        this.Remove(border);
        border = new Border
        {
            Padding = 0,
            Stroke = ColorResource.GetColor("OnBackground", "OnBackgroundDark", Colors.Gray),
            StrokeThickness = 2,
            StrokeDashOffset = 0,
            BackgroundColor = Colors.Transparent,
            StrokeShape = new RoundRectangle
            {
                CornerRadius = 8
            },
            Content = mainEntry
        };
#endif
        border.StrokeDashArray = new DoubleCollection { FirstDash, space, perimeter, 0 };

#if WINDOWS
        this.Add(border);
#endif

        UpdateState(animate: false);
        border.StrokeThickness = 2;
    }

    private void UpdateOffset(double value, bool animate = true)
    {
        if (animate)
        {
            border.StrokeDashOffset = value;
        }
        else
        {
            border.Animate("borderOffset", new Animation((d) =>
            {
                border.StrokeDashOffset = d;
            }, border.StrokeDashOffset, value, Easing.BounceIn), 2, 90);
        }
    }
}
