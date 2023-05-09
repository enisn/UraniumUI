using UraniumUI.Extensions;
using UraniumUI.Pages;
using UraniumUI.Resources;
using UraniumUI.Views;
using Path = Microsoft.Maui.Controls.Shapes.Path;

namespace UraniumUI.Material.Controls;

public class TextFieldPasswordShowHideAttachment : StatefulContentView
{
    public TextField TextField { get; protected set; }

    protected Path iconPath = new Path
    {
        Fill = ColorResource.GetColor("OnBackground", "OnBackgroundDark", Colors.DarkGray).WithAlpha(.5f),
    };

    public TextFieldPasswordShowHideAttachment()
    {
        VerticalOptions = LayoutOptions.Center;
        this.Content = iconPath;
        this.Padding = 10;
        this.PressedCommand = new Command(SwitchPassword);
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
            iconPath.Data = null;

            return;
        }

        iconPath.Data = TextField.IsPassword ? UraniumShapes.Eye : UraniumShapes.EyeSlash;
    }
}
