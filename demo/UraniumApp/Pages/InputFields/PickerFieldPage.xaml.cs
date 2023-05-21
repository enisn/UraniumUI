using System;
using UraniumUI;

namespace UraniumApp.Pages.InputFields;

public partial class PickerFieldPage : ContentPage
{
	public PickerFieldPage()
	{
		InitializeComponent();
    }

    private void Reset()
    {
        if (BindingContext != null)
        {
            BindingContext = UraniumServiceProvider.Current.GetRequiredService(BindingContext.GetType());
        }
    }
}