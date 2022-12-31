using System.Collections.ObjectModel;
using System.Windows.Input;
using UraniumUI;
using UraniumUI.Material.Controls;

namespace UraniumApp.ViewModels.TabViews;
public class WebTabViewModel : UraniumBindableObject
{
    public ObservableCollection<WebTabItem> TabItems { get; set; } = new()
    {
        new WebTabItem("https://www.bing.com/"),
        new WebTabItem("https://google.com/"),
        new WebTabItem("https://microsoft.com/"),
        new WebTabItem("https://github.com/"),
    };

    private object currentTab;
    public object CurrentTab { get => currentTab; set => SetProperty(ref currentTab, value); }
    public WebTabViewModel()
    {
        CreateNewTabCommand = new Command(CreateNewTab);
        RemoveTabCommand = new Command(RemoveTab);
    }

    public ICommand CreateNewTabCommand { get; set; }

    public ICommand RemoveTabCommand { get; set; }

    private void CreateNewTab()
    {
        var newTabItem = new WebTabItem("https://bing.com/");
        TabItems.Add(newTabItem);
        CurrentTab = newTabItem;
    }

    private void RemoveTab(object obj)
    {
        if (obj is WebTabItem tabItem)
        {
            TabItems.Remove(tabItem);
        }
    }
}

public class WebTabItem : UraniumBindableObject
{
    private string url;
    private string title;

    public WebTabItem(string url = null)
    {
        this.Url = url;
    }

    public string Url
    {
        get => url; set => SetProperty(ref url, value, doAfter: (_url) =>
        {
            if (Uri.TryCreate(_url, UriKind.RelativeOrAbsolute, out Uri uri))
            {
                Title = uri.Host;
            }
        });
    }

    public string Title { get => title; set => SetProperty(ref title, value); }

    public override string ToString()
    {
        return Title;
    }
}