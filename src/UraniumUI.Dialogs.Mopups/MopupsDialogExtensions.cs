using InputKit.Shared.Controls;
using Mopups.Pages;
using Mopups.Services;
using Plainer.Maui.Controls;
using UraniumUI.Resources;
using CheckBox = InputKit.Shared.Controls.CheckBox;

namespace UraniumUI.Dialogs.Mopups;
public static class MopupsDialogExtensions
{
    static readonly Color backdropColor = Colors.Black.WithAlpha(.6f);
    public static async Task<bool> ConfirmAsync(this Page page, string title, string message, string okText = "OK", string cancelText = "Cancel")
    {
        var tcs = new TaskCompletionSource<bool>();

        await MopupService.Instance.PushAsync(new PopupPage
        {
            BackgroundColor = backdropColor,
            CloseWhenBackgroundIsClicked = false,
            Content = GetFrame(page.Width, new VerticalStackLayout
            {
                Children =
                {
                    GetHeader(title),
                    new Label
                    {
                        Text = message,
                        Margin = 20
                    },
                    GetDivider(),
                    GetFooter(
                        okText, new Command(()=>
                        {
                            tcs.TrySetResult(true);
                            MopupService.Instance.PopAsync();
                        }),
                        cancelText, new Command(()=>
                        {
                            tcs.TrySetResult(false);
                            MopupService.Instance.PopAsync();
                        }))
                }
            })
        });

        return await tcs.Task;
    }

    public static async Task<IEnumerable<T>> DisplayCheckBoxPromptAsync<T>(
        this Page page,
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
        rootGrid.Add(new ScrollView { Content = checkBoxGroup, VerticalOptions = LayoutOptions.Start, MaximumHeightRequest = page.Height * 0.6, }, row: 1);
        rootGrid.Add(GetDivider(), row: 2);
        rootGrid.Add(GetFooter(
            accept, new Command(() =>
            {
                tcs.TrySetResult(checkBoxGroup.Children.Where(x => x is CheckBox checkbox && checkbox.IsChecked).Select(s => (T)(s as CheckBox).CommandParameter));
                MopupService.Instance.PopAsync();
            }),
            cancel, new Command(() =>
            {
                tcs.TrySetResult(null);
                MopupService.Instance.PopAsync();
            })
        ), row: 3);

        await MopupService.Instance.PushAsync(new PopupPage
        {
            BackgroundColor = backdropColor,
            CloseWhenBackgroundIsClicked = false,
            Content = GetFrame(page.Width, rootGrid)
        });

        return await tcs.Task;
    }

    public static async Task<T> DisplayRadioButtonPromptAsync<T>(
        this Page page,
        string message,
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
                IsChecked = item.Equals(selected),
            });
        }

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
        rootGrid.Add(new ScrollView { Content = rbGroup, VerticalOptions = LayoutOptions.Start, MaximumHeightRequest = page.Height * 0.6, }, row: 1);
        rootGrid.Add(GetDivider(), row: 2);
        rootGrid.Add(GetFooter(
            accept, new Command(() =>
            {
                tcs.TrySetResult((T)rbGroup.SelectedItem);
                MopupService.Instance.PopAsync();
            }),
            cancel, new Command(() =>
            {
                tcs.TrySetResult(default);
                MopupService.Instance.PopAsync();
            })
        ), row: 3);

        await MopupService.Instance.PushAsync(new PopupPage
        {
            BackgroundColor = backdropColor,
            CloseWhenBackgroundIsClicked = false,
            Content = GetFrame(page.Width, rootGrid)
        });

        return await tcs.Task;
    }

    public static async Task<string> DisplayTextPromptAsync(
        this Page page,
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

        var entryholder = new Frame
        {
            BackgroundColor = ColorResource.GetColor("OnSurface", "OnSurfaceDark", Colors.DarkGray).WithAlpha(.2f),
            HasShadow = false,
            CornerRadius = 4,
            Margin = new Thickness(20, 0, 20, 20),
#if IOS
            Padding = new Thickness(5, 5),
#else
            Padding = new Thickness(5, 0),
#endif
            Content = entry
        };

        await MopupService.Instance.PushAsync(new PopupPage
        {
            BackgroundColor = backdropColor,
            CloseWhenBackgroundIsClicked = false,
            Content = GetFrame(page.Width, new VerticalStackLayout
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
                            MopupService.Instance.PopAsync();
                        }),
                        cancel, new Command(()=>
                        {
                            tcs.TrySetResult(initialValue);
                            MopupService.Instance.PopAsync();
                        }))
                }
            })
        });

        return await tcs.Task;
    }

    private static BoxView GetDivider()
    {
        return new BoxView { StyleClass = new[] { "Divider" }, Margin = 0, HeightRequest = 1 };
    }

    private static View GetFrame(double width, View content)
    {
        return new Frame
        {
            CornerRadius = 20,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            Padding = 0,
            WidthRequest = DeviceInfo.Idiom == DeviceIdiom.Desktop ? 400 : width * .8,
            Content = content
        };
    }

    private static View GetHeader(string title)
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

    private static View GetFooter(string accept, Command acceptCommand, string cancel, Command cancelCommand)
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
                    StyleClass = new []{ "TextButton" },
                    Command = cancelCommand
                },
                new Button
                {
                    Text = accept,
                    StyleClass = new []{ "TextButton" },
                    Command = acceptCommand
                }
            }
        };
    }
}
