using Microsoft.Maui.Controls.Shapes;
using Plainer.Maui.Controls;
using UraniumUI.Resources;

namespace UraniumUI.Material.Controls;
public partial class TextField : InputField
{
    protected EntryView mainEntry => Content as EntryView;

    public override View Content { get; set; } = new EntryView
    {
        Margin = new Thickness(5,1),
    };

    public TextField()
    {
        mainEntry.Focused += MainEntry_Focused;
        mainEntry.Unfocused += MainEntry_Unfocused;
        mainEntry.SetBinding(Entry.TextProperty, new Binding(nameof(Text), source: this));
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

    protected override void UpdateState(bool animate = true)
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
}
