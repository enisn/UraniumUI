using InputKit.Shared.Controls;
using Microsoft.Extensions.Options;
using Plainer.Maui.Controls;
using UraniumUI.Resources;
using CheckBox = InputKit.Shared.Controls.CheckBox;

namespace UraniumUI.Dialogs;

public class DefaultDialogService : IDialogService
{
    public async Task<bool> ConfirmAsync(string title, string message, string okText = "OK", string cancelText = "Cancel")
    {
        var tcs = new TaskCompletionSource<bool>();
        var currentPage = GetCurrentPage();

        var popupPage = new ContentPage
        {
            BackgroundColor = GetBackdropColor(),
            Content = GetFrame(currentPage.Width, new VerticalStackLayout
            {
                Children =
                {
                    GetHeader(title),
                    new Label
                    {
                        Text = message,
                        Margin = 20,
                    },
                    GetDivider(),
                    GetFooter(okText, new Command(() =>
                    {
                        tcs.SetResult(true);
                        currentPage.Navigation.PopModalAsync(animated: false);
                    }), cancelText, new Command(() =>
                    {
                        tcs.SetResult(false);
                        currentPage.Navigation.PopModalAsync(animated: false);
                    }))
                }
            })
        };

        await currentPage.Navigation.PushModalAsync(ConfigurePopupPage(popupPage), animated: false);

        return await tcs.Task;
    }

    public virtual Task<IEnumerable<T>> DisplayCheckBoxPromptAsync<T>(
        string message,
        IEnumerable<T> selectionSource,
        IEnumerable<T> selectedItems = null,
        string accept = "OK",
        string cancel = "Cancel",
        string displayMember = null)
    {
        var tcs = new TaskCompletionSource<IEnumerable<T>>();

        var prop = displayMember != null ? typeof(T).GetProperty(displayMember) : null;

        var checkBoxGroup = new VerticalStackLayout
        {
            Margin = 20,
            Spacing = 10,
            VerticalOptions = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.Start,
        };

        foreach (var item in selectionSource)
        {
            checkBoxGroup.Add(new CheckBox
            {
                Text = prop != null ? prop.GetValue(item)?.ToString() : item.ToString(),
                CommandParameter = item,
                IsChecked = selectedItems?.Contains(item) ?? false,
            });
        }

        var currentPage = GetCurrentPage();

        var rootGrid = new Grid
        {
            RowDefinitions = new RowDefinitionCollection
             {
                 new RowDefinition(GridLength.Auto),
                 new RowDefinition(GridLength.Star),
                 new RowDefinition(GridLength.Auto),
                 new RowDefinition(GridLength.Auto),
             }
        };

        rootGrid.Add(GetHeader(message));
        rootGrid.Add(new ScrollView { Content = checkBoxGroup, VerticalOptions = LayoutOptions.Start, MaximumHeightRequest = currentPage.Height * 0.6, }, row: 1);
        rootGrid.Add(GetDivider(), row: 2);
        rootGrid.Add(GetFooter(
            accept, new Command(() =>
            {
                tcs.TrySetResult(checkBoxGroup.Children.Where(x => x is CheckBox checkbox && checkbox.IsChecked).Select(s => (T)(s as CheckBox).CommandParameter));
                currentPage.Navigation.PopModalAsync(animated: false);
            }),
            cancel, new Command(() =>
            {
                tcs.TrySetResult(null);
                currentPage.Navigation.PopModalAsync(animated: false);
            })
        ), row: 3);

        var popupPage = new ContentPage
        {
            BackgroundColor = GetBackdropColor(),
            Content = GetFrame(currentPage.Width, rootGrid)
        };

        currentPage.Navigation.PushModalAsync(ConfigurePopupPage(popupPage), animated: false);

        return tcs.Task;
    }

    public Task<T> DisplayRadioButtonPromptAsync<T>(string message,
        IEnumerable<T> selectionSource,
        T selected = default,
        string accept = "Ok",
        string cancel = "Cancel",
        string displayMember = null)
    {
        var tcs = new TaskCompletionSource<T>();

        var prop = displayMember != null ? typeof(T).GetProperty(displayMember) : null;

        var rbGroup = new RadioButtonGroupView()
        {
            Margin = 20,
            VerticalOptions = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.Start
        };

        foreach (var item in selectionSource)
        {
            rbGroup.Add(new InputKit.Shared.Controls.RadioButton
            {
                Text = prop != null ? prop.GetValue(item)?.ToString() : item.ToString(),
                Value = item,
            });
        }

        rbGroup.SelectedItem = selected;

        var rootGrid = new Grid
        {
            RowDefinitions = new RowDefinitionCollection
    {
        new RowDefinition(GridLength.Auto),
        new RowDefinition(GridLength.Star),
        new RowDefinition(GridLength.Auto),
        new RowDefinition(GridLength.Auto),
    }
        };

        var currentPage = GetCurrentPage();

        rootGrid.Add(GetHeader(message));
        rootGrid.Add(new ScrollView { Content = rbGroup, VerticalOptions = LayoutOptions.Start, MaximumHeightRequest = currentPage.Height * 0.6, }, row: 1);
        rootGrid.Add(GetDivider(), row: 2);
        rootGrid.Add(GetFooter(
            accept, new Command(() =>
            {
                tcs.TrySetResult((T)rbGroup.SelectedItem);
                currentPage.Navigation.PopModalAsync(animated: false);
            }),
            cancel, new Command(() =>
            {
                tcs.TrySetResult(default);
                currentPage.Navigation.PopModalAsync(animated: false);
            })
        ), row: 3);

        currentPage.Navigation.PushModalAsync(ConfigurePopupPage(new ContentPage
        {
            BackgroundColor = GetBackdropColor(),
            Content = GetFrame(currentPage.Width, rootGrid)
        }), animated: false);

        return tcs.Task;
    }

    public Task<string> DisplayTextPromptAsync(
        string title,
        string message,
        string accept = "OK",
        string cancel = "Cancel",
        string placeholder = null,
        int maxLength = -1,
        Keyboard keyboard = null,
        string initialValue = "")
    {
        var tcs = new TaskCompletionSource<string>();

        var entry = new EntryView
        {
            HorizontalOptions = LayoutOptions.Fill,
            Placeholder = placeholder,
            MaxLength = maxLength != -1 ? maxLength : int.MaxValue,
            ClearButtonVisibility = ClearButtonVisibility.WhileEditing,
            Keyboard = keyboard,
            TextColor = ColorResource.GetColor("OnBackground", "OnBackgroundDark", Colors.WhiteSmoke),
            PlaceholderColor = ColorResource.GetColor("Background", "BackgroundDark", Colors.Gray).WithAlpha(.5f),
            BackgroundColor = Colors.Transparent,
            Text = initialValue,
        };

        var entryholder = new Border
        {
            BackgroundColor = ColorResource.GetColor("OnSurface", "OnSurfaceDark", Colors.DarkGray).WithAlpha(.2f),
            StyleClass = new[] { "SurfaceContainer", "Rounded" },
            Margin = new Thickness(20, 0, 20, 20),
#if IOS
            Padding = new Thickness(5, 5),
#else
            Padding = new Thickness(5, 0),
#endif
            Content = entry
        };

        var currentPage = GetCurrentPage();

        var popupPage = new ContentPage
        {
            BackgroundColor = GetBackdropColor(),
            Content = GetFrame(currentPage.Width, new VerticalStackLayout
            {
                Children =
                {
                    GetHeader(title),
                    new Label
                    {
                        Text = message,
                        Margin = 20
                    },
                    entryholder,
                    GetDivider(),
                    GetFooter(
                        accept, new Command(()=>
                        {
                            tcs.TrySetResult(entry.Text);
                            currentPage.Navigation.PopModalAsync(animated: false);
                        }),
                        cancel, new Command(()=>
                        {
                            tcs.TrySetResult(initialValue);
                            currentPage.Navigation.PopModalAsync(animated: false);
                        }))
                }
            })
        };

        currentPage.Navigation.PushModalAsync(ConfigurePopupPage(popupPage), animated: false);

        return tcs.Task;
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

        return Application.Current.MainPage;
    }

    protected virtual Color GetBackdropColor()
    {
        return Application.Current.RequestedTheme switch
        {
            AppTheme.Light => Color.FromArgb("#80000000"),
            AppTheme.Dark => Color.FromArgb("#80ffffff"),
            _ => Color.FromArgb("#80808080")
        };
    }

    protected virtual Page ConfigurePopupPage(Page popupPage)
    {
#if IOS
        Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific.Page.SetModalPresentationStyle(
            popupPage.On<Microsoft.Maui.Controls.PlatformConfiguration.iOS>(),
            Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific.UIModalPresentationStyle.OverFullScreen
            );
#endif

        return popupPage;
    }

    protected virtual View GetFrame(double width, View content)
    {
        var options = UraniumServiceProvider.Current.GetRequiredService<IOptions<DialogOptions>>()?.Value;
        var desiredWidth = DeviceInfo.Idiom == DeviceIdiom.Desktop ? 400 : width * .8;
        var frame = new Border
        {
            StyleClass = new[] { "SurfaceContainer", "Rounded" },
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            Padding = 0,
            WidthRequest = desiredWidth,
            Content = content
        };

        foreach (var effectFactory in options.Effects)
        {
            frame.Effects.Add(effectFactory());
        }

        return frame;
    }
    protected virtual BoxView GetDivider()
    {
        return new BoxView { StyleClass = new[] { "Divider" }, Margin = 0, HeightRequest = 1 };
    }
    protected virtual View GetHeader(string title)
    {
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

    protected virtual View GetFooter(string accept, Command acceptCommand, string cancel, Command cancelCommand)
    {
        return new FlexLayout
        {
            JustifyContent = Microsoft.Maui.Layouts.FlexJustify.End,
            Margin = new Thickness(10),
            Children =
            {
                new Button
                {
                    Text = cancel,
                    StyleClass = new []{ "TextButton", "Dialog.Cancel" },
                    Command = cancelCommand
                },
                new Button
                {
                    Text = accept,
                    StyleClass = new []{ "TextButton", "Dialog.Accept" },
                    Command = acceptCommand
                }
            }
        };
    }
}
