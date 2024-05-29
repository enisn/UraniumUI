using System.Windows.Input;

namespace UraniumUI.Material.Controls;
public partial class Paginator
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

    public DataTemplate FirstPageButtonTemplate { get => (DataTemplate)GetValue(FirstPageButtonTemplateProperty); set => SetValue(FirstPageButtonTemplateProperty, value); }

    public static readonly BindableProperty FirstPageButtonTemplateProperty = BindableProperty.Create(
        nameof(FirstPageButtonTemplate),
        typeof(DataTemplate),
        typeof(Paginator)
    );

    public DataTemplate PreviousPageButtonTemplate { get => (DataTemplate)GetValue(PreviousPageButtonTemplateProperty); set => SetValue(PreviousPageButtonTemplateProperty, value); }

    public static readonly BindableProperty PreviousPageButtonTemplateProperty = BindableProperty.Create(
        nameof(PreviousPageButtonTemplate),
        typeof(DataTemplate),
        typeof(Paginator)
    );

    public DataTemplate NextPageButtonTemplate { get => (DataTemplate)GetValue(NextPageButtonTemplateProperty); set => SetValue(NextPageButtonTemplateProperty, value); }

    public static readonly BindableProperty NextPageButtonTemplateProperty = BindableProperty.Create(
        nameof(NextPageButtonTemplate),
        typeof(DataTemplate),
        typeof(Paginator)
    );

    public DataTemplate LastPageButtonTemplate { get => (DataTemplate)GetValue(LastPageButtonTemplateProperty); set => SetValue(LastPageButtonTemplateProperty, value); }

    public static readonly BindableProperty LastPageButtonTemplateProperty = BindableProperty.Create(
        nameof(LastPageButtonTemplate),
        typeof(DataTemplate),
        typeof(Paginator)
    );

    public bool ShowFirstAndLastPageButtons { get => (bool)GetValue(ShowFirstAndLastPageButtonsProperty); set => SetValue(ShowFirstAndLastPageButtonsProperty, value); }

    public static readonly BindableProperty ShowFirstAndLastPageButtonsProperty = BindableProperty.Create(
        nameof(ShowFirstAndLastPageButtons),
        typeof(bool),
        typeof(Paginator),
        defaultValue: true
    );

    public bool ShowPreviousAndNextPageButtons { get => (bool)GetValue(ShowPreviousAndNextPageButtonsProperty); set => SetValue(ShowPreviousAndNextPageButtonsProperty, value); }

    public static readonly BindableProperty ShowPreviousAndNextPageButtonsProperty = BindableProperty.Create(
        nameof(ShowPreviousAndNextPageButtons),
        typeof(bool),
        typeof(Paginator),
        defaultValue: true
    );
}