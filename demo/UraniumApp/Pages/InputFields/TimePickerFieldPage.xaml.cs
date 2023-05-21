using UraniumUI;

namespace UraniumApp.Pages.InputFields;

public partial class TimePickerFieldPage : ContentPage
{
	public TimePickerFieldPage()
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