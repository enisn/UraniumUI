using DotNurse.Injector.Attributes;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace UraniumApp;

[RegisterAs(typeof(ThemeChangeViewModel), ServiceLifetime.Singleton)]
public class ThemeChangeViewModel : ReactiveObject
{
    public AppTheme[] AppThemeList { get; } = new[] { AppTheme.Light, AppTheme.Dark };
    [Reactive] public AppTheme SelectedTheme { get; set; }

    public ThemeChangeViewModel()
    {
        SelectedTheme = App.Current.RequestedTheme == AppTheme.Dark ? AppTheme.Dark : AppTheme.Light;

        App.Current.RequestedThemeChanged += (s, e) =>
        {
            if (SelectedTheme != App.Current.RequestedTheme)
                SelectedTheme = App.Current.RequestedTheme;
        };

        this.
            WhenAnyValue(x => x.SelectedTheme).
            Subscribe(theme =>
            {
                if (App.Current.UserAppTheme != theme)
                {
                    App.Current.UserAppTheme = theme;
                }
            });
    }
}