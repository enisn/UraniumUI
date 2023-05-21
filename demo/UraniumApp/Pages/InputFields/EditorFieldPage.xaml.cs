using UraniumUI;

namespace UraniumApp.Pages.InputFields;

public partial class EditorFieldPage : ContentPage
{
    public EditorFieldPage()
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