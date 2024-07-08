using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UraniumUI.Extensions;
public static class AnimationExtensions
{
    public static Task FadeToSafely(this VisualElement element, double opacity, uint length = 250, Easing easing = null)
    {
        if (!element.IsLoaded)
        {
            element.Opacity = opacity;
            return Task.CompletedTask;
        }

        return element.FadeTo(opacity, length, easing);
    }

    public static Task RelScaleToSafely(this VisualElement element, double scale, uint length = 250, Easing easing = null)
    {
        if (!element.IsLoaded)
        {
            element.Scale = scale;
            return Task.CompletedTask;
        }

        return element.RelScaleTo(scale, length, easing);
    }

    public static Task RotateToSafely(this VisualElement element, double degrees, uint length = 250, Easing easing = null)
    {
        if (!element.IsLoaded)
        {
            element.Rotation = degrees;
            return Task.CompletedTask;
        }

        return element.RotateTo(degrees, length, easing);
    }

    public static Task RelRotateToSafely(this VisualElement element, double degrees, uint length = 250, Easing easing = null)
    {
        if (!element.IsLoaded)
        {
            element.Rotation += degrees;
            return Task.CompletedTask;
        }

        return element.RelRotateTo(degrees, length, easing);
    }

    public static Task ScaleToSafely(this VisualElement element, double scale, uint length = 250, Easing easing = null)
    {
        if (!element.IsLoaded)
        {
            element.Scale = scale;
            return Task.CompletedTask;
        }

        return element.ScaleTo(scale, length, easing);
    }

    public static Task TranslateToSafely(this VisualElement element, double x, double y, uint length = 250, Easing easing = null)
    {
        if (!element.IsLoaded)
        {
            element.TranslationX = x;
            element.TranslationY = y;
            return Task.CompletedTask;
        }

        return element.TranslateTo(x, y, length, easing);
    }
}
