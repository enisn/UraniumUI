using DotNurse.Injector.Attributes;
using Mopups.Services;

namespace UraniumApp.Inputs.ColorPicking;

[RegisterAs(typeof(IColorPicker))]
class ColorPicker : IColorPicker
{
    public async Task PickCollorForAsync(object context, string bindingPath)
    {
        await MopupService.Instance.PushAsync(new ColorEditPopupPage(context, bindingPath));
    }
}