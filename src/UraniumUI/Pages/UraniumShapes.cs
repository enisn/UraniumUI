using InputKit.Shared.Helpers;
using Microsoft.Maui.Controls.Shapes;

namespace UraniumUI.Pages;
public static class UraniumShapes
{
    public static Geometry ArrowLeft = GeometryConverter.FromPath(Paths.ArrowLeft);
    public static Geometry ExclamationCircle = GeometryConverter.FromPath(Paths.ExclamationCircle);
    public static Geometry X = GeometryConverter.FromPath(Paths.X);

    public static class Paths
	{
		public const string ArrowLeft = "M9.46967 5.46967C9.76256 5.17678 10.2374 5.17678 10.5303 5.46967L16.5303 11.4697C16.8232 11.7626 16.8232 12.2374 16.5303 12.5303L10.5303 18.5303C10.2374 18.8232 9.76256 18.8232 9.46967 18.5303C9.17678 18.2374 9.17678 17.7626 9.46967 17.4697L14.9393 12L9.46967 6.53033C9.17678 6.23744 9.17678 5.76256 9.46967 5.46967Z";
		public const string ExclamationCircle = "M 2.7835 16.2165 A 9.5 9.5 90 1 1 16.2165 2.7835 A 9.5 9.5 90 0 1 2.7835 16.2165 z M 8.55 4.75 v 5.7 h 1.9 V 4.75 H 8.55 z m 0 7.6 v 1.9 h 1.9 v -1.9 H 8.55 z";
        public const string X = "M17.705 7.705l-1.41-1.41L12 10.59 7.705 6.295l-1.41 1.41L10.59 12l-4.295 4.295 1.41 1.41L12 13.41l4.295 4.295 1.41-1.41L13.41 12l4.295-4.295z";
    }
}
