using UraniumUI.Material.Resources;

namespace UraniumUI.StyleBuilder;
public partial class App : Application
{
    public App(IServiceProvider serviceProvider)
    {
        InitializeComponent();

        MainPage = serviceProvider.GetRequiredService<MainPage>();
    }
}
