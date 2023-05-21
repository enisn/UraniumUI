using Microsoft.Maui.Controls.Shapes;

namespace UraniumApp.Inputs.GeometryPicking;

public interface IGeometryPicker
{
    Task<string> PickGeometryForAsync();
}
