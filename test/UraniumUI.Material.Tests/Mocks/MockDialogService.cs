using UraniumUI.Dialogs;

namespace UraniumUI.Material.Tests.Mocks;
internal class MockDialogService : IDialogService
{
    public Task<bool> ConfirmAsync(string title, string message, string okText = "OK", string cancelText = "Cancel")
    {
        return Task.FromResult(default(bool));
    }

    public Task<IEnumerable<T>> DisplayCheckBoxPromptAsync<T>(string message, IEnumerable<T> selectionSource, IEnumerable<T> selectedItems = null, string accept = "OK", string cancel = "Cancel", string displayMember = null)
    {
        return Task.FromResult(Enumerable.Empty<T>());
    }

    public Task<T> DisplayRadioButtonPromptAsync<T>(string message, IEnumerable<T> selectionSource, T selected = default, string accept = "Ok", string cancel = "Cancel", string displayMember = null, bool isPassword = false)
    {
        return Task.FromResult(default(T));
    }

    public Task<T> DisplayRadioButtonPromptAsync<T>(string message, IEnumerable<T> selectionSource, T selected = default, string accept = "Ok", string cancel = "Cancel", string displayMember = null)
    {
        throw new NotImplementedException();
    }

    public Task<string> DisplayTextPromptAsync(string title, string message, string accept = "OK", string cancel = "Cancel", string placeholder = null, int maxLength = -1, Keyboard keyboard = null, string initialValue = "")
    {
        return Task.FromResult(string.Empty);
    }

    public Task<string> DisplayTextPromptAsync(string title, string message, string accept = "OK", string cancel = "Cancel", string placeholder = null, int maxLength = -1, Keyboard keyboard = null, string initialValue = "", bool isPassword = false)
    {
        return Task.FromResult(string.Empty);
    }

    public Task DisplayViewAsync(string title, View content, string okText = "OK")
    {
        return Task.CompletedTask;
    }

    public Task<TViewModel> DisplayFormViewAsync<TViewModel>(string title, TViewModel viewModel = null, string submit = "OK", string cancel = "Cancel", string reset = null) where TViewModel : class
    {
        return Task.FromResult(default(TViewModel));
    }
}
