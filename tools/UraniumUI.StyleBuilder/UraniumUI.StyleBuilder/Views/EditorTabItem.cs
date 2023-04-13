using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UraniumUI.StyleBuilder.Templating;

namespace UraniumUI.StyleBuilder.Views;
public class EditorTabItem : ContentView
{
    protected readonly DataTemplateOptions dataTemplateOptions;
    public EditorTabItem()
    {
        dataTemplateOptions = UraniumServiceProvider.Current
            .GetRequiredService<IOptions<DataTemplateOptions>>().Value;
    }

    protected override void OnBindingContextChanged()
    {
        base.OnBindingContextChanged();
        if (dataTemplateOptions.DataTemplates.TryGetValue(BindingContext.GetType(), out var dataTemplate))
        {
            this.Content = dataTemplate.CreateContent() as View;
        }
    }
}
