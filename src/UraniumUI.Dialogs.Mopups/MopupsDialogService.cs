
using InputKit.Shared.Controls;
using Microsoft.Extensions.Options;
using Mopups.Pages;
using Mopups.Services;
using Plainer.Maui.Controls;
using UraniumUI.Controls;
using UraniumUI.Infrastructure;
using UraniumUI.Resources;
using CheckBox = InputKit.Shared.Controls.CheckBox;

namespace UraniumUI.Dialogs.Mopups;

public class MopupsDialogService : IDialogService
{
    protected DialogOptions DialogOptions { get; }

    public MopupsDialogService(IOptions<DialogOptions> dialogOptions)
    {
        DialogOptions = dialogOptions.Value;
    }

    public virtual async Task<bool> ConfirmAsync(string title, string message, string okText = "OK", string cancelText = "Cancel")
    {
        var tcs = new TaskCompletionSource<bool>();

        var popup = new PopupPage
        {
            BackgroundColor = DialogOptions.GetBackdropColor(),
            CloseWhenBackgroundIsClicked = false,
        };

        popup.Content = GetFrame(Page.Width, new VerticalStackLayout
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
                    GetFooter(new Dictionary<string, Command>
                    {
                        { okText, new Command(() =>
                        {
                            tcs.TrySetResult(true);
                            MopupService.Instance.RemovePageAsync(popup);
                        }) },
                        { cancelText, new Command(() =>
                        {
                            tcs.TrySetResult(false);
                            MopupService.Instance.RemovePageAsync(popup);
                        })}
                    })
                }
        });

        await MopupService.Instance.PushAsync(popup);

        return await tcs.Task;
    }

    public Task<IDisposable> DisplayProgressAsync(string title, string message)
    {
        return DisplayProgressCancellableAsync(title, message, null, null);
    }

    public async Task<IDisposable> DisplayProgressCancellableAsync(string title, string message, string cancelText = "Cancel", CancellationTokenSource tokenSource = null)
    {
        tokenSource ??= new CancellationTokenSource();

        var progress = new ActivityIndicator
        {
            IsRunning = true,
            IsVisible = true,
            HorizontalOptions = LayoutOptions.Center,
            Color = ColorResource.GetColor("Primary", "PrimaryDark", Colors.Blue),
            Margin = 20,
        };

        var verticalStackLayout = new VerticalStackLayout
        {
            Children =
            {
                GetHeader(title),
                new Label
                {
                    Text = message,
                    Margin = 20,
                },
                progress
            }
        };

        if (!string.IsNullOrEmpty(cancelText))
        {
            verticalStackLayout.Children.Add(GetDivider());
            verticalStackLayout.Children.Add(GetFooter(new Dictionary<string, Command>
            {
                { cancelText,  new Command(() => tokenSource?.Cancel()) }
            }));
        }

        var popupPage = new PopupPage
        {
            BackgroundColor = DialogOptions.GetBackdropColor(),
            CloseWhenBackgroundIsClicked = false,
            Content = GetFrame(Page.Width, verticalStackLayout)
        };

        await MopupService.Instance.PushAsync(popupPage);

        var cancelAction = new DisposableAction(() =>
        {
            MopupService.Instance.RemovePageAsync(popupPage);
        });

        tokenSource.Token.Register(cancelAction.Dispose);

        return cancelAction;
    }

    public virtual async Task<IEnumerable<T>> DisplayCheckBoxPromptAsync<T>(string message, IEnumerable<T> selectionSource, IEnumerable<T> selectedItems = null, string accept = "OK", string cancel = "Cancel", string displayMember = null)
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

        var popup = new PopupPage
        {
            BackgroundColor = DialogOptions.GetBackdropColor(),
            CloseWhenBackgroundIsClicked = false,
            Content = GetFrame(Page.Width, rootGrid)
        };

        rootGrid.Add(GetHeader(message));
        rootGrid.Add(new ScrollView { Content = checkBoxGroup, VerticalOptions = LayoutOptions.Start, MaximumHeightRequest = Page.Height * 0.6, }, row: 1);
        rootGrid.Add(GetDivider(), row: 2);
        rootGrid.Add(GetFooter( new Dictionary<string, Command>
        {
            { accept, new Command(() =>
            {
                tcs.TrySetResult(checkBoxGroup.Children.Where(x => x is CheckBox checkbox && checkbox.IsChecked).Select(s => (T)(s as CheckBox).CommandParameter));
                MopupService.Instance.RemovePageAsync(popup);
            }) },
            { cancel, new Command(() =>
            {
                tcs.TrySetResult(null);
                MopupService.Instance.RemovePageAsync(popup);
            }) } 
        }), row: 3);

        await MopupService.Instance.PushAsync(popup);

        return await tcs.Task;
    }

    public virtual async Task<T> DisplayRadioButtonPromptAsync<T>(string message, IEnumerable<T> selectionSource, T selected = default, string accept = "Ok", string cancel = "Cancel", string displayMember = null)
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

        var popup = new PopupPage
        {
            BackgroundColor = DialogOptions.GetBackdropColor(),
            CloseWhenBackgroundIsClicked = false,
            Content = GetFrame(Page.Width, rootGrid)
        };

        rootGrid.Add(GetHeader(message));
        rootGrid.Add(new ScrollView { Content = rbGroup, VerticalOptions = LayoutOptions.Start, MaximumHeightRequest = Page.Height * 0.6, }, row: 1);
        rootGrid.Add(GetDivider(), row: 2);
        rootGrid.Add(GetFooter(new Dictionary<string, Command>
        {
            { accept, new Command(() =>
            {
                tcs.TrySetResult((T)rbGroup.SelectedItem);
                MopupService.Instance.RemovePageAsync(popup);
            }) },
            { cancel, new Command(() =>
            {
                tcs.TrySetResult(default);
                MopupService.Instance.RemovePageAsync(popup);
            })
            }
        }), row: 3);

        await MopupService.Instance.PushAsync(popup);

        return await tcs.Task;
    }

    public virtual async Task<string> DisplayTextPromptAsync(string title, string message, string accept = "OK", string cancel = "Cancel", string placeholder = null, int maxLength = -1, Keyboard keyboard = null, string initialValue = "", bool isPassword = false)
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
            IsPassword = isPassword
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

        var popup = new PopupPage
        {
            BackgroundColor = DialogOptions.GetBackdropColor(),
            CloseWhenBackgroundIsClicked = false,
        };

        popup.Content = GetFrame(Page.Width, new VerticalStackLayout
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
                    GetFooter(new Dictionary<string, Command>
                    {
                        { accept, new Command(() =>
                        {
                            tcs.TrySetResult(entry.Text);
                            MopupService.Instance.RemovePageAsync(popup);
                        }) },
                        { cancel, new Command(() =>
                        {
                            tcs.TrySetResult(initialValue);
                            MopupService.Instance.RemovePageAsync(popup);
                        })
                        }
                    })
                }
        });
        await MopupService.Instance.PushAsync(popup);

        return await tcs.Task;
    }

    public virtual async Task<DateTime?> DisplayDatePromptAsync(
        string title,
        DateTime? selectedDate = null,
        DateTime? minimumDate = null,
        DateTime? maximumDate = null,
        string accept = "OK",
        string cancel = "Cancel",
        string clear = "Clear",
        string today = "Today")
    {
        var tcs = new TaskCompletionSource<DateTime?>();
        var originalSelectedDate = selectedDate;
        var normalizedSelectedDate = selectedDate?.Date;

        var calendarView = CreateDatePromptCalendar(normalizedSelectedDate, minimumDate, maximumDate);
        var popup = new PopupPage
        {
            BackgroundColor = DialogOptions.GetBackdropColor(),
            CloseWhenBackgroundIsClicked = false,
        };

        popup.Content = GetFrame(Page.Width, new VerticalStackLayout
        {
            Children =
            {
                GetHeader(title),
                new ScrollView
                {
                    Content = calendarView,
                    Margin = new Thickness(12, 16, 12, 0),
                    MaximumHeightRequest = Page.Height * .75,
                },
                GetDivider(),
                GetFooter(CreateDatePromptFooterButtons(
                    calendarView,
                    tcs,
                    originalSelectedDate,
                    accept,
                    cancel,
                    clear,
                    today,
                    () => MopupService.Instance.RemovePageAsync(popup)))
            }
        });

        await MopupService.Instance.PushAsync(popup);

        return await tcs.Task;
    }

    public virtual Task DisplayViewAsync(string title, View content, string okText = "OK")
    {
        var popup = new PopupPage
        {
            BackgroundColor = DialogOptions.GetBackdropColor(),
            CloseWhenBackgroundIsClicked = false,
        };

        popup.Content = GetFrame(Page.Width, new VerticalStackLayout
        {
            Children =
                {
                    GetHeader(title),
                    content,
                    GetDivider(),
                    new Button
                    {
                        Text = okText,
                        StyleClass = new []{ "TextButton", "Dialog.Button0" },
                        Command = new Command(() => MopupService.Instance.RemovePageAsync(popup))
                    }
                }
        });

        return MopupService.Instance.PushAsync(popup);
    }

    public Task<TViewModel> DisplayFormViewAsync<TViewModel>(string title, TViewModel viewModel = null, string submit = "OK", string cancel = "Cancel") where TViewModel : class
    {
        var tcs = new TaskCompletionSource<TViewModel>();
        var formView = new AutoFormView
        {
            Padding = 8,
            ShowSubmitButton = false,
            ShowResetButton = false,
            ShowMissingProperties = false,
            Source = viewModel ?? UraniumServiceProvider.Current.GetRequiredService<TViewModel>(),
        };

        var popup = new PopupPage
        {
            BackgroundColor = DialogOptions.GetBackdropColor(),
            CloseWhenBackgroundIsClicked = false,
        };

        popup.Content = GetFrame(Page.Width, new VerticalStackLayout
        {
            Children =
            {
                GetHeader(title),
                new ScrollView { Content = formView, MaximumHeightRequest = Page.Height * .8 },
                GetDivider(),
                GetFooter( new Dictionary<string, Command>
                {
                    { submit, new Command(() =>
                    {
                        formView.Submit();
                        if (formView.IsValidated)
                        {
                            tcs.TrySetResult(viewModel);
                            MopupService.Instance.RemovePageAsync(popup);
                        }
                    }) },
                    { cancel, new Command(() =>
                    {
                        tcs.TrySetResult(null);
                        MopupService.Instance.RemovePageAsync(popup);
                    }) }
                })
            }
        });

        MopupService.Instance.PushAsync(popup);

        return tcs.Task;
    }

    private static CalendarView CreateDatePromptCalendar(DateTime? selectedDate, DateTime? minimumDate, DateTime? maximumDate)
    {
        var displayDate = selectedDate ?? GetFallbackDisplayDate(minimumDate, maximumDate);

        return new CalendarView
        {
            SelectedDate = selectedDate,
            DisplayDate = displayDate,
            MinimumDate = minimumDate,
            MaximumDate = maximumDate,
            HorizontalOptions = LayoutOptions.Fill,
        };
    }

    private static DateTime GetFallbackDisplayDate(DateTime? minimumDate, DateTime? maximumDate)
    {
        var today = DateTime.Today;

        if ((!minimumDate.HasValue || today >= minimumDate.Value.Date)
            && (!maximumDate.HasValue || today <= maximumDate.Value.Date))
        {
            return today;
        }

        return minimumDate?.Date ?? maximumDate?.Date ?? today;
    }

    private static Dictionary<string, Command> CreateDatePromptFooterButtons(
        CalendarView calendarView,
        TaskCompletionSource<DateTime?> tcs,
        DateTime? selectedDate,
        string accept,
        string cancel,
        string clear,
        string today,
        Func<Task> close)
    {
        var footerButtons = new Dictionary<string, Command>
        {
            {
                accept, new Command(async () =>
                {
                    tcs.TrySetResult(calendarView.SelectedDate);
                    await close();
                })
            },
            {
                cancel, new Command(async () =>
                {
                    tcs.TrySetResult(selectedDate);
                    await close();
                })
            }
        };

        if (!string.IsNullOrEmpty(today))
        {
            footerButtons.Add(today, new Command(() => calendarView.TrySelectDate(DateTime.Today)));
        }

        if (!string.IsNullOrEmpty(clear))
        {
            footerButtons.Add(clear, new Command(async () =>
            {
                tcs.TrySetResult(null);
                await close();
            }));
        }

        return footerButtons;
    }

    public MopupsDialogService WithPage(Page page)
    {
        Page = page;
        return this;
    }

    private Page page;
    public Page Page { get => page ?? GetCurrentPage(); set => page = value; }

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
        if (Application.Current.MainPage is FlyoutPage flyout)
        {
            return flyout.Flyout;
        }
        return Application.Current.MainPage;
    }

    protected virtual View GetDivider()
    {
        if (DialogOptions.GetDivider != null)
        {
            return DialogOptions.GetDivider();
        }

        return new BoxView { StyleClass = new[] { "Divider" }, Margin = 0, HeightRequest = 1 };
    }

    protected virtual View GetFrame(double width, View content)
    {
        var options = UraniumServiceProvider.Current.GetRequiredService<IOptions<DialogOptions>>()?.Value;

        var frame = new Border
        {
            StyleClass = new[] { "SurfaceContainer", "Rounded" },
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            Padding = 0,
            WidthRequest = DeviceInfo.Idiom == DeviceIdiom.Desktop ? 400 : width * .8,
            Content = content
        };

        foreach (var effectFactory in options.Effects)
        {
            frame.Effects.Add(effectFactory());
        }

        return frame;
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
            AlignItems = Microsoft.Maui.Layouts.FlexAlignItems.Center,
            Wrap = Microsoft.Maui.Layouts.FlexWrap.Wrap,
            Margin = new Thickness(10),
        };

        if (footerButtons is null)
        {
            return layout;
        }

        foreach (var (text, command) in footerButtons.Reverse())
        {
            var button = new Button
            {
                Text = text,
                // Can be styled with StyleClass `Dialog.Button0`, `Dialog.Button1`, etc
                StyleClass = new[] { "TextButton", "Dialog.Button" + layout.Children.Count },
                Command = command
            };

            FlexLayout.SetShrink(button, 0);
            layout.Children.Add(button);
        }

        return layout;
    }
}
