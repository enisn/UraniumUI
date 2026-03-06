
using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Maui.Views;
using InputKit.Shared.Controls;
using Microsoft.Extensions.Options;
using Plainer.Maui.Controls;
using UraniumUI.Controls;
using UraniumUI.Infrastructure;
using UraniumUI.Resources;
using CheckBox = InputKit.Shared.Controls.CheckBox;

namespace UraniumUI.Dialogs.CommunityToolkit;

public class CommunityToolkitDialogService : CommunityToolkitDialogServiceBase, IDialogService
{
    public CommunityToolkitDialogService(IOptions<DialogOptions> dialogOptions) : base(dialogOptions)
    {
    }

    public CommunityToolkitDialogService WithPage(Page page)
    {
        Page = page;
        return this;
    }

    public Task<bool> ConfirmAsync(string title, string message, string accept = "OK", string cancelText = "Cancel")
    {
        var tcs = new TaskCompletionSource<bool>();

        var calculatedSize = CalculateSize(Page);
        var rootContainer = new VerticalStackLayout();

#if IOS || MACCATALYST
        var popup = new Popup
        {
            WidthRequest = calculatedSize.Width,
            HeightRequest = calculatedSize.Height,
            Padding = 0,
            BackgroundColor = ColorResource.GetColor("Surface", "SurfaceDark", Colors.Transparent),
            CanBeDismissedByTappingOutsideOfPopup = false,
            Content = rootContainer,
        };
        rootContainer.VerticalOptions = LayoutOptions.Center;
#else
        var popup = new Popup()
        {
            WidthRequest = calculatedSize.Width,
            HeightRequest = calculatedSize.Height,
            BackgroundColor = Colors.Transparent,
            Padding = 0,
            CanBeDismissedByTappingOutsideOfPopup = false,
            Content = new ContentView
            {
                BackgroundColor = Colors.Transparent,
                Content = GetFrame(calculatedSize.Width, rootContainer)
            }
        };
#endif

        var messageLabel = new Label
        {
            Text = message,
            Margin = 20,
        };

        var footer = GetFooter(new Dictionary<string, Command>
        {
            { accept, new Command(async () =>
            {
                tcs.SetResult(true);
                await popup.CloseAsync();
            }) },
            { cancelText, new Command(async () =>
            {
                tcs.SetResult(false);
                await popup.CloseAsync();
            } )}
        });

        rootContainer.Add(GetHeader(title));
        rootContainer.Add(new ScrollView
        {
            Content = messageLabel,
            VerticalOptions = LayoutOptions.Start,
#if IOS || MACCATALYST
            //MaximumHeightRequest = calculatedSize.Height
#else
            MaximumHeightRequest = calculatedSize.Height
#endif
        });
        rootContainer.Add(GetDivider());
        rootContainer.Add(footer);

        Page.ShowPopup(popup);

        return tcs.Task;
    }

    public Task<IDisposable> DisplayProgressAsync(string title, string message)
    {
        return DisplayProgressCancellableAsync(title, message, null, null);
    }

    public Task<IDisposable> DisplayProgressCancellableAsync(string title, string message, string cancelText = "Cancel", CancellationTokenSource tokenSource = null)
    {
        tokenSource ??= new CancellationTokenSource();
        var calculatedSize = CalculateSize(Page);

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
                {cancelText, new Command(() => tokenSource?.Cancel()) }
            }));
        }

        var popup = new Popup()
        {
            WidthRequest = calculatedSize.Width,
            HeightRequest = calculatedSize.Height,
            BackgroundColor = Colors.Transparent,
            Padding = 0,
            CanBeDismissedByTappingOutsideOfPopup = false,

            Content = new ContentView
            {
                BackgroundColor = Colors.Transparent,
                Content = GetFrame(calculatedSize.Width, verticalStackLayout)
            }
        };

        var cancelAction = new DisposableAction(async () => await popup.CloseAsync());
        tokenSource.Token.Register(cancelAction.Dispose);

        Page.ShowPopup(popup);

        return Task.FromResult<IDisposable>(cancelAction);
    }

    public Task<IEnumerable<T>> DisplayCheckBoxPromptAsync<T>(string message, IEnumerable<T> selectionSource, IEnumerable<T> selectedItems = null, string accept = "OK", string cancel = "Cancel", string displayMember = null, Color color = null)
    {
        var tcs = new TaskCompletionSource<IEnumerable<T>>();
        var calculatedSize = CalculateSize(Page);
#if IOS || MACCATALYST
        var rootContainer = new Grid()
        {
            HeightRequest = calculatedSize.Height
        };
        var popup = new Popup
        {
            WidthRequest = calculatedSize.Width,
            HeightRequest = calculatedSize.Height,
            BackgroundColor = ColorResource.GetColor("Surface", "SurfaceDark", Colors.Transparent),
            CanBeDismissedByTappingOutsideOfPopup = false,
            Padding = 0,
            Content = rootContainer
        };
        rootContainer.HeightRequest = calculatedSize.Height;
#else

        var rootContainer = new StackLayout();

        var popup = new Popup()
        {
            WidthRequest = calculatedSize.Width,
            HeightRequest = calculatedSize.Height,
            BackgroundColor = Colors.Transparent,
            Padding = 0,
            CanBeDismissedByTappingOutsideOfPopup = false,

            Content = new ContentView
            {
                BackgroundColor = Colors.Transparent,
                Content = GetFrame(calculatedSize.Width, rootContainer)
            }
        };
#endif

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
            var checkBox = new CheckBox
            {
                Text = prop != null ? prop.GetValue(item)?.ToString() : item.ToString(),
                CommandParameter = item,
                IsChecked = selectedItems?.Contains(item) ?? false,
            };
            if (color != null)
            {
                checkBox.Color = color;
            }
            checkBoxGroup.Add(checkBox);
        }

        var footer = GetFooter(new Dictionary<string, Command>
        {
            {accept, new Command(async () =>
            {
                tcs.SetResult(checkBoxGroup.Children.Where(x => x is CheckBox checkbox && checkbox.IsChecked).Select(s => (T)(s as CheckBox).CommandParameter));
                await popup.CloseAsync();
            }) },
            {cancel, new Command(async () =>
            {
                tcs.SetResult(null);
                await popup.CloseAsync();
            }) }
        });

#if IOS || MACCATALYST
        rootContainer.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
        rootContainer.RowDefinitions.Add(new RowDefinition(GridLength.Star));
        rootContainer.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
        rootContainer.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
        rootContainer.Add(GetHeader(message));
        rootContainer.Add(new ScrollView { Content = checkBoxGroup }, row: 1);
        rootContainer.Add(GetDivider(), row: 2);
        rootContainer.Add(footer, row: 3);
#else
        rootContainer.Add(GetHeader(message));
        rootContainer.Add(new ScrollView { Content = checkBoxGroup, VerticalOptions = LayoutOptions.Start, MaximumHeightRequest = calculatedSize.Height });
        rootContainer.Add(GetDivider());
        rootContainer.Add(footer);
#endif

        Page.ShowPopup(popup);

        return tcs.Task;
    }

    public Task<T> DisplayRadioButtonPromptAsync<T>(string message, IEnumerable<T> selectionSource, T selected = default, string accept = "Ok", string cancel = "Cancel", string displayMember = null)
    {
        var tcs = new TaskCompletionSource<T>();
        var calculatedSize = CalculateSize(Page);

#if IOS || MACCATALYST
        var rootContainer = new Grid()
        {
            HeightRequest = calculatedSize.Height
        };
        var popup = new Popup
        {
            WidthRequest = calculatedSize.Width,
            HeightRequest = calculatedSize.Height,
            BackgroundColor = ColorResource.GetColor("Surface", "SurfaceDark", Colors.Transparent),
            CanBeDismissedByTappingOutsideOfPopup = false,
            Padding = 0,
            Content = rootContainer
        };
        rootContainer.HeightRequest = calculatedSize.Height;
#else
        var rootContainer = new VerticalStackLayout();

        var popup = new Popup()
        {
            WidthRequest = calculatedSize.Width,
            HeightRequest = calculatedSize.Height,
            BackgroundColor = Colors.Transparent,
            Padding = 0,
            CanBeDismissedByTappingOutsideOfPopup = false,
            Content = new ContentView
            {
                BackgroundColor = Colors.Transparent,
                Content = GetFrame(calculatedSize.Width, rootContainer)
            }
        };
#endif
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

        var footer = GetFooter(new Dictionary<string, Command>
        {
            { accept, new Command(async () =>
            {
                tcs.SetResult((T)rbGroup.SelectedItem);
                await popup.CloseAsync();
            }) },
            { cancel, new Command(async () =>
            {
                tcs.SetResult(default);
                await popup.CloseAsync();
            }) }
        });

#if IOS || MACCATALYST
        rootContainer.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
        rootContainer.RowDefinitions.Add(new RowDefinition(GridLength.Star));
        rootContainer.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
        rootContainer.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
        rootContainer.Add(GetHeader(message));
        rootContainer.Add(new ScrollView { Content = rbGroup }, row: 1);
        rootContainer.Add(GetDivider(), row: 2);
        rootContainer.Add(footer, row: 3);
#else
        rootContainer.Add(GetHeader(message));
        rootContainer.Add(new ScrollView { Content = rbGroup, VerticalOptions = LayoutOptions.Start, MaximumHeightRequest = calculatedSize.Height });
        rootContainer.Add(GetDivider());
        rootContainer.Add(footer);
#endif
        Page.ShowPopup(popup);

        return tcs.Task;
    }

    public Task<string> DisplayTextPromptAsync(string title, string message, string accept = "OK", string cancel = "Cancel", string placeholder = null, int maxLength = -1, Keyboard keyboard = null, string initialValue = "", bool isPassword = false)
    {
        var tcs = new TaskCompletionSource<string>();
        var calculatedSize = CalculateSize(Page);
        var rootContainer = new VerticalStackLayout();

#if IOS || MACCATALYST
        var popup = new Popup
        {
            WidthRequest = calculatedSize.Width,
            HeightRequest = 230,
            BackgroundColor = ColorResource.GetColor("Surface", "SurfaceDark", Colors.Transparent),
            CanBeDismissedByTappingOutsideOfPopup = false,
            Padding = 0,
            Content = rootContainer,
        };
        rootContainer.VerticalOptions = LayoutOptions.Center;
#else
        var popup = new Popup()
        {
            WidthRequest = Page.Width,
            HeightRequest = Page.Height,
            BackgroundColor = Colors.Transparent,
            CanBeDismissedByTappingOutsideOfPopup = false,
            Content = new ContentView
            {
                BackgroundColor = Colors.Transparent,
                Content = GetFrame(calculatedSize.Width, rootContainer)
            }
        };
#endif
        var entry = new EntryView
        {
            HorizontalOptions = LayoutOptions.Fill,
            Placeholder = placeholder,
            MaxLength = maxLength != -1 ? maxLength : int.MaxValue,
            ClearButtonVisibility = ClearButtonVisibility.WhileEditing,
            Keyboard = keyboard,
            TextColor = ColorResource.GetColor("OnBackground", "OnBackgroundDark"),
            PlaceholderColor = ColorResource.GetColor("Background", "BackgroundDark").WithAlpha(.5f),
            BackgroundColor = Colors.Transparent,
            Text = initialValue,
            IsPassword = isPassword
        };

        var entryholder = new Border
        {
            BackgroundColor = ColorResource.GetColor("OnSurface", "OnSurfaceDark").WithAlpha(.2f),
            StyleClass = new[] { "SurfaceContainer", "Rounded" },
#if IOS
            Padding = new Thickness(15, 0),
#else
            Padding = new Thickness(5, 0),
#endif
            Content = entry
        };

        var entryContainer = new VerticalStackLayout
        {
            Margin = 20,
            Spacing = 10,
            Children =
            {
                new Label
                {
                    Text = message,
                },
                entryholder
            }
        };

        var footer = GetFooter(new Dictionary<string, Command>
        {
            { accept, new Command(async () =>
            {
                tcs.SetResult(entry.Text);
                await popup.CloseAsync();
            }) },
            { cancel, new Command(async () =>
            {
                tcs.SetResult(initialValue);
                await popup.CloseAsync();
            }) }
        });

        rootContainer.Add(GetHeader(title));
        rootContainer.Add(new ScrollView
        {
            Content = entryContainer,
            VerticalOptions = LayoutOptions.Start,
#if IOS || MACCATALYST
            //MaximumHeightRequest = calculatedSize.Height
#else
            MaximumHeightRequest = calculatedSize.Height
#endif
        });
        rootContainer.Add(GetDivider());
        rootContainer.Add(footer);

        Page.ShowPopup(popup);

        return tcs.Task;
    }

    public Task DisplayViewAsync(string title, View content, string okText = "OK")
    {
        var tcs = new TaskCompletionSource();
        var calculatedSize = CalculateSize(Page);
        var rootContainer = new VerticalStackLayout();

#if IOS || MACCATALYST
        var popup = new Popup
        {
            WidthRequest = calculatedSize.Width,
            HeightRequest = 230,
            BackgroundColor = ColorResource.GetColor("Surface", "SurfaceDark", Colors.Transparent),
            CanBeDismissedByTappingOutsideOfPopup = false,
            Padding = 0,
            Content = rootContainer,
        };
        rootContainer.VerticalOptions = LayoutOptions.Center;
#else
        var popup = new Popup()
        {
            WidthRequest = calculatedSize.Width,
            HeightRequest = calculatedSize.Height,
            BackgroundColor = Colors.Transparent,
            Padding = 0,
            CanBeDismissedByTappingOutsideOfPopup = false,
            Content = new ContentView
            {
                BackgroundColor = Colors.Transparent,
                Content = GetFrame(calculatedSize.Width, rootContainer)
            }
        };
#endif

        var footer = GetFooter(new Dictionary<string, Command>
        {
            { okText, new Command(async () =>
            {
                tcs.SetResult();
                await popup.CloseAsync();
            }) }
        });

        rootContainer.Add(GetHeader(title));
        rootContainer.Add(new ScrollView
        {
            Content = content,
            VerticalOptions = LayoutOptions.Start,
#if IOS || MACCATALYST
            //MaximumHeightRequest = calculatedSize.Height
#else
            MaximumHeightRequest = calculatedSize.Height
#endif
        });
        rootContainer.Add(GetDivider());
        rootContainer.Add(footer);

        Page.ShowPopup(popup);

        return tcs.Task;
    }

    public Task<TViewModel> DisplayFormViewAsync<TViewModel>(string title, TViewModel viewModel = null, string submit = "OK", string cancel = "Cancel") where TViewModel : class
    {
        var tcs = new TaskCompletionSource<TViewModel>();
        var calculatedSize = CalculateSize(Page);
        var rootContainer = new VerticalStackLayout();

        var formView = new AutoFormView
        {
            Padding = 8,
            ShowResetButton = false,
            ShowSubmitButton = false,
            ShowMissingProperties = false,
            Source = viewModel,
        };

#if IOS || MACCATALYST
        var popup = new Popup
        {
            WidthRequest = calculatedSize.Width,
            Padding = 0,
            HeightRequest = 230,
            BackgroundColor = ColorResource.GetColor("Surface", "SurfaceDark", Colors.Transparent),
            CanBeDismissedByTappingOutsideOfPopup = false,
            Content = rootContainer,
        };
        rootContainer.VerticalOptions = LayoutOptions.Center;
#else
        var popup = new Popup()
        {
            WidthRequest = calculatedSize.Width,
            HeightRequest = calculatedSize.Height,
            BackgroundColor = Colors.Transparent,
            Padding = 0,
            CanBeDismissedByTappingOutsideOfPopup = false,
            Content = new ContentView
            {
                BackgroundColor = Colors.Transparent,
                Content = GetFrame(calculatedSize.Width, rootContainer)
            }
        };
#endif

        var footer = GetFooter(new Dictionary<string, Command>
        {
            { submit, new Command(async () =>
            {
                tcs.TrySetResult(viewModel);
                await popup.CloseAsync();
            }) },
            { cancel, new Command(async () =>
            {
                tcs.TrySetResult(null);
                await popup.CloseAsync();
            }) }
        });

        rootContainer.Add(GetHeader(title));
        rootContainer.Add(new ScrollView
        {
            Content = formView,
            VerticalOptions = LayoutOptions.Start,
#if IOS || MACCATALYST
            //MaximumHeightRequest = calculatedSize.Height
#else
            MaximumHeightRequest = calculatedSize.Height
#endif
        });
        rootContainer.Add(GetDivider());
        rootContainer.Add(footer);

        Page.ShowPopup(popup);

        return tcs.Task;
    }
}
