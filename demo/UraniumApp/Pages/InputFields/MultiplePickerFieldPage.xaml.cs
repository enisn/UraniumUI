using UraniumUI;

namespace UraniumApp.Pages.InputFields;

public partial class MultiplePickerFieldPage : ContentPage
{
	public MultiplePickerFieldPage()
	{
		InitializeComponent();
    }

    private void Reset(object sender, EventArgs eventArgs)
    {
        if (BindingContext != null)
        {
            BindingContext = UraniumServiceProvider.Current.GetRequiredService(BindingContext.GetType());
        }
    }
}