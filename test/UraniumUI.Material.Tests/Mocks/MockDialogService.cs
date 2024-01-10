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

    public Task<T> DisplayRadioButtonPromptAsync<T>(string message, IEnumerable<T> selectionSource, T selected = default, string accept = "Ok", string cancel = "Cancel", string displayMember = null)
    {
        return Task.FromResult(default(T));
    }

    public Task<string> DisplayTextPromptAsync(string title, string message, string accept = "OK", string cancel = "Cancel", string placeholder = null, int maxLength = -1, Keyboard keyboard = null, string initialValue = "")
    {
        return Task.FromResult(string.Empty);
    }
}
