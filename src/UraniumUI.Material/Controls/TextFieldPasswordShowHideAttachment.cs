using Microsoft.Maui.Controls.Shapes;
using UraniumUI.Extensions;
using UraniumUI.Pages;
using UraniumUI.Resources;
using UraniumUI.Views;
using Path = Microsoft.Maui.Controls.Shapes.Path;

namespace UraniumUI.Material.Controls;

public class TextFieldPasswordShowHideAttachment : StatefulContentView
{
    public TextField TextField { get; protected set; }

    public TextFieldPasswordShowHideAttachment()
    {
        VerticalOptions = LayoutOptions.Center;
        Padding = new Thickness(5, 0);
        Margin = new Thickness(0, 0, 5, 0);
        TappedCommand = new Command(SwitchPassword);
    }

    protected override void OnParentSet()
    {
        TextField = this.FindInParents<TextField>();
        if (TextField == null)
        {
            return;
        }

        UpdateIcon();
    }

    protected virtual void SwitchPassword(object parameter)
    {
        if (TextField is null)
        {
            UpdateIcon();
            return;
        }

        TextField.IsPassword = !TextField.IsPassword;
        UpdateIcon();
    }

    protected void UpdateIcon()
    {
        if (TextField is null)
        {
            Content = null;

            return;
        }

        Content = TextField.IsPassword ? GetPathFromData(UraniumShapes.Eye) : GetPathFromData(UraniumShapes.EyeSlash);
    }

    private Path GetPathFromData(Geometry data)
    {
        return new Path
        {
            Fill = ColorResource.GetColor("OnBackground", "OnBackgroundDark", Colors.DarkGray).WithAlpha(.5f),
            VerticalOptions = LayoutOptions.Center,
            Data = data,
        };
    }
}
