using UraniumApp.ViewModels;
using UraniumUI;

namespace UraniumApp.Pages;

public partial class RadioButtonsPage : ContentPage
{
    public RadioButtonsPage(RadioButtonsViewModel vm)
    {
        BindingContext = vm;
        InitializeComponent();
        App.Current.RequestedThemeChanged += (s, e) => Reset();
    }

    private void Reset_Clicked(object sender, EventArgs e)
    {
        Reset();
    }

    private void Reset()
    {
        BindingContext = UraniumServiceProvider.Current.GetRequiredService<RadioButtonsViewModel>();
    }
}
