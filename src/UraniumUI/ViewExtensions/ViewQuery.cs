using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UraniumUI.Extensions;

namespace UraniumUI.ViewExtensions;
public static class ViewQuery
{
    public static readonly BindableProperty IdProperty = BindableProperty.CreateAttached(
        "Id", 
        typeof(string),
        typeof(ViewQuery),
        string.Empty);

    public static string GetId(this BindableObject obj)
    {
        return (string)obj.GetValue(IdProperty);
    }

    public static void SetId(this BindableObject obj, string value)
    {
        obj.SetValue(IdProperty, value);
    }

    public static T FindByViewQueryId<T>(this View view, string id) 
        where T : VisualElement
    {
        return view.FindInChildrenHierarchy<T>(x => GetId(x) == id);
    }

    public static IEnumerable<T> FindManyByViewQueryId<T>(this View view, string id)
        where T : View
    {
        return view.FindManyInChildrenHierarchy<T>(x => GetId(x) == id);
    }
}
