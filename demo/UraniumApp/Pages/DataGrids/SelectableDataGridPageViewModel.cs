using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace UraniumApp.Pages.DataGrids;
public class SelectableDataGridPageViewModel : CustomDataGridPageViewModel
{
    public ObservableCollection<Student> SelectedItems { get; set; } = new ObservableCollection<Student>();

    public ICommand RemoveSelectedCommand { get; set; }

    public SelectableDataGridPageViewModel() : base()
    {
        RemoveSelectedCommand = new Command(() =>
        {
            for (int i = 0; i < SelectedItems.Count; i++)
            {
                Items.Remove(SelectedItems[i]);
            }
        });
    }

    protected override async void Initialize()
    {
        Items = new ObservableCollection<Student>(await DataStore.GetListAsync(simulateNetwork: false));

        SelectedItems.Add(Items[0]);
        SelectedItems.Add(Items[2]);
    }
}
