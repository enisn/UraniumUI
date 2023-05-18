using UraniumApp.ViewModels;
using UraniumUI;

namespace UraniumApp.Pages;

public partial class CheckBoxesPage : ContentPage
{
    public CheckBoxesPage(CheckBoxesViewModel vm)
    {
        BindingContext = vm;
        InitializeComponent();
        App.Current.RequestedThemeChanged += (s,e)=> Reset();
    }

    void Button_Clicked(object sender, EventArgs e)
    {
        Reset();
    }

    void Reset()
    {
        _ = BindingContext;

        BindingContext = UraniumServiceProvider.Current.GetRequiredService<CheckBoxesViewModel>();
    }
}
