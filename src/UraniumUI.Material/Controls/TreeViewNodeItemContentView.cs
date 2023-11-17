namespace UraniumUI.Material.Controls;

public class TreeViewNodeItemContentView : ContentView
{

    private static void ItemTemplateChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = bindable as TreeViewNodeItemContentView;
        control.BuildItem();
    }

    private static void SourceChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = bindable as TreeViewNodeItemContentView;
        control.BuildItem();
    }

    public bool HideOnNullContent { get; set; } = false;

    protected void BuildItem()
    {
        if (Item is null)
        {
            Content = null;
            return;
        }

        //Create the content
        try
        {
            Content = CreateTemplateForItem(Item, ItemTemplate, false);
        }
        catch
        {
            Content = null;
        }
        finally
        {
            if (HideOnNullContent)
                IsVisible = Content != null;
        }
    }


    public DataTemplate ItemTemplate
    {
        get => (DataTemplate)GetValue(ItemTemplateProperty);
        set => SetValue(ItemTemplateProperty, value);
    }

    public static readonly BindableProperty ItemTemplateProperty = BindableProperty.Create(
        nameof(ItemTemplate), typeof(DataTemplate), typeof(TreeViewNodeItemContentView),
        propertyChanged: ItemTemplateChanged);

    public object Item
    {
        get => GetValue(ItemProperty);
        set => SetValue(ItemProperty, value);
    }

    public static readonly BindableProperty ItemProperty = BindableProperty.Create(
        nameof(Item), typeof(object), typeof(TreeViewNodeItemContentView), null,
        propertyChanged: SourceChanged);


    public static View CreateTemplateForItem(object item, DataTemplate itemTemplate, bool createDefaultIfNoTemplate = true)
    {
        var templateToUse = itemTemplate is DataTemplateSelector templateSelector ? 
                templateSelector.SelectTemplate(item, null) : 
                itemTemplate;

        //If we still don't have a template, create a label
        if (templateToUse is null)
            return createDefaultIfNoTemplate ? new Label() { Text = item.ToString() } : null;

        //Create the content
        //If a view wasn't created, we can't use it, exit
        if (!(templateToUse.CreateContent() is View view)) return new Label() { Text = item.ToString() }; ;

        //Set the binding
        view.BindingContext = item;

        return view;
    }
}
