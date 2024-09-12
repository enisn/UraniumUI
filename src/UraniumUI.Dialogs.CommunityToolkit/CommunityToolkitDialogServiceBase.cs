using Microsoft.Extensions.Options;
using UraniumUI.Extensions;

namespace UraniumUI.Dialogs.CommunityToolkit;

public class CommunityToolkitDialogServiceBase
{
    private Page page;

    public Page Page { get => page ?? GetCurrentPage(); set => page = value; }

    protected DialogOptions DialogOptions { get; }

    public CommunityToolkitDialogServiceBase(IOptions<DialogOptions> dialogOptions)
    {
        DialogOptions = dialogOptions.Value;
    }

    protected static View GetFrame(double width, View content)
    {
        var frame = new Border
        {
            Content = content,
            StyleClass = new[] { "SurfaceContainer", "Rounded" },
            Padding = 0,
            VerticalOptions = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.Center,
            WidthRequest = width,
        };

        var options = UraniumServiceProvider.Current.GetRequiredService<IOptions<DialogOptions>>()?.Value;

        foreach (var efectFactory in options.Effects)
        {
            frame.Effects.Add(efectFactory());
        }

        return frame;
    }

    protected virtual Page GetCurrentPage()
    {
        if (Application.Current.MainPage is Shell shell)
        {
            return shell.CurrentPage;
        }

        if (Application.Current.MainPage is NavigationPage nav)
        {
            return nav.CurrentPage;
        }

        if (Application.Current.MainPage is TabbedPage tabbed)
        {
            return tabbed.CurrentPage;
        }
        if (Application.Current.MainPage is FlyoutPage page)
        {
	        return page.Flyout;
        }
        return Application.Current.MainPage;
    }

    protected virtual View GetDivider()
    {
        if (DialogOptions.GetDivider != null)
        {
            return DialogOptions.GetDivider();
        }

        return new BoxView { StyleClass = new[] { "Divider" }, Margin = 0 };
    }

    protected virtual View GetHeader(string title)
    {
        if (DialogOptions.GetHeader != null)
        {
            return DialogOptions.GetHeader(title);
        }

        return new StackLayout
        {
            HorizontalOptions = LayoutOptions.Fill,
            Children =
            {
                new Label
                {
                    Text = title,
                    Margin = 20,
                },
                GetDivider(),
            }
        };
    }

    protected virtual View GetFooter(Dictionary<string, Command> footerButtons)
    {
        if (DialogOptions.GetFooter != null)
        {
            return DialogOptions.GetFooter(footerButtons);
        }

        var layout = new FlexLayout
        {
            JustifyContent = Microsoft.Maui.Layouts.FlexJustify.End,
            Margin = new Thickness(10),
        };

        foreach (var button in footerButtons.Reverse())
        {
            layout.Children.Add(new Button
            {
                Text = button.Key,
                StyleClass = new[] { "TextButton", "Dialog.Button" + layout.Children.Count },
                Command = button.Value
            });
        }

        return layout;
    }

    protected virtual Size CalculateSize(Page page)
    {
        if (DeviceInfo.Current.Idiom == DeviceIdiom.Desktop || DeviceInfo.Current.Idiom == DeviceIdiom.Tablet)
        {
            return new Size(400, 400);
        }

        if (DeviceInfo.Current.Idiom == DeviceIdiom.Phone)
        {
            var baseValue = page.Width;
            if (page.Width > page.Height)
            {
                baseValue = page.Height;
            }

            var edge = (baseValue * .8).Clamp(200, 400);

            return new Size(edge, edge * .9);
        }

        return new Size(100, 100);
    }
}