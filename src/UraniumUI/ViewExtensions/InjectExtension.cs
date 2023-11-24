namespace UraniumUI.ViewExtensions;

[ContentProperty(nameof(Type))]
[AcceptEmptyServiceProvider]
public class InjectExtension : IMarkupExtension
{
    public Type Type { get; set; }
    public object ProvideValue(IServiceProvider serviceProvider)
    {
        return UraniumServiceProvider.Current.GetRequiredService(Type);
    }
}
