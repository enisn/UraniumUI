using InputKit.Shared.Helpers;
using Microsoft.Maui.Controls.Shapes;

namespace UraniumUI.Pages;
public static class UraniumShapes
{
    public static Geometry ArrowRight = GeometryConverter.FromPath(Paths.ArrowRight);
    public static Geometry ArrowDown = GeometryConverter.FromPath(Paths.ArrowDown);
    public static Geometry ExclamationCircle = GeometryConverter.FromPath(Paths.ExclamationCircle);
    public static Geometry X = GeometryConverter.FromPath(Paths.X);
    public static Geometry XCircle = GeometryConverter.FromPath(Paths.XCircle);
    public static Geometry Eye = GeometryConverter.FromPath(Paths.Eye);
    public static Geometry EyeSlash = GeometryConverter.FromPath(Paths.EyeSlash);

    public static class Paths
	{
		public const string ArrowRight = "m 5.9697 2.9697 c 0.2929 -0.2929 0.7677 -0.2929 1.0606 0 l 6 6 c 0.2929 0.2929 0.2929 0.7677 0 1.0606 l -6 6 c -0.2929 0.2929 -0.7677 0.2929 -1.0606 0 c -0.2929 -0.2929 -0.2929 -0.7677 0 -1.0606 l 5.4696 -5.4697 l -5.4696 -5.4697 c -0.2929 -0.2929 -0.2929 -0.7678 0 -1.0607 z";
		public const string ArrowDown = "m 14.0303 1.9697 c 0.2929 0.2929 0.2929 0.7677 0 1.0606 l -6 6 c -0.2929 0.2929 -0.7677 0.2929 -1.0606 0 l -6 -6 c -0.2929 -0.2929 -0.2929 -0.7677 -0 -1.0606 c 0.2929 -0.2929 0.7677 -0.2929 1.0606 -0 l 5.4697 5.4696 l 5.4697 -5.4696 c 0.2929 -0.2929 0.7678 -0.2929 1.0607 -0 z";
		public const string ExclamationCircle = "M 2.9835 16.4165 A 9.5 9.5 90 1 1 16.4165 2.9835 A 9.5 9.5 90 0 1 2.9835 16.4165 z M 8.75 4.95 v 5.7 h 1.9 V 4.95 H 8.75 z m 0 7.6 v 1.9 h 1.9 v -1.9 H 8.75 z";
        public const string X = "M 11.705 1.705 L 10.295 0.295 L 6 4.59 L 1.705 0.295 L 0.295 1.705 L 4.59 6 L 0.295 10.295 L 1.705 11.705 L 6 7.41 L 10.295 11.705 L 11.705 10.295 L 7.41 6 L 11.705 1.705 Z";
        public const string XCircle = "M 8.25 0 C 6.0463 0 3.9746 0.8582 2.4164 2.4163 c -3.2167 3.2167 -3.2167 8.4505 -0.0001 11.6673 C 3.9745 15.6419 6.0463 16.5 8.25 16.5 c 2.2036 0 4.2754 -0.8582 5.8336 -2.4164 c 3.2167 -3.2167 3.2167 -8.4506 0 -11.6673 C 12.5254 0.8582 10.4536 0 8.25 0 z M 11.9623 11.9623 c -0.1465 0.1464 -0.3384 0.2197 -0.5303 0.2197 c -0.1919 0 -0.3839 -0.0733 -0.5302 -0.2197 L 8.25 9.3107 l -2.6517 2.6517 c -0.1465 0.1464 -0.3384 0.2197 -0.5303 0.2197 c -0.1919 0 -0.3839 -0.0733 -0.5302 -0.2197 c -0.293 -0.2928 -0.293 -0.7678 0 -1.0607 L 7.1894 8.25 l -2.6517 -2.6517 c -0.293 -0.2928 -0.293 -0.7678 0 -1.0607 c 0.2928 -0.2928 0.7678 -0.2928 1.0607 0 L 8.25 7.1894 l 2.6516 -2.6517 c 0.2928 -0.2928 0.7678 -0.2928 1.0607 0 c 0.293 0.2928 0.293 0.7678 0 1.0607 L 9.3107 8.25 l 2.6517 2.6516 C 12.2552 11.1945 12.2552 11.6694 11.9623 11.9623 z";
        public const string Eye = "M10 12c-5.042.007-10-2.686-10-6S4.984-.017 10 0c5.016.017 10 2.686 10 6s-4.958 5.993-10 6zm0-2a4 4 0 1 0 0-8 4 4 0 0 0 0 8zm0-2a2 2 0 1 1 0-4 2 2 0 0 1 0 4z";
        public const string EyeSlash = "M9.329 11.885L2.12 19.092a1 1 0 1 1-1.414-1.414l7.324-7.324a2 2 0 0 1 2.322-2.322L17.679.706a1 1 0 0 1 1.414 1.414L11.885 9.33a2 2 0 0 1-2.556 2.556zm7.54-6.127C18.75 6.842 20 8.34 20 10c0 3.314-4.958 5.993-10 6a14.734 14.734 0 0 1-3.053-.32l1.861-1.86a4 4 0 0 0 5.011-5.011l3.05-3.051zm-4.16-1.496l-1.834 1.834a4 4 0 0 0-4.779 4.779L2.869 14.1C1.134 13.028 0 11.585 0 10c0-3.314 4.984-6.017 10-6 .914.003 1.827.094 2.709.262z";
    }
}
