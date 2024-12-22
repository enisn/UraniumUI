using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using UraniumUI.Controls;

namespace UraniumUI.Handlers;

#if ANDROID
using Android.Widget;
using Android.App;
using AndroidX.AppCompat.Widget;
using Android.Text;
using UraniumUI.Platforms.Android;

public partial class UraniumDatePickerHandler : ViewHandler<IUraniumDatePicker, UraniumAndroidDatePicker>
{
    protected override UraniumAndroidDatePicker CreatePlatformView()
    {
        return new UraniumAndroidDatePicker(Context)
        {
            ShowPicker = ShowPickerDialog,
            HidePicker = () =>
            {
                // Do nothing
            }
        };
    }

    private void ShowPickerDialog()
    {
        var datePicker = new DatePickerDialog(Context!, (o, e) =>
        {
            if (VirtualView != null)
            {
                VirtualView.Date = e.Date;
            }
        }, DateTime.Now.Year, DateTime.Now.Month - 1, DateTime.Now.Day);
        datePicker.Show();
    }

    protected virtual DatePickerDialog CreateDatePickerDialog(int year, int month, int day)
    {
        var dialog = new DatePickerDialog(Context!, (o, e) =>
        {
            if (VirtualView != null)
            {
                VirtualView.Date = e.Date;
            }
        }, year, month, day);

        dialog.ShowEvent += (s, e) =>
        {
            Console.WriteLine("ShowEvent");
        };

        dialog.DismissEvent += (s, e) =>
        {
            Console.WriteLine("DismissEvent");
        };

        return dialog;
    }

    public static void MapSelectedDate(UraniumDatePickerHandler handler, IUraniumDatePicker datePicker)
    {
        handler.PlatformView.Text = datePicker.Date?.ToString("d");
    }
}
#endif

#if WINDOWS
using Microsoft.UI.Xaml.Controls;
public partial class UraniumDatePickerHandler : ViewHandler<IUraniumDatePicker, CalendarDatePicker>
{
    public UraniumDatePickerHandler(IPropertyMapper mapper, CommandMapper commandMapper = null) : base(mapper, commandMapper)
    {
    }
    protected override CalendarDatePicker CreatePlatformView()
    {
        var datePicker = new CalendarDatePicker();

        datePicker.DateChanged += (s, e) =>
        {
            if (e.NewDate.HasValue)
            {
                VirtualView.Date = e.NewDate!.Value.DateTime;
            }
        };
        return datePicker;
    }

    public static void MapSelectedDate(UraniumDatePickerHandler handler, IUraniumDatePicker datePicker)
    {
        handler.PlatformView.Date = datePicker.Date ?? DateTime.Now;
    }
}
#endif
#if IOS || MACCATALYST
using UIKit;

public partial class UraniumDatePickerHandler : ViewHandler<IUraniumDatePicker, UIDatePicker>
{
    public UraniumDatePickerHandler(IPropertyMapper mapper, CommandMapper commandMapper = null) : base(mapper, commandMapper)
    {
    }
    protected override UIDatePicker CreatePlatformView()
    {
        var datePicker = new UIDatePicker();
        datePicker.Mode = UIDatePickerMode.Date;
        datePicker.ValueChanged += (s, e) =>
        {
            VirtualView.Date = datePicker.Date.ToDateTime();
        };
        return datePicker;
    }

    public static void MapSelectedDate(UraniumDatePickerHandler handler, IUraniumDatePicker datePicker)
    {
        if (datePicker.Date == null)
        {
            return;
        }

        handler.PlatformView.Date = datePicker.Date?.ToNSDate();
    }
}

#endif

#if (NET8_0 || NET9_0) && !ANDROID && !IOS && !MACCATALYST && !WINDOWS
public partial class UraniumDatePickerHandler : ViewHandler<IUraniumDatePicker, object>
{
    public UraniumDatePickerHandler(IPropertyMapper mapper, CommandMapper commandMapper = null) : base(mapper, commandMapper)
    {
    }

    protected override object CreatePlatformView()
    {
        throw new NotImplementedException();
    }

    public static void MapSelectedDate(UraniumDatePickerHandler handler, IUraniumDatePicker datePicker)
    {
    }
}
#endif

public partial class UraniumDatePickerHandler
{
    public UraniumDatePickerHandler() : base(UraniumDatePickerPropertyMapper)
    {
    }

    public static IPropertyMapper<IUraniumDatePicker, UraniumDatePickerHandler> UraniumDatePickerPropertyMapper => new PropertyMapper<IUraniumDatePicker, UraniumDatePickerHandler>(ViewMapper)
    {
        [nameof(IUraniumDatePicker.Date)] = MapSelectedDate,
    };
}