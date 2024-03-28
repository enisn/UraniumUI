using System.Globalization;
using Microsoft.Maui.Controls;
using System.Runtime.CompilerServices;
using Microsoft.Maui.Platform;

#if ANDROID
using ViewCompat = AndroidX.Core.View.ViewCompat;
#endif

namespace UraniumUI.Extensions;
public static class ViewExtensions
{
    public static T FindInParents<T>(this View view)
        where T : VisualElement
    {
        var itemToCheck = view.Parent;
        while (itemToCheck != null)
        {
            if (itemToCheck is T found)
            {
                return found;
            }

            itemToCheck = itemToCheck.Parent;
        }

        return default;
    }

    public static T FindInChildrenHierarchy<T>(this View view)
        where T : VisualElement
    {
        if (view is Layout layout)
        {
            foreach (var item in layout.Children)
            {
                if (item is T found)
                {
                    return found;
                }
                else if (item is Layout anotherLayout)
                {
                    return anotherLayout.FindInChildrenHierarchy<T>();
                }
            }
        }

        if (view is Microsoft.Maui.Controls.ContentView contentView)
        {
            return contentView.Content.FindInChildrenHierarchy<T>();
        }

        return null;
    }

    public static IEnumerable<T> FindManyInChildrenHierarchy<T>(this View view)
        where T : View
    {
        if (view is T itself)
        {
            yield return itself;
        }

        if (view is Layout layout)
        {
            foreach (var item in layout.Children)
            {
                if (item is T found)
                {
                    yield return found;
                }

                if (item is View childView)
                {
                    foreach (var child in childView.FindManyInChildrenHierarchy<T>())
                    {
                        yield return child;
                    }
                }
            }
        }

        if (view is IContentView contentView && contentView.Content is View contentViewContent)
        {
            foreach (var child in contentViewContent.FindManyInChildrenHierarchy<T>())
            {
                yield return child;
            }
        }
    }

    public static bool IsRtl(this VisualElement element)
    {
        if (element.FlowDirection != FlowDirection.MatchParent)
        {
            return element.FlowDirection == FlowDirection.RightToLeft;
        }

        if (element.Parent is VisualElement parentElement)
        {
            return IsRtl(parentElement);
        }

#if ANDROID
        if (element.Handler is not null)
        {
            var dir = ViewCompat.GetLayoutDirection(element.ToPlatform(element.Handler.MauiContext));
            return dir == ViewCompat.LayoutDirectionRtl;
        }
#endif

        // Fallback to culture:
        return CultureInfo.CurrentCulture.TextInfo.IsRightToLeft;
    }

    public static IVisualTreeElement GetNextFocusableElement(IVisualTreeElement parent, IVisualTreeElement startAfterElement)
    {
        if (parent == null || startAfterElement == null)
        {
            //Nothing more to search
            return null;
        }

        var parentControls = parent.GetVisualChildren();

        if (parentControls != null)
        {
            IVisualTreeElement firstFocusableElementBefore = null;
            IVisualTreeElement firstFocusableElementAfter = null;

            bool startElementFound = false;

            foreach (var control in parentControls)
            {
                if (control.Equals(startAfterElement))
                {
                    startElementFound = true;

                    continue;
                }

                if (control is View view && view.IsEnabled && view.IsVisible)
                {
                    if (startElementFound)
                    {
                        firstFocusableElementAfter ??= control;
                    }
                    else
                    {
                        firstFocusableElementBefore ??= control;
                    }
                }

                //We have valid candidates, break out
                if (firstFocusableElementBefore != null && firstFocusableElementAfter != null)
                {
                    break;
                }
            }

            if (firstFocusableElementAfter != null)
            {
                return firstFocusableElementAfter;
            }
            else if (firstFocusableElementBefore != null)
            {
                return firstFocusableElementBefore;
            }
            else
            {
                //Go up the stack
                return GetNextFocusableElement(parent.GetVisualParent(), parent);
            }
        }

        return null;
    }
}
