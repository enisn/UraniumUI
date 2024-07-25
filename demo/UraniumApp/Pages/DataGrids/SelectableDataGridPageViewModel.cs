using DotNurse.Injector.Attributes;
using ReactiveUI.Fody.Helpers;
using System.Collections.ObjectModel;
using System.Windows.Input;
using UraniumUI.Dialogs;

namespace UraniumApp.Pages.DataGrids;

[RegisterAs(typeof(SelectableDataGridPageViewModel))]
public class SelectableDataGridPageViewModel : BindableObject
{
    [Reactive] public ObservableCollection<Student> Items { get; private set; } = new();
    public ObservableCollection<Student> SelectedItems { get; set; } = new ObservableCollection<Student>();

    public ICommand RemoveSelectedCommand { get; set; }

    protected StudentDataStore DataStore { get; } = new StudentDataStore();

    public SelectableDataGridPageViewModel()
    {
        Initialize();

        RemoveSelectedCommand = new Command(() =>
        {
            for (int i = 0; i < SelectedItems.Count; i++)
            {
                Items.Remove(SelectedItems[i]);
            }
        });
    }

    protected async void Initialize()
    {
        Items = new ObservableCollection<Student>(
            await DataStore.GetListAsync(simulateNetwork: false));

        SelectedItems.Add(Items[0]);
        SelectedItems.Add(Items[2]);
    }
}
