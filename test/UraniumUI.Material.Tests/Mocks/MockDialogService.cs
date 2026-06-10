using UraniumUI.Dialogs;
using UraniumUI.Infrastructure;

namespace UraniumUI.Material.Tests.Mocks;
internal class MockDialogService : IDialogService
{
    public bool UseDatePromptResult { get; set; }

    public DateTime? DatePromptResult { get; set; }

    public int DatePromptCallCount { get; private set; }

    public string DatePromptTitle { get; private set; }

    public DateTime? DatePromptSelectedDate { get; private set; }

    public DateTime? DatePromptMinimumDate { get; private set; }

    public DateTime? DatePromptMaximumDate { get; private set; }

    public Task<bool> ConfirmAsync(string title, string message, string okText = "OK", string cancelText = "Cancel")
    {
        return Task.FromResult(default(bool));
    }

    public Task<IDisposable> DisplayProgressAsync(string title, string message)
    {
        return Task.FromResult<IDisposable>(new DisposableAction(() => { }));
    }

    public Task<IDisposable> DisplayProgressCancellableAsync(string title, string message, string cancelText = "Cancel", CancellationTokenSource tokenSource = null)
    {
        return Task.FromResult<IDisposable>(new DisposableAction(() => { }));
    }

    public Task<IEnumerable<T>> DisplayCheckBoxPromptAsync<T>(string message, IEnumerable<T> selectionSource, IEnumerable<T> selectedItems = null, string accept = "OK", string cancel = "Cancel", string displayMember = null)
    {
        return Task.FromResult(Enumerable.Empty<T>());
    }

    public Task<T> DisplayRadioButtonPromptAsync<T>(string message, IEnumerable<T> selectionSource, T selected = default, string accept = "Ok", string cancel = "Cancel", string displayMember = null)
    {
        return Task.FromResult(default(T));
    }

    public Task<string> DisplayTextPromptAsync(string title, string message, string accept = "OK", string cancel = "Cancel", string placeholder = null, int maxLength = -1, Keyboard keyboard = null, string initialValue = "")
    {
        return Task.FromResult(string.Empty);
    }

    public Task<string> DisplayTextPromptAsync(string title, string message, string accept = "OK", string cancel = "Cancel", string placeholder = null, int maxLength = -1, Keyboard keyboard = null, string initialValue = "", bool isPassword = false)
    {
        return Task.FromResult(string.Empty);
    }

    public Task<DateTime?> DisplayDatePromptAsync(
        string title,
        DateTime? selectedDate = null,
        DateTime? minimumDate = null,
        DateTime? maximumDate = null,
        string accept = "OK",
        string cancel = "Cancel",
        string clear = "Clear",
        string today = "Today")
    {
        DatePromptCallCount++;
        DatePromptTitle = title;
        DatePromptSelectedDate = selectedDate;
        DatePromptMinimumDate = minimumDate;
        DatePromptMaximumDate = maximumDate;

        return Task.FromResult(UseDatePromptResult ? DatePromptResult : selectedDate);
    }

    public Task DisplayViewAsync(string title, View content, string okText = "OK")
    {
        return Task.CompletedTask;
    }

    public Task<TViewModel> DisplayFormViewAsync<TViewModel>(string title, TViewModel viewModel = null, string submit = "OK", string cancel = "Cancel") where TViewModel : class
    {
        return Task.FromResult(default(TViewModel));
    }
}
