using DynamicData;
using DynamicData.Binding;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Windows.Input;
using UraniumUI.Dialogs;
using UraniumUI.StyleBuilder.StyleManager;
using UraniumUI.StyleBuilder.ViewModels;

namespace UraniumUI.StyleBuilder;

public class MainPageViewModel : ReactiveObject
{
    private readonly IServiceProvider serviceProvider;

    protected IDialogService Dialog { get; }

    public ObservableCollection<ISavable> Items { get; } = new ObservableCollection<ISavable>();

    [Reactive] public ISavable CurrentItem { get; set; }

    [ObservableAsProperty] public bool CanSave { get; }

    public MainPageViewModel(IDialogService dialog, IServiceProvider serviceProvider)
    {
        NewCommand = new Command(NewAsync);
        OpenCommand = new Command(OpenAsync);
        CloseCommand = ReactiveCommand.CreateFromTask<object>(CloseAsync);

        Dialog = dialog;
        this.serviceProvider = serviceProvider;

        this.WhenAnyValue(x => x.CurrentItem)
            .Select(x => x != null)
            .ToPropertyEx(this, x => x.CanSave);

        this.WhenAnyValue(x => x.CanSave)
            .Subscribe((_) => NotifyContextChanged());
    }

    private void NotifyContextChanged()
    {
        SaveCommand = new Command(SaveAsync, () => CurrentItem != null);
        SaveAsCommand = new Command(SaveAsAsync, () => CurrentItem != null);
    }

    public ICommand NewCommand { get; }
    public ICommand OpenCommand { get; }
    public ICommand CloseCommand { get; }
    [Reactive] public ICommand SaveCommand { get; private set; }
    [Reactive] public ICommand SaveAsCommand { get; private set; }

    protected virtual async void NewAsync()
    {
        try
        {
            var result = await Dialog.DisplayRadioButtonPromptAsync(
                 "Create New",
                 new[] { "Colors", "Styles" });

            if (result == null)
            {
                return;
            }

            if (result == "Colors")
            {
                var colorsEditorViewModel = serviceProvider.GetRequiredService<ColorsEditorViewModel>();

                await colorsEditorViewModel.NewAsync();

                Items.Add(colorsEditorViewModel);
                CurrentItem = colorsEditorViewModel;
            }
        }
        catch (Exception ex)
        {
            await App.Current.MainPage.DisplayAlert("Error", ex.ToString(), "OK");
        }
    }

    protected virtual async void OpenAsync()
    {
        try
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

            var existing = Items.FirstOrDefault(x => x.Path == fileResult.FullPath);
            if (existing != null)
            {
                CurrentItem = existing;
                return;
            }

            var colorsEditorViewModel = serviceProvider.GetRequiredService<ColorsEditorViewModel>();

            await colorsEditorViewModel.LoadAsync(fileResult.FullPath);
            Items.Add(colorsEditorViewModel);
            CurrentItem = colorsEditorViewModel;
        }
        catch (Exception ex)
        {
            await App.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
        }
    }

    protected virtual async void SaveAsync()
    {
        try
        {
            await CurrentItem?.SaveAsync();
        }
        catch (Exception ex)
        {
            await App.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
        }
    }

    protected virtual async void SaveAsAsync()
    {
        try
        {
            await CurrentItem?.SaveAsAsync();
        }
        catch (Exception ex)
        {
            await App.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
        }
    }

    protected virtual Task CloseAsync(object data)
    {
        if (data is ISavable item)
        {
            Items.Remove(item);
            item.Dispose();
        }

        return Task.CompletedTask;
    }

    public class StyleResourceFileType : FilePickerFileType
    {
        protected override IEnumerable<string> GetPlatformFileType(DevicePlatform platform)
        {
            yield return "xaml";
        }
    }
}