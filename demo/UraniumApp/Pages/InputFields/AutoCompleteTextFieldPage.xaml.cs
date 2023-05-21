using UraniumUI;

namespace UraniumApp.Pages.InputFields;

public partial class AutoCompleteTextFieldPage : ContentPage
{
	public AutoCompleteTextFieldPage()
	{
		InitializeComponent();
        App.Current.RequestedThemeChanged += (_, _) => Reset();
    }

    private void Reset()
    {
        if (BindingContext != null)
        {
            BindingContext = UraniumServiceProvider.Current.GetRequiredService(BindingContext.GetType());
        }
    }
}