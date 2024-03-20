using System.Windows.Input;

namespace UraniumUI.Extensions;
public static class CommandExtensions
{
    public static bool TryExecute(this ICommand command, object parameter)
    {
        if (command is not null && command.CanExecute(parameter))
        {
            command.Execute(parameter);
            return true;
        }

        return false;
    }
}
