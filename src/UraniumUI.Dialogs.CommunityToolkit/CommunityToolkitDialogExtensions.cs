using CommunityToolkit.Maui.Views;
using InputKit.Shared.Controls;
using Plainer.Maui.Controls;
using UraniumUI.Controls;
using UraniumUI.Infrastructure;
using UraniumUI.Resources;
using CheckBox = InputKit.Shared.Controls.CheckBox;

namespace UraniumUI.Dialogs.CommunityToolkit;
public static class CommunityToolkitDialogExtensions
{
    public static Task<bool> ConfirmAsync(
        this Page page,
        string title,
        string message,
        string accept = "OK",
        string cancel = "Cancel")
    {
       return GetService().WithPage(page)
           .ConfirmAsync(title, message, accept, cancel);
    }

    public static Task<IDisposable> DisplayProgressAsync(this Page page, string title, string message)
    {
        return GetService().WithPage(page)
            .DisplayProgressAsync(title, message);
    }

    public static Task<IDisposable> DisplayProgressCancellableAsync(this Page page, string title, string message, string cancelText = "Cancel", CancellationTokenSource tokenSource = null)
    {
        return GetService().WithPage(page)
            .DisplayProgressCancellableAsync(title, message, cancelText, tokenSource);  
    }

    public static Task<IEnumerable<T>> DisplayCheckBoxPromptAsync<T>(
        this Page page,
        string message,
        IEnumerable<T> selectionSource,
        IEnumerable<T> selectedItems = default,
        string accept = "OK",
        string cancel = "Cancel",
        string displayMember = null)
    {
        return GetService().WithPage(page)
            .DisplayCheckBoxPromptAsync(message, selectionSource, selectedItems, accept, cancel, displayMember);
    }

    public static Task<T> DisplayRadioButtonPromptAsync<T>(
        this Page page,
        string message,
        IEnumerable<T> selectionSource,
        T selected = default,
        string accept = "Ok",
        string cancel = "Cancel", string displayMember = null)
    {
        return GetService().WithPage(page)
            .DisplayRadioButtonPromptAsync(message, selectionSource, selected, accept, cancel, displayMember);
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
        string initialValue = "",
        bool isPassword = false)
    {
        return GetService().WithPage(page)
            .DisplayTextPromptAsync(title, message, accept, cancel, placeholder, maxLength, keyboard, initialValue, isPassword);
    }

    public static Task DisplayViewAsync(this Page page, string title, View content, string okText = "OK")
    {
        return GetService().WithPage(page)
            .DisplayViewAsync(title, content, okText);
    }

    public static Task<TViewModel> DisplayFormViewAsync<TViewModel>(this Page page, string title, TViewModel viewModel = null, string submit = "OK", string cancel = "Cancel") where TViewModel : class
    {
        return GetService().WithPage(page)
            .DisplayFormViewAsync(title, viewModel, submit, cancel);
    }

    private static CommunityToolkitDialogService GetService()
        => UraniumServiceProvider.Current.GetRequiredService<CommunityToolkitDialogService>();
}
