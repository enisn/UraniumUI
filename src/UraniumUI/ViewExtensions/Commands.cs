using System.Windows.Input;

namespace UraniumUI.ViewExtensions;
public static class Commands
{
    /// <summary>
    /// Define url as CommandParameter.
    /// </summary>
    public static ICommand OpenLinkCommand { get; } = new Command(parameter =>
    {
        if (parameter is string url)
        {
            Browser.Default.OpenAsync(url);
        }
        else if (parameter is Uri uri)
        {
            Browser.Default.OpenAsync(uri);
        }
        else
        {
            throw new NotSupportedException("CommandParameter doesn't fit to the command!");
        }
    });
}
