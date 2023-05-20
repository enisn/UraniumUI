using UraniumUI;

namespace UraniumApp;

[ContentProperty(nameof(Type))]
[AcceptEmptyServiceProvider]
public class InjectExtension : IMarkupExtension<object>
{
    public Type Type { get; set; }
    public object ProvideValue(IServiceProvider serviceProvider)
    {
        return UraniumServiceProvider.Current.GetRequiredService(Type);
    }
}
