using Mopups.Services;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Windows.Input;
using UraniumUI.Dialogs;
using UraniumUI.StyleBuilder.Controls;
using UraniumUI.StyleBuilder.StyleManager;

namespace UraniumUI.StyleBuilder.ViewModels;
public class ColorsEditorViewModel : ReactiveObject, ISavable
{
    public ColorStyleManager ColorStyleManager { get; private set; }

    protected IDialogService Dialog { get; }

    [Reactive] public string Title { get; protected set; }

    [ObservableAsProperty] public string Path { get; }

    public IDictionary<string, ReactiveColor> Colors => ColorStyleManager?.Colors;

    public ColorsEditorViewModel(ColorStyleManager colorStyleManager, IDialogService dialog)
    {
        ColorStyleManager = colorStyleManager;
        Dialog = dialog;
        EditColorCommand = new Command(EditColorAsync);
        this.ColorStyleManager
            .WhenAnyValue(x => x.Path)
            .ToPropertyEx(this, x => x.Path);
    }
    public ICommand EditColorCommand { get; }

    protected virtual async void EditColorAsync(object parameter)
    {
        await MopupService.Instance.PushAsync(new ColorEditPopupPage(this, parameter?.ToString()));
    }

    public async Task<bool> NewAsync()
    {
        var result = await Dialog.DisplayRadioButtonPromptAsync(
                "Choose a color palette to based on...",
                new[] { "Uranium Colors", "Material Colors" },
                "Uranium Colors");

        if (result == null)
        {
            return false;
        }

        if (result == "Material Colors")
        {
            using var stream = await FileSystem.OpenAppPackageFileAsync("MaterialColors.xml");
            await ColorStyleManager.CreateNewAsync(stream);

            return true;
        }

        await ColorStyleManager.CreateNewAsync();

        return true;
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