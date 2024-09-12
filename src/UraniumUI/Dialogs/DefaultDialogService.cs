using InputKit.Shared.Controls;
using Microsoft.Extensions.Options;
using Plainer.Maui.Controls;
using UraniumUI.Controls;
using UraniumUI.Infrastructure;
using UraniumUI.Resources;
using CheckBox = InputKit.Shared.Controls.CheckBox;

namespace UraniumUI.Dialogs;

public class DefaultDialogService : IDialogService
{
    public DialogOptions DialogOptions { get; }

    private Page page;

    public Page Page { get => page ?? GetCurrentPage(); set => page = value; }

    public DefaultDialogService(IOptions<DialogOptions> options)
    {
        DialogOptions = options.Value;
    }

    public DefaultDialogService WithPage(Page page)
    {
        Page = page;
        return this;
    }

    public Task DisplayViewAsync(string title, View content, string okText = "OK")
    {
        var tcs = new TaskCompletionSource<bool>();

        var popupPage = new ContentPage
        {
            BackgroundColor = GetBackdropColor(),
            Content = GetFrame(Page.Width, new VerticalStackLayout
            {
                Children =
                {
                    GetHeader(title),
                    content,
                    GetDivider(),
                    GetFooter(new Dictionary<string, Command>
                    {
                        {
                            okText, new Command(() =>
                            {
                                tcs.SetResult(true);
                                Page.Navigation.PopModalAsync(animated: false);
                            })
                        }
                    })
                }
            })
        };

        Page.Navigation.PushModalAsync(ConfigurePopupPage(popupPage), animated: false);

        return tcs.Task;
    }

    public Task<IDisposable> DisplayProgressAsync(string title, string message)
    {
        return DisplayProgressCancellableAsync(title, message, cancelText: null);
    }

    public async Task<IDisposable> DisplayProgressCancellableAsync(string title, string message, string cancelText = "Cancel", CancellationTokenSource tokenSource = default)
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
                { 
                    cancelText,  new Command(() =>
                    {
                        tokenSource?.Cancel();
                    })
                }
            }));
        }

        var popupPage = new ContentPage
        {
            BackgroundColor = GetBackdropColor(),
            Content = GetFrame(Page.Width, verticalStackLayout)
        };

        await Page.Navigation.PushModalAsync(ConfigurePopupPage(popupPage), animated: false);

        var cancelAction = new DisposableAction(() =>
        {
            if (Page.Navigation.ModalStack.LastOrDefault() == popupPage)
            {
                Page.Navigation.PopModalAsync(animated: false);
            }
        });

        tokenSource.Token.Register(cancelAction.Dispose);

        return cancelAction;
    }

    public async Task<bool> ConfirmAsync(string title, string message, string okText = "OK", string cancelText = "Cancel")
    {
        var tcs = new TaskCompletionSource<bool>();

        var popupPage = new ContentPage
        {
            BackgroundColor = GetBackdropColor(),
            Content = GetFrame(Page.Width, new VerticalStackLayout
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
                    GetFooter(new Dictionary<string, Command>
                    {
                        {
                            okText, new Command(() =>
                            {
                                tcs.SetResult(true);
                                Page.Navigation.PopModalAsync(animated: false);
                            })
                        },
                        {
                            cancelText, new Command(() =>
                            {
                                tcs.SetResult(false);
                                Page.Navigation.PopModalAsync(animated: false);
                            })
                        }
                    })
                }
            })
        };

        await Page.Navigation.PushModalAsync(ConfigurePopupPage(popupPage), animated: false);

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
        rootGrid.Add(new ScrollView { Content = checkBoxGroup, VerticalOptions = LayoutOptions.Start, MaximumHeightRequest = Page.Height * 0.6, }, row: 1);
        rootGrid.Add(GetDivider(), row: 2);
        rootGrid.Add(GetFooter(new Dictionary<string, Command>
        {
            {
                accept, new Command(() =>
                {
                    tcs.TrySetResult(checkBoxGroup.Children.Where(x => x is CheckBox checkbox && checkbox.IsChecked).Select(s => (T)(s as CheckBox).CommandParameter));
                    Page.Navigation.PopModalAsync(animated: false);
                })
            },
            {
                cancel, new Command(() =>
                {
                    tcs.TrySetResult(null);
                    Page.Navigation.PopModalAsync(animated: false);
                })
            }
        }
        ), row: 3);

        var popupPage = new ContentPage
        {
            BackgroundColor = GetBackdropColor(),
            Content = GetFrame(Page.Width, rootGrid)
        };

        Page.Navigation.PushModalAsync(ConfigurePopupPage(popupPage), animated: false);

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

        rootGrid.Add(GetHeader(message));
        rootGrid.Add(new ScrollView { Content = rbGroup, VerticalOptions = LayoutOptions.Start, MaximumHeightRequest = Page.Height * 0.6, }, row: 1);
        rootGrid.Add(GetDivider(), row: 2);
        rootGrid.Add(GetFooter(new Dictionary<string, Command>
        {
            {
                accept, new Command(() =>
                {
                    tcs.TrySetResult((T)rbGroup.SelectedItem);
                    Page.Navigation.PopModalAsync(animated: false);
                })
            },
            {
                cancel, new Command(() =>
                {
                    tcs.TrySetResult(default);
                    Page.Navigation.PopModalAsync(animated: false);
                })
            }
        }), row: 3);

        Page.Navigation.PushModalAsync(ConfigurePopupPage(new ContentPage
        {
            BackgroundColor = GetBackdropColor(),
            Content = GetFrame(Page.Width, rootGrid)
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
        string initialValue = "",
        bool isPassword = false)
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

        var popupPage = new ContentPage
        {
            BackgroundColor = GetBackdropColor(),
            Content = GetFrame(Page.Width, new VerticalStackLayout
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
                        {
                            accept, new Command(() =>
                            {
                                tcs.TrySetResult(entry.Text);
                                Page.Navigation.PopModalAsync(animated: false);
                            })
                        },
                        {
                            cancel, new Command(() =>
                            {
                                tcs.TrySetResult(initialValue);
                                Page.Navigation.PopModalAsync(animated: false);
                            })
                        }
                    })
                }
            })
        };

        Page.Navigation.PushModalAsync(ConfigurePopupPage(popupPage), animated: false);

        return tcs.Task;
    }

    public Task<TViewModel> DisplayFormViewAsync<TViewModel>(string title, TViewModel viewModel = null, string submit = "OK", string cancel = "Cancel") where TViewModel : class
    {
        var tcs = new TaskCompletionSource<TViewModel>();

        var formView = new AutoFormView()
        {
            Padding = 8,
            ShowSubmitButton = false,
            ShowResetButton = false,
            ShowMissingProperties = false,
            Source = viewModel ?? UraniumServiceProvider.Current.GetRequiredService<TViewModel>(),
        };

        var popupPage = new ContentPage
        {
            BackgroundColor = GetBackdropColor(),
            Content = GetFrame(Page.Width, new VerticalStackLayout
            {
                Children =
                {
                    GetHeader(title),
                    new ScrollView { Content = formView },
                    GetDivider(),
                    GetFooter(new Dictionary<string, Command>
                    {
                        {
                            submit, new Command(() =>
                            {
                                formView.Submit();
                                if (formView.IsValidated)
                                {
                                    Page.Navigation.PopModalAsync(animated: false);
                                    tcs.SetResult(viewModel);
                                }
                            })
                        },
                        {
                            cancel, new Command(() =>
                            {
                                Page.Navigation.PopModalAsync(animated: false);
                                tcs.SetResult(null);
                            })
                        }
                    })
                }
            })
        };

        Page.Navigation.PushModalAsync(ConfigurePopupPage(popupPage), animated: false);

        return tcs.Task;
    }

    protected virtual Color GetBackdropColor()
    {
        return DialogOptions.GetBackdropColor();
    }

    protected virtual Page ConfigurePopupPage(ContentPage popupPage)
    {
#if IOS
        Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific.Page.SetModalPresentationStyle(
            popupPage.On<Microsoft.Maui.Controls.PlatformConfiguration.iOS>(),
            Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific.UIModalPresentationStyle.OverFullScreen
            );
#endif
        popupPage.Content.Scale = .75f;
        popupPage.Content.Opacity = 0;

        popupPage.Content.ScaleTo(1, 360, Easing.Linear);
        popupPage.Content.FadeTo(1, 200, Easing.Linear);

        return popupPage;
    }

    protected virtual View GetFrame(double width, View content)
    {
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

        foreach (var effectFactory in DialogOptions.Effects)
        {
            frame.Effects.Add(effectFactory());
        }

        return frame;
    }

    protected virtual View GetDivider()
    {
        if (DialogOptions.GetDivider != null)
        {
            return DialogOptions.GetDivider();
        }

        return new BoxView { StyleClass = new[] { "Divider" }, Margin = 0, HeightRequest = 1 };
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

        if (footerButtons is null)
        {
            return layout;
        }

        foreach (var item in footerButtons.Reverse())
        {
            layout.Children.Add(new Button
            {
                Text = item.Key,
                // Can be styled with StyleClass `Dialog.Button0`, `Dialog.Button1`, etc
                StyleClass = new[] { "TextButton", "Dialog.Button" + layout.Children.Count },
                Command = item.Value
            });
        }

        return layout;
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
}
