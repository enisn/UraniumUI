using Plainer.Maui.Controls;

namespace UraniumUI.Controls;


/// <summary>
/// See explanation in <see cref="DatePickerWrappedView"/>
/// </summary>
public class TimePickerWrappedView : TimePickerView, ITimePicker
{
    TimeSpan ITimePicker.Time
    {
        get => Time;
        set
        {
            if (value == Time)
            {
                Time += TimeSpan.FromSeconds(1);
            }

            Time = value;
            OnPropertyChanged(nameof(Time));
        }
    }
}
