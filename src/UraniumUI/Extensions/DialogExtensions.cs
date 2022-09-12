using CommunityToolkit.Maui.Views;
using InputKit.Shared.Controls;
using Microsoft.Maui.Controls.Shapes;
using System.Text.RegularExpressions;
using UraniumUI.Resources;
using CheckBox = InputKit.Shared.Controls.CheckBox;

namespace UraniumUI.Extensions;
public static class DialogExtensions
{
    public static Task<IEnumerable<T>> DisplayCheckBoxPromptAsync<T>(
        this Page page,
        string message,
        IEnumerable<T> selectionSource,
        IEnumerable<T> selectedItems = default,
        string accept = "OK",
        string cancel = "Cancel",
        string displayMember = null)
    {
        var tcs = new TaskCompletionSource<IEnumerable<T>>();
        var calculatedSize = CalculateSize(page);
        var rootContainer = new StackLayout();

#if IOS || MACCATALYST
        var popup = new Popup
        {
            Size = new Size(calculatedSize.Width, calculatedSize.Height),
            Color = ColorResource.GetColor("Surface", "SurfaceDark", Colors.Transparent),
            CanBeDismissedByTappingOutsideOfPopup = false,
            Content = rootContainer
        };
        rootContainer.HeightRequest = calculatedSize.Height;
#else
        var popup = new Popup()
        {
            Size = new Size(page.Width, page.Height),
            Color = Colors.Transparent,
            CanBeDismissedByTappingOutsideOfPopup = false,


            Content = new ContentView
            {
                BackgroundColor = Colors.Transparent,
                Content = new Frame
                {
                    Content = rootContainer,
                    CornerRadius = 20,
                    Padding = 0,
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Center,
                    WidthRequest = calculatedSize.Width,
                }
            }
        };
#endif

        var prop = displayMember != null ? typeof(T).GetProperty(displayMember) : null;

        var checkBoxGroup = new VerticalStackLayout
        {
            Margin = 20,
            VerticalOptions = LayoutOptions.Center,
        };

        foreach (var item in selectionSource)
        {
            checkBoxGroup.Add(new InputKit.Shared.Controls.CheckBox
            {
                Text = prop != null ? prop.GetValue(item)?.ToString() : item.ToString(),
                CommandParameter = item,
                IsChecked = selectedItems?.Contains(item) ?? false,
            });
        }

        View footer = GetFooter(
          accept,
          new Command(() =>
          {
              tcs.SetResult(checkBoxGroup.Children.Where(x => x is CheckBox checkbox && checkbox.IsChecked).Select(s => (T)(s as CheckBox).CommandParameter));
              popup.Close();
          }),
          cancel,
          new Command(() =>
          {
              tcs.SetResult(null);
              popup.Close();
          }));

        rootContainer.Add(GetHeader(message));
#if IOS || MACCATALYST
        rootContainer.Add(new ScrollView { Content = checkBoxGroup, VerticalOptions = LayoutOptions.Start, HorizontalScrollBarVisibility = ScrollBarVisibility.Always, MaximumHeightRequest = calculatedSize.Height - 120 });
#else
        rootContainer.Add(new ScrollView { Content = checkBoxGroup, VerticalOptions = LayoutOptions.Start, MaximumHeightRequest = calculatedSize.Height });
#endif
        rootContainer.Add(GetDivider());
        rootContainer.Add(footer);

        page.ShowPopup(popup);

        return tcs.Task;
    }

    public static Task<T> DisplayRadioButtonPromptAsync<T>(
        this Page page,
        string message,
        IEnumerable<T> selectionSource,
        T selected = default(T),
        string accept = "Ok",
        string cancel = "Cancel", string displayMember = null)
    {
        var tcs = new TaskCompletionSource<T>();
        var calculatedSize = CalculateSize(page);
        var rootContainer = new VerticalStackLayout();

#if IOS || MACCATALYST
        var popup = new Popup
        {
            Size = new Size(calculatedSize.Width, calculatedSize.Height),
            Color = ColorResource.GetColor("Surface", "SurfaceDark", Colors.Transparent),
            CanBeDismissedByTappingOutsideOfPopup = false,
            Content = rootContainer
        };
        rootContainer.HeightRequest = calculatedSize.Height;
#else

        var popup = new Popup()
        {
            Size = new Size(page.Width, page.Height),
            Color = Colors.Transparent,
            CanBeDismissedByTappingOutsideOfPopup = false,
            Content = new ContentView
            {
                BackgroundColor = Colors.Transparent,
                Content = new Frame
                {
                    Content = rootContainer,
                    CornerRadius = 20,
                    Padding = 0,
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Center,
                    WidthRequest = calculatedSize.Width,
                }
            }
        };
#endif
        var prop = displayMember != null ? typeof(T).GetProperty(displayMember) : null;

        var rbGroup = new RadioButtonGroupView()
        {
            Margin = 20,
            VerticalOptions = LayoutOptions.Center,
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

        View footer = GetFooter(
            accept,
            new Command(() =>
            {
                tcs.SetResult((T)rbGroup.SelectedItem);
                popup.Close();
            }),
            cancel,
            new Command(() =>
            {
                tcs.SetResult(default(T));
                popup.Close();
            }));

        rootContainer.Add(GetHeader(message));
#if IOS || MACCATALYST
        rootContainer.Add(new ScrollView { Content = rbGroup, VerticalOptions = LayoutOptions.Start, MaximumHeightRequest = calculatedSize.Height - 120 });
#else
        rootContainer.Add(new ScrollView { Content = rbGroup, VerticalOptions = LayoutOptions.Start, MaximumHeightRequest = calculatedSize.Height });
#endif
        rootContainer.Add(GetDivider());
        rootContainer.Add(footer);

        page.ShowPopup(popup);

        return tcs.Task;
    }

    public static Task<string> DisplayTextPromptAsync(
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
        var calculatedSize = CalculateSize(page);
        var rootContainer = new VerticalStackLayout();
#if IOS || MACCATALYST
        var popup = new Popup
        {
            Size = new Size(calculatedSize.Width, calculatedSize.Height),
            Color = ColorResource.GetColor("Surface", "SurfaceDark", Colors.Transparent),
            CanBeDismissedByTappingOutsideOfPopup = false,
            Content = rootContainer
        };
        rootContainer.HeightRequest = calculatedSize.Height;
#else
        var popup = new Popup()
        {
            Size = new Size(page.Width, page.Height),
            Color = Colors.Transparent,
            CanBeDismissedByTappingOutsideOfPopup = false,
            Content = new ContentView
            {
                BackgroundColor = Colors.Transparent,
                Content = new Frame
                {
                    Content = rootContainer,
                    CornerRadius = 20,
                    Padding = 0,
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Center,
                    WidthRequest = calculatedSize.Width,
                }
            }
        };
#endif
        var entry = new Entry
        {
            Placeholder = placeholder,
            MaxLength = maxLength != -1 ? maxLength : int.MaxValue,
            ClearButtonVisibility = ClearButtonVisibility.WhileEditing,
            Keyboard = keyboard,
            TextColor = ColorResource.GetColor("OnBackground", "OnBackgroundDark"),
            PlaceholderColor = ColorResource.GetColor("Background", "BackgroundDark").WithAlpha(.5f),
            BackgroundColor = Colors.Transparent,
            Text = initialValue,
        };

        var entryholder = new Frame
        {
            BackgroundColor = ColorResource.GetColor("OnSurface", "OnSurfaceDark").WithAlpha(.2f),
            HasShadow = false,
            CornerRadius = 4,
            Padding = new Thickness(5,0),
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

        View footer = GetFooter(
            accept,
        new Command(() =>
        {
                tcs.SetResult(entry.Text);
                popup.Close();
            }),
            cancel,
            new Command(() =>
            {
                tcs.SetResult(null);
                popup.Close();
            }));

        rootContainer.Add(GetHeader(title));
        rootContainer.Add(new ScrollView
        {
            Content = entryContainer,
            VerticalOptions = LayoutOptions.Start,
#if IOS || MACCATALYST
            MaximumHeightRequest = calculatedSize.Height / 2
#else
            MaximumHeightRequest = calculatedSize.Height
#endif
        });
        rootContainer.Add(GetDivider());
        rootContainer.Add(footer);

        page.ShowPopup(popup);

        return tcs.Task;
    }
    
    private static BoxView GetDivider()
    {
        return new BoxView { StyleClass = new[] { "Divider" }, Margin = 0 };
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

    private static Size CalculateSize(Page page)
    {
        if (DeviceInfo.Current.Idiom == DeviceIdiom.Desktop)
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
