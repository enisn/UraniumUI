namespace UraniumApp.Inputs.ColorPicking;

public interface IColorPicker
{
    Task PickCollorForAsync(object context, string bindingPath);
}
