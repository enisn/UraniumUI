# CalendarView

`CalendarView` is a cross-platform calendar layout for single-date selection. It is rendered with MAUI controls instead of native calendar views, so nullable selection, min/max behavior, and same-date selection are consistent across platforms.

## Usage

It is defined in the `UraniumUI.Controls` namespace. You can use it in XAML like this:

```xml
xmlns:uranium="http://schemas.enisn-projects.io/dotnet/maui/uraniumui"
```

Then you can use it with the `uranium:CalendarView` tag.

```xml
<uranium:CalendarView
    SelectedDate="{Binding Date}"
    MinimumDate="{Binding MinimumDate}"
    MaximumDate="{Binding MaximumDate}" />
```

## Properties

- **SelectedDate**: The selected date. Supports `DateTime?` and can be `null`.
- **DisplayDate**: The month currently displayed by the calendar.
- **MinimumDate**: The earliest selectable date. Dates before this value are disabled.
- **MaximumDate**: The latest selectable date. Dates after this value are disabled.
- **FirstDayOfWeek**: The first weekday column. Defaults to the current culture's first day of week.
- **VisibleDates**: The 42 visible calendar cells for the current display month.

## Selection

`CalendarView` raises `DateSelected` when a selectable day is tapped, even if the tapped date is already selected. This is useful for picker flows where confirming the same date should still be treated as a user action.

```csharp
calendarView.DateSelected += (sender, args) =>
{
    var selectedDate = args.SelectedDate;
};
```

You can also select or clear dates from code:

```csharp
calendarView.TrySelectDate(DateTime.Today);
calendarView.ClearSelection();
```

## Year Selection

Tap the month/year header to switch from the day grid to a year grid. This is useful for birthdate and other historical-date flows where navigating month-by-month would be too slow. The previous/next navigation buttons move between year pages while the year grid is visible. Selecting a year returns to the day grid and keeps the current month where possible.

## Styling

The view exposes style classes for common parts:

- `CalendarView`
- `CalendarView.Header`
- `CalendarView.MonthLabel`
- `CalendarView.NavigationButton`
- `CalendarView.PreviousMonthButton`
- `CalendarView.NextMonthButton`
- `CalendarView.WeekdayGrid`
- `CalendarView.WeekdayLabel`
- `CalendarView.DaysGrid`
- `CalendarView.DayButton`
- `CalendarView.DayButton.OutsideMonth`
- `CalendarView.DayButton.Disabled`
- `CalendarView.DayButton.Selected`
- `CalendarView.YearGrid`
- `CalendarView.YearButton`
- `CalendarView.YearButton.Disabled`
- `CalendarView.YearButton.Selected`
