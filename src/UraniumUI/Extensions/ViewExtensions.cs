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

    public static T FindInChildrenHierarchy<T>(this View view, Func<T, bool> expression = null)
        where T : VisualElement
    {
        expression ??= _ => true;

        if (view is Layout layout)
        {
            foreach (var item in layout.Children)
            {
                if (item is T found && expression(found))
                {
                    return found;
                }
                else if (item is Layout anotherLayout)
                {
                    return anotherLayout.FindInChildrenHierarchy<T>(expression);
                }
            }
        }

        if (view is Microsoft.Maui.Controls.ContentView contentView)
        {
            return contentView.Content.FindInChildrenHierarchy<T>(expression);
        }

        return null;
    }

    public static IEnumerable<T> FindManyInChildrenHierarchy<T>(this View view, Func<T, bool> expression = null)
        where T : View
    {
        expression ??= _ => true;

        if (view is T itself && expression(itself))
        {
            yield return itself;
        }

        if (view is Layout layout)
        {
            foreach (var item in layout.Children)
            {
                if (item is T found && expression(found))
                {
                    yield return found;
                }

                if (item is View childView)
                {
                    foreach (var child in childView.FindManyInChildrenHierarchy<T>(expression))
                    {
                        yield return child;
                    }
                }
            }
        }

        if (view is IContentView contentView && contentView.Content is View contentViewContent)
        {
            foreach (var child in contentViewContent.FindManyInChildrenHierarchy<T>(expression))
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

    public static void AddIf(this ICollection<IView> collection, View view, bool condition)
    {
        if (condition)
        {
            collection.Add(view);
        }
    }
}
