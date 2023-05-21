using UraniumUI;

namespace UraniumApp.Pages.InputFields;

public partial class DatePickerFieldPage : ContentPage
{
	public DatePickerFieldPage()
	{
		InitializeComponent();
	}

    private void Reset(object sender, EventArgs e)
    {
        if (BindingContext != null)
        {
            BindingContext = UraniumServiceProvider.Current.GetRequiredService(BindingContext.GetType());
        }
    }
}