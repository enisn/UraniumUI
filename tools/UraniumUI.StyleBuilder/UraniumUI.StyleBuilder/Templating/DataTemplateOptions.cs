namespace UraniumUI.StyleBuilder.Templating;
public class DataTemplateOptions
{
    public Dictionary<Type, DataTemplate> DataTemplates { get; } = new();

    public DataTemplateOptions Register<TViewModel, TView>() 
        where TViewModel : class
        where TView : View
    {
        DataTemplates.Add(typeof(TViewModel), new DataTemplate(typeof(TView)));

        return this;
    }
}
