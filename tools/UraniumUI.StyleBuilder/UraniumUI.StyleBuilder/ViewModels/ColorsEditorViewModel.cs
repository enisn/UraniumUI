using Mopups.Services;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using UraniumUI.Dialogs;
using UraniumUI.StyleBuilder.Controls;
using UraniumUI.StyleBuilder.StyleManager;

namespace UraniumUI.StyleBuilder.ViewModels;
public class ColorsEditorViewModel : ReactiveObject, ISavable
{
    public ColorStyleManager ColorStyleManager { get; }
    protected IDialogService Dialog { get; }

    public ColorPalette Colors => ColorStyleManager?.Palette;

    [Reactive] public string Title { get; protected set; }

    public ColorsEditorViewModel(ColorStyleManager colorStyleManager, IDialogService dialog)
    {
        ColorStyleManager = colorStyleManager;
        Dialog = dialog;
        EditColorCommand = new Command(EditColorAsync);
    }
    public ICommand EditColorCommand { get; private set; }

    protected virtual async void EditColorAsync(object parameter)
    {
        await MopupService.Instance.PushAsync(new ColorEditPopupPage(this, parameter?.ToString()));
    }

    public async Task NewAsync()
    {
        await ColorStyleManager.CreateNewAsync();
    }

    public async Task LoadAsync(string path)
    {
        await ColorStyleManager.LoadAsync(path);
        Title = path;
    }

    public async Task SaveAsync()
    {
        if (string.IsNullOrEmpty(ColorStyleManager.Path))
        {
            await SaveAsAsync();
            return;
        }

        await ColorStyleManager.SaveAsync(ColorStyleManager.Path);
    }

    public async Task SaveAsAsync()
    {
        await ColorStyleManager.SaveAsAsync();
    }

    public override string ToString() => Title ?? "New Colors.xaml";
}