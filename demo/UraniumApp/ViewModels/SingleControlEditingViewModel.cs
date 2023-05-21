using DynamicData.Binding;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using UraniumApp.Inputs.ColorPicking;

namespace UraniumApp.ViewModels;
public class SingleControlEditingViewModel<T> : ReactiveObject
    where T : View, new()
{
    [Reactive] public string Title { get; set; }
    [Reactive] public T Control { get; set; }
    [Reactive] public List<BindableProperty> EditingProperties { get; set; }
    public string XamlSourceCode => SourceCode.ToString();
    protected XDocument SourceCode { get; }

    public SingleControlEditingViewModel()
    {
        Control = InitializeControl();
        Title = typeof(T).Name;
        SourceCode = XDocument.Parse(InitialXDocumentCode);

        InitializeDefaultValues(defaultValues);
        this.WhenAnyValue(x => x.EditingProperties)
            .Subscribe(InitializeProperties);
    }
    protected virtual string InitialXDocumentCode => $"""<ContentPage xmlns:material="http://schemas.enisn-projects.io/dotnet/maui/uraniumui/material"><material:{typeof(T).Name}/></ContentPage>""";
    protected readonly Dictionary<string, object> defaultValues = new Dictionary<string, object>();
    protected virtual T InitializeControl()
    {
        return new T();
    }

    protected virtual void InitializeDefaultValues(Dictionary<string, object> values)
    {
    }

    protected IDisposable controlPropertyChangedDisposable;
    protected void InitializeProperties(List<BindableProperty> list)
    {
        controlPropertyChangedDisposable?.Dispose();

        if (list != null)
        {
            controlPropertyChangedDisposable = Control
                .WhenAnyPropertyChanged(list.Select(s => s.PropertyName).ToArray())
                .Subscribe(GenerateSourceCode);

            GenerateSourceCode();
        }
    }

    protected virtual void GenerateSourceCode(object parameter = null)
    {
        var contentPage = SourceCode.Descendants().First();

        var material = contentPage.GetNamespaceOfPrefix("material");

        var control = contentPage.Descendants(material + Control.GetType().Name).First();

        foreach (var property in EditingProperties)
        {
            var value = Control.GetValue(property);

            if (value is null)
            {
                control.SetAttributeValue(property.PropertyName, null);
                continue;
            }

            if (defaultValues.TryGetValue(property.PropertyName, out var predefinedDefaultValue)
                && (value.Equals(predefinedDefaultValue)))
            {
                value = null;
            }
            else if (value is string str && string.IsNullOrEmpty(str))
            {
                value = null;
            }
            else if (value.Equals(property.DefaultValue))
            {
                value = null;
            }

            control.SetAttributeValue(property.PropertyName, FormatValue(value));
        }

        PostGenerateSourceCode(control);

        this.RaisePropertyChanged(nameof(XamlSourceCode));
    }

    protected virtual void PostGenerateSourceCode(XElement control)
    {
    }

    protected virtual string FormatValue(object value)
    {
        if (value is null)
        {
            return null;
        }

        if (value is Color color)
        {
            return color.ToArgbHex();
        }

        if (value is Microsoft.Maui.Keyboard keyboard)
        {
            return keyboard.GetType().Name.Replace("Keyboard", string.Empty);
        }

        return value.ToString();
    }
}
