using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UraniumUI.Dialogs;
using UraniumUI.StyleBuilder.StyleManager;

namespace UraniumUI.StyleBuilder.ViewModels;
public class StyleEditorViewModel : ReactiveObject, ISavable, ILoadable
{
    public ResourceStyleManager ResourceStyleManager { get; }
    protected IDialogService Dialog { get; }
    [Reactive] public string Title { get; set; }
    [ObservableAsProperty] public string Path { get; }

    public StyleEditorViewModel(ResourceStyleManager resourceStyleManager, IDialogService dialog)
    {
        ResourceStyleManager = resourceStyleManager;
        Dialog = dialog;
    }

    public void Dispose()
    {

    }

    public Task LoadAsync(string path)
    {
        return ResourceStyleManager.LoadAsync(path);
    }

    public Task SaveAsAsync()
    {
        throw new NotImplementedException();
    }

    public Task SaveAsync()
    {
        throw new NotImplementedException();
    }
}
