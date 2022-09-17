using InputKit.Shared.Helpers;
using Microsoft.Maui.Controls.Shapes;

namespace UraniumUI.Pages;
public static class UraniumShapes
{
    public static Geometry ExclamationCircle = GeometryConverter.FromPath(Paths.ExclamationCircle);

    public static class Paths
	{
		public const string ExclamationCircle = "M 2.7835 16.2165 A 9.5 9.5 90 1 1 16.2165 2.7835 A 9.5 9.5 90 0 1 2.7835 16.2165 z M 8.55 4.75 v 5.7 h 1.9 V 4.75 H 8.55 z m 0 7.6 v 1.9 h 1.9 v -1.9 H 8.55 z";

    }
}
