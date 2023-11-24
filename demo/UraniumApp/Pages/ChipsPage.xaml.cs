using UraniumApp.ViewModels;
using UraniumUI;
using UraniumUI.Dialogs.Mopups;
using UraniumUI.Material.Controls;

namespace UraniumApp.Pages;

public partial class ChipsPage : ContentPage
{
	public ChipsPage()
	{
		BindingContext = UraniumServiceProvider.Current.GetRequiredService<ChipsViewModel>();
		InitializeComponent();
	}
}