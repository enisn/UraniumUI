namespace UraniumUI.Dialogs;
public class DialogOptions
{
    public List<Func<RoutingEffect>> Effects { get; } = new();

    /// <summary>
    /// Factory for backdrop color. It can be configured to return different colors based on the current theme.
    /// </summary>
    /// <remarks>
    /// It can be used like this:
    /// <code>
    /// options.GetBackdropColor = () => Application.Current.RequestedTheme switch
    /// {
    ///    AppTheme.Light => Color.FromArgb("#80000000"),
    ///    AppTheme.Dark => Color.FromArgb("#80ffffff"),
    ///    _ => Color.FromArgb("#80808080")
    /// }
    /// </code>
    /// </remarks>
    public Func<Color> GetBackdropColor { get; set; } = () => Application.Current.RequestedTheme switch
    {
        AppTheme.Light => Color.FromArgb("#80000000"),
        AppTheme.Dark => Color.FromArgb("#80ffffff"),
        _ => Color.FromArgb("#80808080")
    };

    public Func<View> GetDivider { get; set; }

    public GetFooterDelegate GetFooter { get; set; }

    public GetHeaderDelegate GetHeader { get; set; }

    public delegate View GetFooterDelegate(Dictionary<string, Command> footerButtons);

    public delegate View GetHeaderDelegate(string title);
}
