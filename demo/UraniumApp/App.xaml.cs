using InputKit.Shared.Controls;
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
}
