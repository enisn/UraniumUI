using System.Collections.ObjectModel;
using System.Windows.Input;
using UraniumUI;
using UraniumUI.Material.Controls;

namespace UraniumApp.ViewModels.TabViews;
public class WebTabViewModel : UraniumBindableObject
{

	public ObservableCollection<TabItem> TabItems { get; set; } = new()
	{
		new WebTabItem("https://www.bing.com/"),
		new WebTabItem("https://google.com/"),
		new WebTabItem("https://microsoft.com/"),
		new WebTabItem("https://github.com/"),
	};

	private TabItem currentTab;
	public TabItem CurrentTab { get => currentTab; set => SetProperty(ref currentTab, value); }
	public WebTabViewModel()
	{
		CreateNewTabCommand = new Command(CreateNewTab);
	}

	public ICommand CreateNewTabCommand { get; set; }

	private void CreateNewTab()
	{
		var newTabItem = new WebTabItem("https://bing.com/");
		TabItems.Add(newTabItem);
		CurrentTab = newTabItem;
	}
}

public class WebTabItem : TabItem
{
	public WebTabItem(string url = "https://www.bing.com/")
	{
		this.Title = new Uri(url).Host;
		this.ContentTemplate = new DataTemplate(() =>
		{
			var webView = new WebView
			{
				Source = url,
				VerticalOptions = LayoutOptions.Fill
			};

			return webView;
		});
	}
}