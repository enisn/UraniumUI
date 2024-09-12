namespace UraniumUI.Dialogs;
public class DefaultDialogAnimatedContentPage : ContentPage
{
	public DefaultDialogAnimatedContentPage()
	{
        Loaded += OnLoaded;
	}

    private void OnLoaded(object sender, EventArgs e)
    {
        Loaded -= OnLoaded;

        if (Content is null)
        {
            return;
        }

        Content.Opacity = 0;
        Content.Scale = 0.8;

        Content.FadeTo(1, 250, Easing.CubicInOut);
        Content.ScaleTo(1, 250, Easing.CubicInOut);
    }

    public async Task CloseAsync()
    {
        if (Content is null)
        {
            return;
        }

        var tasks = new Task[]
        {
            Content.FadeTo(0, 250, Easing.CubicInOut),
            Content.ScaleTo(0.8, 250, Easing.CubicInOut)
        };

        await Task.WhenAll(tasks);

        await Navigation.PopModalAsync();
    }

    
}
