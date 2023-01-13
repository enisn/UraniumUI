using Mopups.Pages;
using Mopups.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UraniumUI.Resources;

namespace UraniumUI;
public static class MopupsDialogExtensions
{
    public static async Task<bool> ConfirmAsync(this Page page, string title, string message, string okText = "OK", string cancelText = "Cancel")
    {
        var tcs = new TaskCompletionSource<bool>();

        await MopupService.Instance.PushAsync(new PopupPage
        {
            CloseWhenBackgroundIsClicked = false,
            Content = new Frame
            {
                CornerRadius = 20,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                Padding = 0,
                WidthRequest = DeviceInfo.Idiom == DeviceIdiom.Desktop ? 400 : page.Width * .8,
                Content = new VerticalStackLayout
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
                                tcs.SetResult(true);
                                MopupService.Instance.PopAsync();
                            }),
                            cancelText, new Command(()=>
                            {
                                tcs.SetResult(false);
                                MopupService.Instance.PopAsync();
                            }))
                    }
                }
            }
        });

        return await tcs.Task;
    }

    public static Task<IEnumerable<T>> DisplayCheckBoxPromptAsync<T>(string message, IEnumerable<T> selectionSource, IEnumerable<T> selectedItems = null, string accept = "OK", string cancel = "Cancel", string displayMember = null)
    {
        throw new NotImplementedException();
    }

    public static Task<T> DisplayRadioButtonPromptAsync<T>(string message, IEnumerable<T> selectionSource, T selected = default, string accept = "Ok", string cancel = "Cancel", string displayMember = null)
    {
        throw new NotImplementedException();
    }

    public static Task<string> DisplayTextPromptAsync(string title, string message, string accept = "OK", string cancel = "Cancel", string placeholder = null, int maxLength = -1, Keyboard keyboard = null, string initialValue = "")
    {
        throw new NotImplementedException();
    }

    private static BoxView GetDivider()
    {
        return new BoxView { StyleClass = new[] { "Divider" }, Margin = 0, HeightRequest = 1 };
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
