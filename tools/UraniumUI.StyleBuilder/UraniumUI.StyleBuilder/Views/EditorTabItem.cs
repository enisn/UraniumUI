using Microsoft.Extensions.Options;
using System.Windows.Input;
using UraniumUI.StyleBuilder.Templating;

namespace UraniumUI.StyleBuilder.Views;
public class EditorTabItem : ContentView
{
    protected override void OnBindingContextChanged()
    {
        base.OnBindingContextChanged();

        var dataTemplateOptions = UraniumServiceProvider.Current
            .GetRequiredService<IOptions<DataTemplateOptions>>().Value;

        if (dataTemplateOptions.DataTemplates.TryGetValue(BindingContext.GetType(), out var dataTemplate))
        {
            try
            {
                var content = dataTemplate.CreateContent() as View;
                this.Content = content;
            }
            catch (Exception ex)
            {
                App.Current.MainPage.DisplayAlert("Error", ex.ToString(), "OK");
            }
        }
    }
}
