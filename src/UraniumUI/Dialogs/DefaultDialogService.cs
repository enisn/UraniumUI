using InputKit.Shared.Controls;

namespace UraniumUI.Dialogs;
public class DefaultDialogService : IDialogService
{
    public Task<bool> ConfirmAsync(string title, string message, string okText = "OK", string cancelText = "Cancel")
    {
        return Application.Current.MainPage.DisplayAlert(title, message, okText, cancelText);
    }

    public Task<IEnumerable<T>> DisplayCheckBoxPromptAsync<T>(
        string message,
        IEnumerable<T> selectionSource,
        IEnumerable<T> selectedItems = null,
        string accept = "OK",
        string cancel = "Cancel",
        string displayMember = null)
    {
        var tcs = new TaskCompletionSource<IEnumerable<T>>();

        var selectionView = new SelectionView
        {
            ColumnNumber = 1,
            SelectionType = InputKit.Shared.SelectionType.CheckBox,
            ItemsSource = selectionSource.ToList(),
        };

        Application.Current.MainPage.Navigation.PushModalAsync(new ContentPage
        {
            Content = new ScrollView
            {
                Content = new VerticalStackLayout
                {
                    HorizontalOptions = LayoutOptions.Center,
                    Children = {
                        new Label{ Text = message },
                        selectionView,
                        new HorizontalStackLayout
                        {
                            Children =
                            {
                                new Button
                                {
                                    Text = accept,
                                    Command = new Command(() =>
                                    {
                                        tcs.SetResult(selectionView.SelectedItems.Cast<T>());
                                        Application.Current.MainPage.Navigation.PopModalAsync();
                                    })
                                },
                                new Button
                                {
                                    Text = cancel,
                                    Command = new Command(() =>
                                    {
                                        tcs.SetResult(null);
                                        Application.Current.MainPage.Navigation.PopModalAsync();
                                    })
                                }
                            }
                        }
                    }
                }
            }
        });

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

        var selectionView = new SelectionView
        {
            ColumnNumber = 1,
            SelectionType = InputKit.Shared.SelectionType.RadioButton,
            ItemsSource = selectionSource.ToList(),
        };

        selectionView.SelectedItem = selected;

        Application.Current.MainPage.Navigation.PushModalAsync(new ContentPage
        {
            Content = new ScrollView
            {
                Content = new VerticalStackLayout
                {
                    HorizontalOptions = LayoutOptions.Center,
                    Children = {
                        new Label{ Text = message },
                        selectionView,
                        new HorizontalStackLayout
                        {
                            Children =
                            {
                                new Button
                                {
                                    Text = accept,
                                    Command = new Command(() =>
                                    {
                                        tcs.SetResult((T)(selectionView.SelectedItem ?? default(T)));
                                        Application.Current.MainPage.Navigation.PopModalAsync();
                                    })
                                },
                                new Button
                                {
                                    Text = cancel,
                                    Command = new Command(() =>
                                    {
                                        tcs.SetResult(default(T));
                                        Application.Current.MainPage.Navigation.PopModalAsync();
                                    })
                                }
                            }
                        }
                    }
                }
            }
        });

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
        return Application.Current.MainPage.DisplayPromptAsync(title, message, accept, cancel, placeholder, maxLength, keyboard, initialValue);
    }
}
