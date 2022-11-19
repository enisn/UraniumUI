using InputKit.Shared.Helpers;
using Microsoft.Maui.Controls.Shapes;

namespace UraniumUI.Pages;
public static class UraniumShapes
{
    public static Geometry ArrowRight = GeometryConverter.FromPath(Paths.ArrowRight);
    public static Geometry ExclamationCircle = GeometryConverter.FromPath(Paths.ExclamationCircle);
    public static Geometry X = GeometryConverter.FromPath(Paths.X);

    public static class Paths
	{
		public const string ArrowRight = "M 0.4697 0.4697 C 0.7626 0.1768 1.2374 0.1768 1.5303 0.4697 L 7.5303 6.4697 C 7.8232 6.7626 7.8232 7.2374 7.5303 7.5303 L 1.5303 13.5303 C 1.2374 13.8232 0.7626 13.8232 0.4697 13.5303 C 0.1768 13.2374 0.1768 12.7626 0.4697 12.4697 L 5.9393 7 L 0.4697 1.5303 C 0.1768 1.2374 0.1768 0.7626 0.4697 0.4697 Z";
		public const string ExclamationCircle = "M 2.9835 16.4165 A 9.5 9.5 90 1 1 16.4165 2.9835 A 9.5 9.5 90 0 1 2.9835 16.4165 z M 8.75 4.95 v 5.7 h 1.9 V 4.95 H 8.75 z m 0 7.6 v 1.9 h 1.9 v -1.9 H 8.75 z";
        public const string X = "M 11.705 1.705 L 10.295 0.295 L 6 4.59 L 1.705 0.295 L 0.295 1.705 L 4.59 6 L 0.295 10.295 L 1.705 11.705 L 6 7.41 L 10.295 11.705 L 11.705 10.295 L 7.41 6 L 11.705 1.705 Z";
    }
}
