using InputKit.Shared.Helpers;
using Microsoft.Maui.Controls.Shapes;

namespace UraniumUI.Pages;
public static class UraniumShapes
{
    public static Geometry ExclamationCircle = GeometryConverter.FromPath(Paths.ExclamationCircle);
    public static Geometry X = GeometryConverter.FromPath(Paths.X);

    public static class Paths
	{
		public const string ExclamationCircle = "M 2.7835 16.2165 A 9.5 9.5 90 1 1 16.2165 2.7835 A 9.5 9.5 90 0 1 2.7835 16.2165 z M 8.55 4.75 v 5.7 h 1.9 V 4.75 H 8.55 z m 0 7.6 v 1.9 h 1.9 v -1.9 H 8.55 z";
        public const string X = "M17.705 7.705l-1.41-1.41L12 10.59 7.705 6.295l-1.41 1.41L10.59 12l-4.295 4.295 1.41 1.41L12 13.41l4.295 4.295 1.41-1.41L13.41 12l4.295-4.295z";
    }
}
