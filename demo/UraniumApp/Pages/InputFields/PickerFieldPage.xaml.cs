using System;
using UraniumUI;

namespace UraniumApp.Pages.InputFields;

public partial class PickerFieldPage : ContentPage
{
	public PickerFieldPage()
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