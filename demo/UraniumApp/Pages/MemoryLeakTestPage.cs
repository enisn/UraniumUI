using MemoryToolkit.Maui;

namespace UraniumApp.Pages;

public class MemoryLeakTestPage : ContentPage
{
    public MemoryLeakTestPage(Type controlType)
    {
        this.Content = new VerticalStackLayout
        {
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            Spacing = 10,
            Children =
            {
                (View)Activator.CreateInstance(controlType),
                new Button
                {
                    Text = "Go Back to complete",
                    StyleClass = new[] { "FilledButton" },
                    Command = new Command(() =>
                    {
                        Navigation.PopModalAsync();
                    })
                }
            }
        };

        LeakMonitorBehavior.SetCascade(this, true);
    }
}