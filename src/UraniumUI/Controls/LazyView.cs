using UraniumUI.Extensions;

namespace UraniumUI.Controls;

[ContentProperty(nameof(ContentTemplate))]
public class LazyView : ContentView
{
    public static readonly BindableProperty ContentTemplateProperty =
       BindableProperty.Create(nameof(ContentTemplate), typeof(DataTemplate), typeof(LazyView),
           propertyChanged: (bo, ov, nv) => (bo as LazyView).OnContentTemplateChanged());

    public DataTemplate ContentTemplate
    {
        get => (DataTemplate)GetValue(ContentTemplateProperty);
        set => SetValue(ContentTemplateProperty, value);
    }

    public LazyView()
    {
    }

    protected virtual void OnContentTemplateChanged()
    {
        //Task.Run(InitializeAsync).ConfigureAwait(false);
    }

    protected virtual async Task InitializeAsync()
    {
        await Task.Yield(); // Make sure we're on a background thread.

        if (ContentTemplate != null)
        {
            var _content = ContentTemplate.CreateContent() as View;
            if (_content != null)
            {
                //Dispatcher.DispatchDelayed(TimeSpan.FromMilliseconds(10),() =>
                Dispatcher.DispatchDelayed(TimeSpan.FromMilliseconds(10), () =>
                {
                    Content = _content;

                    var rootPage = this.FindInParents<Page>();

                    rootPage.Appearing -= RootPage_Appearing;
                });
            }
        }
    }

    protected override void OnParentSet()
    {
        base.OnParentSet();

        var rootPage = this.FindInParents<Page>();

        rootPage.Appearing += RootPage_Appearing;
    }

    private void RootPage_Appearing(object sender, EventArgs e)
    {
        Console.WriteLine("Appearing");
        Task.Run(InitializeAsync).ConfigureAwait(false);
    }
}
