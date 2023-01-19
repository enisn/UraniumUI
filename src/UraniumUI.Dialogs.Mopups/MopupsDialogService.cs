using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UraniumUI.Dialogs;

namespace UraniumUI.Dialogs.Mopups;

public class MopupsDialogService : IDialogService
{
    public virtual Task<bool> ConfirmAsync(string title, string message, string okText = "OK", string cancelText = "Cancel")
    {
        return GetCurrentPage().ConfirmAsync(title, message, okText, cancelText);
    }

    public virtual Task<IEnumerable<T>> DisplayCheckBoxPromptAsync<T>(string message, IEnumerable<T> selectionSource, IEnumerable<T> selectedItems = null, string accept = "OK", string cancel = "Cancel", string displayMember = null)
    {
        return GetCurrentPage().DisplayCheckBoxPromptAsync(message, selectionSource, selectedItems, accept, cancel, displayMember);
    }

    public virtual Task<T> DisplayRadioButtonPromptAsync<T>(string message, IEnumerable<T> selectionSource, T selected = default, string accept = "Ok", string cancel = "Cancel", string displayMember = null)
    {
        return GetCurrentPage().DisplayRadioButtonPromptAsync(message, selectionSource, selected, accept, cancel, displayMember);
    }

    public virtual Task<string> DisplayTextPromptAsync(string title, string message, string accept = "OK", string cancel = "Cancel", string placeholder = null, int maxLength = -1, Keyboard keyboard = null, string initialValue = "")
    {
        return GetCurrentPage().DisplayTextPromptAsync(title, message, accept, cancel, placeholder, maxLength, keyboard, initialValue);
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
