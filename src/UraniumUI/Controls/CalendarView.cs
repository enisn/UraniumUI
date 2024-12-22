namespace UraniumUI.Controls;
public class CalendarView : View, ICalendarView
{
    public static readonly BindableProperty SelectionModeProperty = BindableProperty.Create(
        nameof(SelectionMode), typeof(SelectionMode), typeof(CalendarView), default(SelectionMode));

    public static readonly BindableProperty SelectedDateProperty = BindableProperty.Create(
        nameof(SelectedDate), typeof(DateTime?), typeof(CalendarView), default(DateTime?));

    public static readonly BindableProperty SelectedDatesProperty = BindableProperty.Create(
        nameof(SelectedDates), typeof(IList<DateTime>), typeof(CalendarView), default(IList<DateTime>));

    public SelectionMode SelectionMode
    {
        get => (SelectionMode)GetValue(SelectionModeProperty);
        set => SetValue(SelectionModeProperty, value);
    }

    public DateTime? SelectedDate
    {
        get => (DateTime?)GetValue(SelectedDateProperty);
        set => SetValue(SelectedDateProperty, value);
    }

    public IList<DateTime> SelectedDates
    {
        get => (IList<DateTime>)GetValue(SelectedDatesProperty);
        set => SetValue(SelectedDatesProperty, value);
    }
}
