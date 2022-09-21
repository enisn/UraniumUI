namespace UraniumUI.Extensions;
public static class ViewExtensions
{
    public static T FindInParents<T>(this View view)
        where T : View
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

        return null;
    }
}
