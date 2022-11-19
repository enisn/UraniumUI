using System.Windows.Input;

namespace UraniumUI.Views;
public interface IStatefulView : IView
{
    object CommandParameter { get; set; }
    ICommand HoverCommand { get; set; }
    ICommand HoverExitCommand { get; set; }
    ICommand LongPressCommand { get; set; }
    ICommand PressedCommand { get; set; }
    ICommand TappedCommand { get; set; }
}