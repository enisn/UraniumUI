using DotNurse.Injector.Attributes;
using Microsoft.Maui.Controls;
using UraniumApp.ViewModels;

namespace UraniumApp.Pages;

[ServiceLifeTime(ServiceLifetime.Singleton)]
public partial class FontIconsMaterialPage : ContentPage
{
	public FontIconsMaterialPage(FontIconsMaterialViewModel viewModel)
	{
		InitializeComponent();
		this.BindingContext = viewModel;
	}
}