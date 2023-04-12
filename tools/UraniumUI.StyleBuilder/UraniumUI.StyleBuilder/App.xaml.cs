using UraniumUI.Material.Resources;

namespace UraniumUI.StyleBuilder;
public partial class App : Application
{
    public App(AppShell shell)
    {
        InitializeComponent();

        MainPage = shell;
    }
}
