using InputKit.Shared.Controls;

namespace UraniumUI.Dialogs;

public interface IDialogService
{
    Task DisplayViewAsync(
        string title,
        View content,
        string okText = "OK");

    Task<bool> ConfirmAsync(
        string title,
        string message,
        string okText = "OK",
        string cancelText = "Cancel");

    Task<IEnumerable<T>> DisplayCheckBoxPromptAsync<T>(
        string message,
        IEnumerable<T> selectionSource,
        IEnumerable<T> selectedItems = default,
        string accept = "OK",
        string cancel = "Cancel",
        string displayMember = null);

    Task<T> DisplayRadioButtonPromptAsync<T>(
        string message,
        IEnumerable<T> selectionSource,
        T selected = default(T),
        string accept = "Ok",
        string cancel = "Cancel", string displayMember = null);

    Task<string> DisplayTextPromptAsync(
        string title,
        string message,
        string accept = "OK",
        string cancel = "Cancel",
        string placeholder = null,
        int maxLength = -1,
        Keyboard keyboard = null,
        string initialValue = "",
        bool isPassword = false);

    Task DisplayFormViewAsync<TViewModel>(
        string title,
        TViewModel viewModel = default,
        string submit = "OK",
        string cancel = "Cancel",
        string reset = null) where TViewModel : class;
}
