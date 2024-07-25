using System.Windows.Input;
using UraniumUI.Extensions;
using UraniumUI.ViewExtensions;

namespace UraniumUI.Material.Controls;

public class Paginator : ContentView
{
    public int CurrentPage { get => (int)GetValue(CurrentPageProperty); set => SetValue(CurrentPageProperty, value); }

    public static readonly BindableProperty CurrentPageProperty = BindableProperty.Create(
        nameof(CurrentPage),
        typeof(int),
        typeof(Paginator),
        defaultValue: 1,
        propertyChanged: (bo, ov, nv) => (bo as Paginator).OnCurrentPageChanged((int)ov, (int)nv)
    );

    public int TotalPageCount { get => (int)GetValue(TotalPageCountProperty); set => SetValue(TotalPageCountProperty, value); }

    public static readonly BindableProperty TotalPageCountProperty = BindableProperty.Create(
        nameof(TotalPageCount),
        typeof(int),
        typeof(Paginator),
        defaultValue: 0, propertyChanged: (bo, ov, nv) => (bo as Paginator).OnTotalPageCountChanged((int)ov, (int)nv)
    );

    public ICommand ChangePageCommand { get => (ICommand)GetValue(ChangePageCommandProperty); set => SetValue(ChangePageCommandProperty, value); }

    public static readonly BindableProperty ChangePageCommandProperty = BindableProperty.Create(
        nameof(ChangePageCommand),
        typeof(ICommand),
        typeof(Paginator)
    );

    public int PageStepCount { get => (int)GetValue(PageStepCountProperty); set => SetValue(PageStepCountProperty, value); }

    public static readonly BindableProperty PageStepCountProperty = BindableProperty.Create(
        nameof(PageStepCount),
        typeof(int),
        typeof(Paginator),
        defaultValue: 2,
        propertyChanged: (bo, ov, nv) => (bo as Paginator).OnPageStepCountChanged((int)ov, (int)nv)
    );

    public bool CanGoNext { get => canGoNext; set { canGoNext = value; OnPropertyChanged(); } }
    public bool CanGoPrevious { get => canGoPrevious; set { canGoPrevious = value; OnPropertyChanged(); } }

    private HorizontalStackLayout PagesStackLayout => this.FindByViewQueryId<HorizontalStackLayout>(nameof(PagesStackLayout));
    private bool canGoNext;
    private bool canGoPrevious;

    public Paginator()
    {
        var stackLayout = new HorizontalStackLayout();

        var firstPageButton = new Button
        {
            Text = "<<",
            StyleClass = new[] { "TextButton" },
            CommandParameter = 1
        };

        firstPageButton.SetBinding(Button.CommandProperty, new Binding(nameof(ChangePageCommand), source: this));
        firstPageButton.SetBinding(Button.IsEnabledProperty, new Binding(nameof(CanGoPrevious), source: this));

        var previousButton = new Button
        {
            Text = "<",
            StyleClass = new[] { "TextButton" },
            Command = new Command(() => ChangePageCommand?.Execute(CurrentPage - 1)),
        };
        previousButton.SetBinding(Button.IsEnabledProperty, new Binding(nameof(CanGoPrevious), source: this));

        var pagesStackLayout = new HorizontalStackLayout();
        pagesStackLayout.SetId(nameof(PagesStackLayout));

        BindableLayout.SetItemsSource(pagesStackLayout, Enumerable.Range(1, TotalPageCount));
        BindableLayout.SetItemTemplate(pagesStackLayout, new DataTemplate(() =>
        {
            var btn = new Button
            {
                StyleClass = new[] { "TextButton" }
            };

            ViewQuery.SetId(btn, "paginator-btn");
            btn.SetBinding(Button.TextProperty, new Binding("."));
            btn.SetBinding(Button.CommandProperty, new Binding(nameof(ChangePageCommand), source: this));
            btn.SetBinding(Button.CommandParameterProperty, new Binding("."));

            return btn;
        }));

        var nextButton = new Button
        {
            Text = ">",
            StyleClass = new[] { "TextButton" },
            Command = new Command(() => ChangePageCommand?.Execute(CurrentPage + 1)),
        };
        nextButton.SetBinding(Button.IsEnabledProperty, new Binding(nameof(CanGoNext), source: this));

        var lastPageButton = new Button
        {
            Text = ">>",
            StyleClass = new[] { "TextButton" },
        };
        lastPageButton.SetBinding(Button.CommandProperty, new Binding(nameof(ChangePageCommand), source: this));
        lastPageButton.SetBinding(Button.CommandParameterProperty, new Binding(nameof(TotalPageCount), source: this));
        lastPageButton.SetBinding(Button.IsEnabledProperty, new Binding(nameof(CanGoNext), source: this));

        stackLayout.Children.Add(firstPageButton);
        stackLayout.Children.Add(previousButton);
        stackLayout.Children.Add(pagesStackLayout);
        stackLayout.Children.Add(nextButton);
        stackLayout.Children.Add(lastPageButton);

        Content = stackLayout;
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
        var currentPageBtn = this.FindInChildrenHierarchy<Button>(x => ViewQuery.GetId(x) == "paginator-btn" && x.CommandParameter.Equals(CurrentPage));
        currentPageBtn.IsEnabled = false;
    }

    protected virtual void UpdateCanGoProperties()
    {
        CanGoPrevious = CurrentPage > 1;
        CanGoNext = CurrentPage < TotalPageCount;
    }
}
