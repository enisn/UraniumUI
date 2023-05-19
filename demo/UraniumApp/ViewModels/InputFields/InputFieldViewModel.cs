using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Windows.Input;
using UraniumApp.Inputs.ColorPicking;
using UraniumUI.Resources;

namespace UraniumApp.ViewModels.InputFields;

public class InputFieldViewModel : ReactiveObject
{
    protected IColorPicker ColorPicker { get; }

    protected Color defaultAccentColor = ColorResource.GetColor("Primary", "PrimaryDark", Colors.Purple);
    [Reactive] public Color AccentColor { get; set; }

    protected Color defaultBorderColor = ColorResource.GetColor("OnBackground", "OnBackgroundDark", Colors.Gray);
    [Reactive] public Color BorderColor { get; set; }

    public ICommand PickAccentColorCommand { get; }
    public ICommand PickBorderColorCommand { get; }

    public InputFieldViewModel(IColorPicker colorPicker)
    {
        ColorPicker = colorPicker;

        AccentColor = defaultAccentColor;
        BorderColor = defaultBorderColor;

        PickAccentColorCommand = new Command(async _ =>
            await ColorPicker.PickCollorForAsync(this, nameof(AccentColor)));

        PickBorderColorCommand = new Command(async _ =>
            await ColorPicker.PickCollorForAsync(this, nameof(BorderColor)));
    }
}
