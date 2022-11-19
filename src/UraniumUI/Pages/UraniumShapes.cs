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
		public const string ArrowRight = "m 5.9697 2.9697 c 0.2929 -0.2929 0.7677 -0.2929 1.0606 0 l 6 6 c 0.2929 0.2929 0.2929 0.7677 0 1.0606 l -6 6 c -0.2929 0.2929 -0.7677 0.2929 -1.0606 0 c -0.2929 -0.2929 -0.2929 -0.7677 0 -1.0606 l 5.4696 -5.4697 l -5.4696 -5.4697 c -0.2929 -0.2929 -0.2929 -0.7678 0 -1.0607 z";
		public const string ExclamationCircle = "M 2.9835 16.4165 A 9.5 9.5 90 1 1 16.4165 2.9835 A 9.5 9.5 90 0 1 2.9835 16.4165 z M 8.75 4.95 v 5.7 h 1.9 V 4.95 H 8.75 z m 0 7.6 v 1.9 h 1.9 v -1.9 H 8.75 z";
        public const string X = "M 11.705 1.705 L 10.295 0.295 L 6 4.59 L 1.705 0.295 L 0.295 1.705 L 4.59 6 L 0.295 10.295 L 1.705 11.705 L 6 7.41 L 10.295 11.705 L 11.705 10.295 L 7.41 6 L 11.705 1.705 Z";
    }
}
