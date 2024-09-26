using DotNurse.Injector.Attributes;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Collections.ObjectModel;
using System.Windows.Input;
using UraniumApp.Pages;
using UraniumUI.Controls;
using UraniumUI.Material.Controls;

namespace UraniumApp.ViewModels;

[RegisterAs(typeof(MemoryLeakTestPrePageViewModel))]
public class MemoryLeakTestPrePageViewModel : ReactiveObject
{
    private readonly MemoryLeakDetectEvents _events;

    [Reactive] public Type[] ControlTypes { get; set; }

    public ObservableCollection<string> Outputs { get; } = new();

    [Reactive] public Type SelectedControlType { get; set; }

    public ICommand GoToTestCommand { get; }

    public ICommand ClearOutputsCommand { get; }

    public MemoryLeakTestPrePageViewModel(MemoryLeakDetectEvents events)
	{
        Task.Run(async () =>
        {
            await Task.Yield();

            var materialControls = typeof(TextField).Assembly.GetTypes().Where(IsAppropriateView);
            var uraniumControls = typeof(ExpanderView).Assembly.GetTypes().Where(IsAppropriateView);

            ControlTypes = materialControls
                .Union(uraniumControls)
                .Union(new[] { typeof(Label), typeof(Entry) })
                .ToArray();
        });

        _events = events;
        _events.OnLeaked += (sender, target) => Outputs.Add($"❌ Leaked: {target.Name}");
        _events.OnCollected += (sender, target) => Outputs.Add($"✅ Collected: {target.Name}");

        GoToTestCommand = new Command(() =>
        {
            if (SelectedControlType is null)
            {
                return;
            }

            Outputs.Clear();
            App.Current.MainPage.Navigation.PushModalAsync(new MemoryLeakTestPage(SelectedControlType));
        });

        ClearOutputsCommand = new Command(() =>
        {
            Outputs.Clear();
        });
    }

    private static bool IsAppropriateView(Type type)
    {
        return type.IsSubclassOf(typeof(View))
            && !type.IsAbstract
            && type.GetConstructors().Any(x => x.GetParameters().Length == 0);
    }
}
