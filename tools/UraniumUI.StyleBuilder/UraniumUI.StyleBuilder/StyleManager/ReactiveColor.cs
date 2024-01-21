using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace UraniumUI.StyleBuilder.StyleManager;

public class ReactiveColor : ReactiveObject
{
    public ReactiveColor(Color color)
    {
        Color = color;
    }

    [Reactive] public Color Color { get; set; }

    public static implicit operator ReactiveColor(Color color)
    {
        return new ReactiveColor(color);
    }
}