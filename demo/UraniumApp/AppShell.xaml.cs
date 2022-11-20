namespace UraniumApp;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        themeSwitch.IsToggled = App.Current.RequestedTheme == AppTheme.Dark;
        App.Current.RequestedThemeChanged += (s, e) =>
        {
            themeSwitch.IsToggled = App.Current.RequestedTheme == AppTheme.Dark;
        };
    }

    private void ThemeToggled(object sender, ToggledEventArgs e)
    {
        App.Current.UserAppTheme = e.Value ? AppTheme.Dark : AppTheme.Light;
    }
}
