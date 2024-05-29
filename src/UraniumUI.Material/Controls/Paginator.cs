using UraniumUI.Extensions;
using UraniumUI.ViewExtensions;

namespace UraniumUI.Material.Controls;

public partial class Paginator : ContentView
{
    public const string ViewIdPrefix = "paginator-btn";
    public bool CanGoNext { get => canGoNext; set { canGoNext = value; OnPropertyChanged(); } }
    public bool CanGoPrevious { get => canGoPrevious; set { canGoPrevious = value; OnPropertyChanged(); } }

    private HorizontalStackLayout PagesStackLayout => this.FindByViewQueryId<HorizontalStackLayout>(nameof(PagesStackLayout));
    private bool canGoNext;
    private bool canGoPrevious;

    public Paginator()
    {
        var stackLayout = new HorizontalStackLayout();
        var previousButton = CreatePreviousPageButton();

        var pagesStackLayout = new HorizontalStackLayout();
        pagesStackLayout.SetId(nameof(PagesStackLayout));

        BindableLayout.SetItemsSource(pagesStackLayout, Enumerable.Range(1, TotalPageCount));
        BindableLayout.SetItemTemplate(pagesStackLayout, new DataTemplate(() =>
        {
            var btn = new Button
            {
                StyleClass = new[] { "TextButton" }
            };

            ViewQuery.SetId(btn, ViewIdPrefix);
            btn.SetBinding(Button.TextProperty, new Binding("."));
            btn.SetBinding(Button.CommandProperty, new Binding(nameof(ChangePageCommand), source: this));
            btn.SetBinding(Button.CommandParameterProperty, new Binding("."));

            return btn;
        }));
        var nextButton = CreateNextPageButton();

        if (ShowFirstAndLastPageButtons)
        {
            stackLayout.Add(CreateFirstPageButton());
        }

        stackLayout.Children.AddIf(previousButton, ShowPreviousAndNextPageButtons);
        stackLayout.Children.Add(pagesStackLayout);
        stackLayout.Children.AddIf(nextButton, ShowPreviousAndNextPageButtons);
        
        if (ShowFirstAndLastPageButtons)
        {
            stackLayout.Add(CreateLastPageButton());
        }

        Content = stackLayout;
    }

    private View CreateLastPageButton()
    {
        var lastPageButton = (View)LastPageButtonTemplate?.CreateContent() ?? new Button
        {
            Text = ">>",
            StyleClass = new[] { "TextButton" },
        };
        lastPageButton.SetBinding(Button.CommandProperty, new Binding(nameof(ChangePageCommand), source: this));
        lastPageButton.SetBinding(Button.CommandParameterProperty, new Binding(nameof(TotalPageCount), source: this));
        lastPageButton.SetBinding(Button.IsEnabledProperty, new Binding(nameof(CanGoNext), source: this));
        return lastPageButton;
    }

    private View CreateNextPageButton()
    {
        var nextButton = (View)NextPageButtonTemplate?.CreateContent() ?? new Button
        {
            Text = ">",
            StyleClass = new[] { "TextButton" },
            Command = new Command(() => ChangePageCommand?.Execute(CurrentPage + 1)),
        };
        nextButton.SetBinding(Button.IsEnabledProperty, new Binding(nameof(CanGoNext), source: this));
        return nextButton;
    }

    private View CreatePreviousPageButton()
    {
        var previousButton = (View)PreviousPageButtonTemplate?.CreateContent() ?? new Button
        {
            Text = "<",
            StyleClass = new[] { "TextButton" },
            Command = new Command(() => ChangePageCommand?.Execute(CurrentPage - 1)),
        };
        previousButton.SetId($"{ViewIdPrefix}-previous");
        previousButton.SetBinding(Button.IsEnabledProperty, new Binding(nameof(CanGoPrevious), source: this));
        return previousButton;
    }

    private View CreateFirstPageButton()
    {
        var firstPageButton = (View)FirstPageButtonTemplate?.CreateContent() ?? new Button
        {
            Text = "<<",
            StyleClass = new[] { "TextButton" },
            CommandParameter = 1
        };

        firstPageButton.SetId($"{ViewIdPrefix}-first");
        firstPageButton.SetBinding(View.IsEnabledProperty, new Binding(nameof(CanGoPrevious), source: this));

        if (firstPageButton is Button button)
        {
            firstPageButton.SetBinding(Button.CommandProperty, new Binding(nameof(ChangePageCommand), source: this));
        }

        return firstPageButton;
    }

    protected virtual void OnTotalPageCountChanged(int oldValue, int newValue)
    {
        UpdatePageNumbersItemsSource();
        ApplyCurrentPageState();
        UpdateCanGoProperties();
    }

    protected virtual void OnCurrentPageChanged(int oldValue, int newValue)
    {
        UpdatePageNumbersItemsSource();
        ApplyCurrentPageState();
        UpdateCanGoProperties();
    }

    protected virtual void OnPageStepCountChanged(int oldValue, int newValue)
    {
        UpdatePageNumbersItemsSource();
        ApplyCurrentPageState();
    }

    protected void UpdatePageNumbersItemsSource()
    {
        var start = Math.Max(1, CurrentPage - PageStepCount);
        var end = Math.Min(CurrentPage + PageStepCount, TotalPageCount);
        BindableLayout.SetItemsSource(PagesStackLayout, Enumerable.Range(start, end - start + 1));
    }

    protected virtual void ApplyCurrentPageState()
    {
        var currentPageBtn = this.FindInChildrenHierarchy<Button>(x => ViewQuery.GetId(x) == ViewIdPrefix && x.CommandParameter.Equals(CurrentPage));
        currentPageBtn.IsEnabled = false;
    }

    protected virtual void UpdateCanGoProperties()
    {
        CanGoPrevious = CurrentPage > 1;
        CanGoNext = CurrentPage < TotalPageCount;
    }
}
