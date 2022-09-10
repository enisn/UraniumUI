using UraniumUI;

namespace UraniumApp;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();

		//this.InitializeUraniumUIResources();

		MainPage = new AppShell();
	}
}
