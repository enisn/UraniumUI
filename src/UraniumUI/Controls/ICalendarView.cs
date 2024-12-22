namespace UraniumUI.Controls;
public interface ICalendarView : IView
{
    SelectionMode SelectionMode { get; set; }

    DateTime? SelectedDate { get; set; }

    IList<DateTime> SelectedDates { get; set; }
}
