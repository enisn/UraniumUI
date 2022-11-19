using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using UraniumUI.Views;

namespace UraniumUI.Material.Controls;

[ContentProperty(nameof(Content))]
public class ButtonView : Border, IStatefulView
{
	public ButtonView()
	{
        Padding = 10;
	}
    
    public ICommand PressedCommand { get => (ICommand)GetValue(PressedCommandProperty); set => SetValue(PressedCommandProperty, value); }

    public static BindableProperty PressedCommandProperty = BindableProperty.Create(nameof(PressedCommand), typeof(ICommand), typeof(StatefulContentView));

    public ICommand HoverCommand { get => (ICommand)GetValue(HoverCommandProperty); set => SetValue(HoverCommandProperty, value); }

    public static BindableProperty HoverCommandProperty = BindableProperty.Create(nameof(HoverCommand), typeof(ICommand), typeof(StatefulContentView));

    public ICommand HoverExitCommand { get => (ICommand)GetValue(HoverExitCommandProperty); set => SetValue(HoverExitCommandProperty, value); }

    public static BindableProperty HoverExitCommandProperty = BindableProperty.Create(nameof(HoverExitCommand), typeof(ICommand), typeof(StatefulContentView));

    public ICommand LongPressCommand { get => (ICommand)GetValue(LongPressCommandProperty); set => SetValue(LongPressCommandProperty, value); }

    public static BindableProperty LongPressCommandProperty = BindableProperty.Create(nameof(LongPressCommand), typeof(ICommand), typeof(StatefulContentView));

    public ICommand TappedCommand { get => (ICommand)GetValue(TappedCommandProperty); set => SetValue(TappedCommandProperty, value); }

    public static BindableProperty TappedCommandProperty = BindableProperty.Create(nameof(TappedCommand), typeof(ICommand), typeof(StatefulContentView));

    public object CommandParameter { get => GetValue(CommandParameterProperty); set => SetValue(CommandParameterProperty, value); }

    public static BindableProperty CommandParameterProperty = BindableProperty.Create(nameof(CommandParameter), typeof(object), typeof(StatefulContentView));
}