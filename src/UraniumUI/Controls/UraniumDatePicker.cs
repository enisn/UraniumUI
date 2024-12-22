using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UraniumUI.Controls;

#if ANDROID
public class UraniumDatePicker : DatePicker, IUraniumDatePicker
#else
public class UraniumDatePicker : View, IUraniumDatePicker
#endif
{
    public static readonly BindableProperty DateProperty = BindableProperty.Create(
        nameof(Date),
        typeof(DateTime?),
        typeof(UraniumDatePicker),
        default(DateTime?),
        propertyChanged: OnDateChanged);

    public DateTime? Date
    {
        get => (DateTime?)GetValue(DateProperty);
        set => SetValue(DateProperty, value);
    }

    public event EventHandler<DateTime> DateChanged;

    private static void OnDateChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (UraniumDatePicker)bindable;
        if (newValue is DateTime newDate)
        {
            control.DateChanged?.Invoke(control, newDate);
        }
    }
}
