using Plainer.Maui.Controls;

namespace UraniumUI.Controls;

/// <summary>
/// TODO Revisit when the underlying dotnet/maui issue is fixed: https://github.com/dotnet/maui/issues/13156
/// A workaround for abovementioned issue where the DateSelected event is not raised when the date is the same as the current date.
/// This "manually" raises the PropertyChanged event when the date is the same as the current date by briefly setting it to a different time before applying the actual value.
/// Alternatively, this workaround could be implemented in Plainer.Maui.Controls.DatePickerView directly.
/// </summary>
public class DatePickerWrappedView : DatePickerView, IDatePicker
{
    DateTime IDatePicker.Date
    {
        get => Date;
        set
        {
            if (value == Date)
            {
                Date += TimeSpan.FromDays(1);
            }

            Date = value;
            OnPropertyChanged(nameof(Date));
        }
    }
}
