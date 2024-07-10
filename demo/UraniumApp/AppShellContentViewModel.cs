using DynamicData;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using UraniumUI.ViewExtensions;

namespace UraniumApp;
public class AppShellContentViewModel : ReactiveObject
{
    private Shell currentShell;

    public Shell CurrentShell
    {
        get => currentShell; set
        {
            currentShell = value;
            OnCurrentShellChanged();
        }
    }

    void OnCurrentShellChanged()
    {
        CurrentShell.Loaded += CurrentShell_Loaded;
    }

    private void CurrentShell_Loaded(object sender, EventArgs e)
    {
        var observableFilter = this.WhenAnyValue(x => x.SearchText)
          .Throttle(TimeSpan.FromMilliseconds(200))
          .Select(searchText => (Func<ShellItem, bool>)(item => string.IsNullOrEmpty(SearchText) || item.Title.Contains(SearchText, StringComparison.OrdinalIgnoreCase)));
      
        CurrentShell.Items.AsObservableChangeSet()
            .Filter(observableFilter)
            //.GroupOn(x => x.GetId() ?? "Others")
            //.Transform(group => new ShellItemGroup(group.GroupKey, group.List.Items.ToList()))
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out var _shellItems)
            .Subscribe();

        Items = _shellItems;

        SelectedItem = CurrentShell.CurrentItem;

        this.WhenAnyValue(x => x.SelectedItem)
            .WhereNotNull()
            .Subscribe(shellItem =>
            {
                CurrentShell.CurrentItem = shellItem;
                if (CurrentShell.FlyoutBehavior == FlyoutBehavior.Flyout)
                {
                    CurrentShell.FlyoutIsPresented = false;
                }
            });
    }

    [Reactive] public string SearchText { get; set; }

    [Reactive] public ReadOnlyObservableCollection<ShellItem> Items { get; private set; }

    [Reactive] public ShellItem SelectedItem { get; set; }
}

//public class ShellItemGroup : List<ShellItem>
//{
//    public string Name { get; private set; }

//    public ShellItemGroup(string name, List<ShellItem> items) : base(items)
//    {
//        Name = name;
//    }
//}