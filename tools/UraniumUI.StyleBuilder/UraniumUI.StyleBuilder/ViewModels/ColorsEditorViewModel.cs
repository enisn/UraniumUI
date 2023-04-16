using Mopups.Services;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Reactive.Linq;
using System.Windows.Input;
using UraniumUI.Dialogs;
using UraniumUI.StyleBuilder.Controls;
using UraniumUI.StyleBuilder.StyleManager;

namespace UraniumUI.StyleBuilder.ViewModels;
public class ColorsEditorViewModel : ReactiveObject, ISavable
{
    public ColorStyleManager ColorStyleManager { get; private set; }

    protected IDialogService Dialog { get; }

    public ColorPalette Colors => ColorStyleManager?.Palette;

    [Reactive] public string Title { get; protected set; }
    [ObservableAsProperty] public string Path { get; }

    public ColorsEditorViewModel(ColorStyleManager colorStyleManager, IDialogService dialog)
    {
        ColorStyleManager = colorStyleManager;
        Dialog = dialog;
        EditColorCommand = new Command(EditColorAsync);
        this.ColorStyleManager
            .WhenAnyValue(x => x.Path)
            .ToPropertyEx(this, x => x.Path);
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
        Title = path;
        await ColorStyleManager.LoadAsync(path);
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

    public void Dispose()
    {
        ColorStyleManager?.Dispose();
        ColorStyleManager = null;
    }
}