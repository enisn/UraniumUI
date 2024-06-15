using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Collections.ObjectModel;
using System.Net.Http.Json;
using System.Windows.Input;

namespace UraniumApp.ViewModels.DataGrid;

public class PaginationSampleViewModel : ReactiveObject
{
    public ObservableCollection<Product> Products { get; } = new();

    [Reactive] public bool IsBusy { get; set; }
    [Reactive] public int CurrentPage { get; set; }
    [Reactive] public int TotalPages { get; set; }

    public const int limit = 10;

    public ICommand GoNextCommand { get; }

    public ICommand GoPreviousCommand { get; }

    public ICommand SetPageCommand { get; }

    public PaginationSampleViewModel()
    {
        GoNextCommand = ReactiveCommand.Create(GoNext);
        GoPreviousCommand = ReactiveCommand.Create(GoPrevious);
        SetPageCommand = new Command<int>(SetPage);

        LoadFirstPage();

        this.
            WhenAnyValue(x => x.CurrentPage).
            Subscribe(async page => await LoadPageAsync(page));
    }

    void LoadFirstPage()
    {
        CurrentPage = 1;
    }

    public void GoNext()
    {
        CurrentPage++;
    }

    public void GoPrevious()
    {
        CurrentPage--;
    }

    public void SetPage(int page)
    {
        if (page >= 1 && page <= TotalPages)
        {
            CurrentPage = page;
        }
    }

    private async Task LoadPageAsync(int page)
    {
        IsBusy = true;

        var response = await GetProductsAsync(limit, (page - 1) * limit);

        IsBusy = false;

        TotalPages = (int)Math.Ceiling((double)response.total / limit);

        Products.Clear();

        foreach (var product in response.products)
        {
            Products.Add(product);
        }
    }

    async Task<ApiResponse> GetProductsAsync(int limit, int skip = 0)
    {
        using (var client = new HttpClient())
        {
            var response = await client.GetFromJsonAsync<ApiResponse>(
                $"https://dummyjson.com/products?limit={limit}&skip={skip}");

            return response;
        }
    }
}

public class ApiResponse
{
    public Product[] products { get; set; }
    public int total { get; set; }
    public int skip { get; set; }
    public int limit { get; set; }
}

public class Product
{
    public int id { get; set; }
    public string title { get; set; }
    public string description { get; set; }
    public int price { get; set; }
    public float discountPercentage { get; set; }
    public float rating { get; set; }
    public int stock { get; set; }
    public string brand { get; set; }
    public string category { get; set; }
    public string thumbnail { get; set; }
    public string[] images { get; set; }
}
