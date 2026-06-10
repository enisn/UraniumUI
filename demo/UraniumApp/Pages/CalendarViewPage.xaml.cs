namespace UraniumApp.Pages;

public partial class CalendarViewPage : ContentPage
{
    public CalendarViewPage()
    {
        InitializeComponent();

        calendarView.MinimumDate = DateTime.Today.AddMonths(-2);
        calendarView.MaximumDate = DateTime.Today.AddMonths(2);
    }

    private void TodayClicked(object sender, EventArgs e)
    {
        calendarView.TrySelectDate(DateTime.Today);
    }

    private void ClearClicked(object sender, EventArgs e)
    {
        calendarView.ClearSelection();
    }
}
