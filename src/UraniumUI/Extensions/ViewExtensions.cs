using Microsoft.Maui.Controls;

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

        if (view is ContentView contentView)
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
}
