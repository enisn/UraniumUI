using DynamicData.Binding;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace UraniumApp;

[ContentProperty("Path")]
public class XBindExtension : IMarkupExtension<object>
{
    public string Path { get; set; }

    public object ProvideValue(IServiceProvider serviceProvider)
    {
        var root = serviceProvider.GetRequiredService<IRootObjectProvider>()
                .RootObject as BindableObject;

        var targetObject = serviceProvider.GetRequiredService<IProvideValueTarget>()
            .TargetObject;

        var targetPropertyInfo = serviceProvider.GetRequiredService<IProvideValueTarget>()
            .TargetProperty as PropertyInfo;

        var bindingContext= root.BindingContext as INotifyPropertyChanged;

        bindingContext.WhenAnyPropertyChanged(Path)
            .Subscribe( _ =>
            {
                var newValue = bindingContext.GetType().GetProperty(Path).GetValue(bindingContext);
                targetPropertyInfo.SetValue(targetObject, newValue);
            });

        //bindingContext.PropertyChanged += (s, e) =>
        //{
        //    if (e.PropertyName == Path)
        //    {
        //        var val = bindingContext.GetType().GetProperty(Path).GetValue(bindingContext);

        //        targetPropertyInfo.SetValue(targetObject, val);
        //    }
        //};

        return bindingContext.GetType().GetProperty(Path).GetValue(bindingContext);
    }

}
