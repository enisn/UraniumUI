namespace UraniumApp.ViewModels;

public class MyTemplateSelector : DataTemplateSelector
{
    public DataTemplate TemplateDefault { get; set; } = DefaultItemTemplate;
    public DataTemplate TemplateA { get; set; } = DefaultItemTemplate;
    public DataTemplate TemplateB { get; set; } = DefaultItemTemplate;
    public DataTemplate TemplateC { get; set; } = DefaultItemTemplate;

    protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
    {
        var myItem = (MyItem)item; 
        return (myItem) switch
        {
            _ when myItem.Name.StartsWith("A") => TemplateA,
            _ when myItem.Name.StartsWith("B") => TemplateB,
            _ when myItem.Name.StartsWith("C") => TemplateC,
            _ => TemplateDefault
        };
    }

    public static DataTemplate DefaultItemTemplate = new DataTemplate(() =>
    {
        var label = new Label { VerticalOptions = LayoutOptions.Center };
        label.Text = "!!!";
        label.TextColor = Colors.Red;
        return label;
    });
}