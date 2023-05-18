using UraniumApp.ViewModels;

namespace UraniumApp.Pages;

public partial class CheckBoxesPage : ContentPage
{
    public CheckBoxesPage(CheckBoxesViewModel vm)
    {
        BindingContext = vm;
        InitializeComponent();
    }
}
