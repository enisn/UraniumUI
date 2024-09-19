using InputKit.Shared.Controls;
#if WINDOWS
using Microsoft.UI.Windowing;
#endif
using UraniumUI;
using UraniumUI.Resources;

namespace UraniumApp;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        SelectionView.GlobalSetting.CornerRadius = 0;
        SelectionView.GlobalSetting.Color = ColorResource.GetColor("Secondary", "SecondaryDark");

        MainPage = UraniumServiceProvider.Current.GetRequiredService<AppShell>();
    }

    protected override Window CreateWindow(IActivationState activationState)
    {
        var window = base.CreateWindow(activationState);
        window.Title = "UraniumApp";

#if WINDOWS
        window.HandlerChanged += (sender, args) =>
        {
            if (window.Handler?.PlatformView is MauiWinUIWindow w)
            {
                var presenter = (w.AppWindow.Presenter as OverlappedPresenter);
            }
        };
#endif

        return window;
    }
}
