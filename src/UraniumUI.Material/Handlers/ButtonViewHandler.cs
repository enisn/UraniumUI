#if NET7_0

using Microsoft.Maui.Handlers;
using System.Windows.Input;
using UraniumUI.Material.Controls;

namespace UraniumUI.Material.Handlers;
public partial class ButtonViewHandler : BorderHandler
{
    public ButtonView StatefulView => VirtualView as ButtonView;
    private void ExecuteCommandIfCan(ICommand command)
    {
        if (command?.CanExecute(StatefulView.CommandParameter) ?? false)
        {
            command.Execute(StatefulView.CommandParameter);
        }
    }
}
#endif