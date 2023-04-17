using CommunityToolkit.Maui.Core.Extensions;
using DynamicData;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UraniumUI.StyleBuilder.StyleManager;
public class ReactiveStyle : ReactiveObject
{
    public ReactiveStyle()
    {
    }

    [Reactive] public Type TargetType { get; set; }

    [Reactive] public bool ApplyToDerivedTypes { get; set; }

    [Reactive] public string Class { get; set; }

    [Reactive] public ObservableCollection<ReactiveSetter> Setters { get; set; }

    public Style ToStyle()
    {
        var style = new Style(TargetType)
        {
            ApplyToDerivedTypes = ApplyToDerivedTypes,
        };

        style.Setters.AddRange(Setters.Select(x => x.ToSetter()));

        return style;
    }

    public static ReactiveStyle FromStyle(Style style)
    {
        return new ReactiveStyle
        {
            TargetType = style.TargetType,
            ApplyToDerivedTypes= style.ApplyToDerivedTypes,
            Class = style.Class,
            Setters = style.Setters
                .Select(ReactiveSetter.FromSetter)
                .ToObservableCollection()
        };
    }
}

public class ReactiveSetter : ReactiveObject
{
    [Reactive] public string TargetName { get; set; }
    public BindableProperty Property { get; set; }
    [Reactive] public object Value { get; set; }

    public Setter ToSetter()
    {
        return new Setter()
        {
            TargetName = TargetName,
            Property = Property,
            Value = Value
        };
    }

    public static ReactiveSetter FromSetter(Setter setter)
    {
        return new ReactiveSetter
        {
            Property = setter.Property,
            Value = setter.Value,
            TargetName = setter.TargetName
        };
    }
}
