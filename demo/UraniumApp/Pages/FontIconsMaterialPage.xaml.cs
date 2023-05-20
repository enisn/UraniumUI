using DotNurse.Injector.Attributes;

namespace UraniumApp.Pages;

[ServiceLifeTime(ServiceLifetime.Singleton)]
public partial class FontIconsMaterialPage : ContentPage
{
	public FontIconsMaterialPage()
	{
		InitializeComponent();
	}
}