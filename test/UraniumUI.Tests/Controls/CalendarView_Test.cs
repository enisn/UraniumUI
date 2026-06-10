using UraniumUI.Controls;
using UraniumUI.Tests.Core;

namespace UraniumUI.Tests.Controls;

public class CalendarView_Test
{
    public CalendarView_Test()
    {
        ApplicationExtensions.CreateAndSetMockApplication();
    }

    [Fact]
    public void VisibleDates_ShouldBuildSixWeekGrid_WhenMonthStartsOnFirstDayOfWeek()
    {
        var calendarView = new CalendarView
        {
            FirstDayOfWeek = DayOfWeek.Monday,
            DisplayDate = new DateTime(2026, 6, 15)
        };

        Assert.Equal(42, calendarView.VisibleDates.Count);
        Assert.Equal(new DateTime(2026, 6, 1), calendarView.VisibleDates.First().Date);
        Assert.Equal(new DateTime(2026, 7, 12), calendarView.VisibleDates.Last().Date);
    }

    [Fact]
    public void VisibleDates_ShouldIncludeLeadingDays_WhenMonthDoesNotStartOnFirstDayOfWeek()
    {
        var calendarView = new CalendarView
        {
            FirstDayOfWeek = DayOfWeek.Monday,
            DisplayDate = new DateTime(2026, 5, 15)
        };

        Assert.Equal(new DateTime(2026, 4, 27), calendarView.VisibleDates.First().Date);
        Assert.Equal(new DateTime(2026, 5, 1), calendarView.VisibleDates[4].Date);
        Assert.False(calendarView.VisibleDates[0].IsCurrentMonth);
        Assert.True(calendarView.VisibleDates[4].IsCurrentMonth);
    }

    [Fact]
    public void TrySelectDate_ShouldSetSelectedDate_WhenDateIsEnabled()
    {
        var calendarView = new CalendarView();
        var date = DateTime.Today.AddDays(1);

        var selected = calendarView.TrySelectDate(date);

        Assert.True(selected);
        Assert.Equal(date.Date, calendarView.SelectedDate);
    }

    [Fact]
    public void TrySelectDate_ShouldRaiseDateSelected_WhenSameDateIsSelectedAgain()
    {
        var date = DateTime.Today;
        var calendarView = new CalendarView
        {
            SelectedDate = date
        };
        var eventCount = 0;

        calendarView.DateSelected += (sender, args) =>
        {
            eventCount++;
            Assert.Equal(date.Date, args.OldDate);
            Assert.Equal(date.Date, args.SelectedDate);
        };

        var selected = calendarView.TrySelectDate(date);

        Assert.True(selected);
        Assert.Equal(1, eventCount);
    }

    [Fact]
    public void TrySelectDate_ShouldIgnoreDate_WhenDateIsBeforeMinimumDate()
    {
        var calendarView = new CalendarView
        {
            MinimumDate = new DateTime(2026, 5, 10),
            MaximumDate = new DateTime(2026, 5, 20)
        };

        var selected = calendarView.TrySelectDate(new DateTime(2026, 5, 9));

        Assert.False(selected);
        Assert.Null(calendarView.SelectedDate);
    }

    [Fact]
    public void TrySelectDate_ShouldIgnoreDate_WhenDateIsAfterMaximumDate()
    {
        var calendarView = new CalendarView
        {
            MinimumDate = new DateTime(2026, 5, 10),
            MaximumDate = new DateTime(2026, 5, 20)
        };

        var selected = calendarView.TrySelectDate(new DateTime(2026, 5, 21));

        Assert.False(selected);
        Assert.Null(calendarView.SelectedDate);
    }

    [Fact]
    public void ClearSelection_ShouldSetSelectedDateToNull()
    {
        var calendarView = new CalendarView
        {
            SelectedDate = DateTime.Today
        };

        calendarView.ClearSelection();

        Assert.Null(calendarView.SelectedDate);
    }

    [Fact]
    public void NextMonthCommand_ShouldAdvanceDisplayDate()
    {
        var calendarView = new CalendarView
        {
            DisplayDate = new DateTime(2026, 5, 15)
        };

        calendarView.NextMonthCommand.Execute(null);

        Assert.Equal(new DateTime(2026, 6, 15), calendarView.DisplayDate);
    }

    [Fact]
    public void PreviousMonthCommand_ShouldNotNavigate_WhenPreviousMonthIsBeforeMinimumDate()
    {
        var calendarView = new CalendarView
        {
            DisplayDate = new DateTime(2026, 5, 15),
            MinimumDate = new DateTime(2026, 5, 10)
        };

        Assert.False(calendarView.PreviousMonthCommand.CanExecute(null));
    }

    [Fact]
    public void VisibleDates_ShouldMarkDatesOutsideMinMaxAsDisabled()
    {
        var calendarView = new CalendarView
        {
            FirstDayOfWeek = DayOfWeek.Monday,
            DisplayDate = new DateTime(2026, 5, 15),
            MinimumDate = new DateTime(2026, 5, 10),
            MaximumDate = new DateTime(2026, 5, 20)
        };

        Assert.False(calendarView.VisibleDates.Single(day => day.Date == new DateTime(2026, 5, 9)).IsEnabled);
        Assert.True(calendarView.VisibleDates.Single(day => day.Date == new DateTime(2026, 5, 10)).IsEnabled);
        Assert.True(calendarView.VisibleDates.Single(day => day.Date == new DateTime(2026, 5, 20)).IsEnabled);
        Assert.False(calendarView.VisibleDates.Single(day => day.Date == new DateTime(2026, 5, 21)).IsEnabled);
    }
}
