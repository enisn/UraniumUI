using CommunityToolkit.Maui.Storage;
using DotNurse.Injector.Attributes;
using DynamicData.Binding;
using Mopups.Services;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Serialization;
using UraniumUI.Dialogs;
using UraniumUI.StyleBuilder.Controls;
using UraniumUI.StyleBuilder.StyleManager;

namespace UraniumUI.StyleBuilder;

public class MainPageViewModel : ReactiveObject
{
    public ColorStyleManager ColorStyleManager { get; }

    protected IDialogService Dialog { get; }

    public ColorPalette Colors => ColorStyleManager?.Palette;

    public MainPageViewModel(ColorStyleManager colorStyleManager, IDialogService dialog)
    {
        NewCommand = new Command(NewAsync);
        OpenCommand = new Command(OpenAsync);
        NotifyContextChanged();
        EditColorCommand = new Command(EditColorAsync);
        ColorStyleManager = colorStyleManager;
        Dialog = dialog;
    }

    private void NotifyContextChanged()
    {
        if (ColorStyleManager != null)
        {
            this.RaisePropertyChanged(nameof(Colors));
        }
        SaveCommand = new Command(SaveAsync, () => Colors != null);
        SaveAsCommand = new Command(SaveAsAsync, () => Colors != null);
    }

    public ICommand NewCommand { get; }
    public ICommand OpenCommand { get; }
    [Reactive] public ICommand SaveCommand { get; private set; }
    [Reactive] public ICommand SaveAsCommand { get; private set; }
    public ICommand EditColorCommand { get; private set; }

    protected virtual async void NewAsync()
    {
        if (ColorStyleManager.IsLoaded)
        {
            if (!await Dialog.ConfirmAsync("Creating new!", "Unsaved Data will be lost if you continue", "Continue"))
            {
                return;
            }
        }

        await ColorStyleManager.CreateNewAsync();

        NotifyContextChanged();
    }

    protected virtual async void OpenAsync()
    {
        var fileResult = await FilePicker.Default.PickAsync(new PickOptions
        {
            FileTypes = new StyleResourceFileType(),
            PickerTitle = "Pick an xml file"
        });

        if (fileResult == null)
        {
            return;
        }

        await ColorStyleManager.LoadAsync(fileResult.FullPath);

        NotifyContextChanged();
    }

    protected virtual async void SaveAsync()
    {
        if (string.IsNullOrEmpty(ColorStyleManager.Path))
        {
            SaveAsAsync();
            return;
        }

        await ColorStyleManager.SaveAsync(ColorStyleManager.Path);
    }

    protected virtual async void SaveAsAsync()
    {
        await ColorStyleManager.SaveAsAsync();
    }

    protected virtual async void EditColorAsync(object parameter)
    {
        await MopupService.Instance.PushAsync(new ColorEditPopupPage(this, parameter?.ToString()));
    }

    public class StyleResourceFileType : FilePickerFileType
    {
        protected override IEnumerable<string> GetPlatformFileType(DevicePlatform platform)
        {
            yield return "xaml";
        }
    }
}