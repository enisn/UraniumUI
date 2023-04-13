using Microsoft.Extensions.Options;

namespace UraniumUI.StyleBuilder.Templating;
public class GenericDataTemplateSelector : DataTemplateSelector
{
    protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
    {
        var options = UraniumServiceProvider.Current.GetRequiredService<IOptions<DataTemplateOptions>>().Value;

        if(options.DataTemplates.TryGetValue(item.GetType(), out var dataTemplate))
        {
            return dataTemplate;
        }

        throw new InvalidOperationException("DataTemplate not found in DataTemplateOptions");

//#if DEBUG
//        return new DataTemplate(() => new Label
//        {
//            Text = "DataTemplate not found in DataTemplateOptions"
//        });
//#else
//        throw new InvalidOperationException("DataTemplate not found in DataTemplateOptions");
//#endif
    }
}
