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
        //var root = serviceProvider.GetRequiredService<IRootObjectProvider>()
        //        .RootObject as BindableObject;

        var provideValueTarget = serviceProvider.GetRequiredService<IProvideValueTarget>();

        var root = GetRootObject(provideValueTarget) as BindableObject;

        var targetObject = provideValueTarget.TargetObject;

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

    public static object? GetRootObject(IProvideValueTarget provideValueTarget)
    {
        //Attemp to get the root object from the provided value target
        var pvtType = provideValueTarget?.GetType();
        var parentsInfo = pvtType?.GetProperty("Microsoft.Maui.Controls.Xaml.IProvideParentValues.ParentObjects", BindingFlags.NonPublic | BindingFlags.Instance);
        var parents = parentsInfo?.GetValue(provideValueTarget) as IEnumerable<object>;
        return parents?.LastOrDefault();
    }
}
