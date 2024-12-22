using Microsoft.Maui.Handlers;
using UraniumUI.Controls;

#if ANDROID
namespace UraniumUI.Handlers;
public partial class CalendarViewHandler : ViewHandler<ICalendarView, Android.Widget.CalendarView>
{
    public CalendarViewHandler(IPropertyMapper mapper, CommandMapper commandMapper = null) : base(mapper, commandMapper)
    {
    }

    protected override Android.Widget.CalendarView CreatePlatformView()
    {
        return new Android.Widget.CalendarView(Context);
    }

    public static void MapSelectionMode(CalendarViewHandler handler, ICalendarView calendarView)
    {
        throw new NotSupportedException("Setting SelectionMode is not supported on Android. ");
    }

    public static void MapSelectedDate(CalendarViewHandler handler, ICalendarView calendarView)
    {
        if (calendarView.SelectedDate.HasValue)
        {
            handler.PlatformView.Date = calendarView.SelectedDate.Value.ToUniversalTime().Ticks;
        }
        else
        {
            handler.PlatformView.Date = DateTime.UtcNow.Ticks;
        }
    }

    public static void MapSelectedDates(CalendarViewHandler handler, ICalendarView calendarView)
    {
        handler.PlatformView.Date = calendarView.SelectedDates.First().ToUniversalTime().Ticks;
    }
}

#endif

#if WINDOWS

public partial class CalendarViewHandler : ViewHandler<ICalendarView, Microsoft.UI.Xaml.Controls.CalendarView>
{
    protected override Microsoft.UI.Xaml.Controls.CalendarView CreatePlatformView()
    {
        return new Microsoft.UI.Xaml.Controls.CalendarView();
    }

    private Microsoft.UI.Xaml.Controls.CalendarViewSelectionMode GetSelectionMode()
    {
        switch (VirtualView.SelectionMode)
        {
            case SelectionMode.None:
                return Microsoft.UI.Xaml.Controls.CalendarViewSelectionMode.None;
            case SelectionMode.Single:
                return Microsoft.UI.Xaml.Controls.CalendarViewSelectionMode.Single;
            case SelectionMode.Multiple:
                return Microsoft.UI.Xaml.Controls.CalendarViewSelectionMode.Multiple;
            default:
                return default;
        }
    }

    public static void MapSelectionMode(CalendarViewHandler handler, ICalendarView calendarView)
    {
        handler.PlatformView.SelectionMode = handler.GetSelectionMode();
    }

    public static void MapSelectedDate(CalendarViewHandler handler, ICalendarView calendarView)
    {
        handler.PlatformView.SelectedDates.Clear();

        if (calendarView.SelectedDate.HasValue)
        {
            handler.PlatformView.SelectedDates.Add(calendarView.SelectedDate.Value);
        }
    }

    public static void MapSelectedDates(CalendarViewHandler handler, ICalendarView calendarView)
    {
        handler.PlatformView.SelectedDates.Clear();
        if (calendarView.SelectedDates != null)
        {
            foreach (var date in calendarView.SelectedDates)
            {
                handler.PlatformView.SelectedDates.Add(date);
            }
        }
    }
}
#endif

#if IOS || MACCATALYST
using UIKit;

namespace UraniumUI.Handlers;
using Foundation;

public partial class CalendarViewHandler : ViewHandler<ICalendarView, UICalendarView>
{
    protected override UICalendarView CreatePlatformView()
    {
        return new UICalendarView();
    }

    //protected override UIDatePicker CreatePlatformView()
    //{
    //    var dp = new UICalendarView();

    //    dp.SelectionBehavior = new UICalendarSelectionSingleDate();
    //    return dp;
    //}

    public static void MapSelectionMode(CalendarViewHandler handler, ICalendarView calendarView)
    {
        handler.PlatformView.SelectionBehavior = calendarView.SelectionMode switch
        {
            SelectionMode.None => null,
            SelectionMode.Single => new UICalendarSelectionSingleDate(),
            SelectionMode.Multiple => new UICalendarSelectionMultiDate(),
            _ => default
        };

    }

    public static void MapSelectedDate(CalendarViewHandler handler, ICalendarView calendarView)
    {
        if (handler.PlatformView.SelectionBehavior is UICalendarSelectionSingleDate singleDate)
        {
            if (calendarView.SelectedDate == null)
            {
                singleDate.SelectedDate = null;
            }
            else
            {
                singleDate.SelectedDate = new NSDateComponents
                {
                    Year = calendarView.SelectedDate.Value.Year,
                    Month = calendarView.SelectedDate.Value.Month,
                    Day = calendarView.SelectedDate.Value.Day
                };
            }
        }
    }

    public static void MapSelectedDates(CalendarViewHandler handler, ICalendarView calendarView)
    {
        if (handler.PlatformView.SelectionBehavior is UICalendarSelectionMultiDate multiDate)
        {
            var dates = new List<NSDateComponents>();
            foreach (var date in calendarView.SelectedDates)
            {
                dates.Add(new NSDateComponents
                {
                    Year = date.Year,
                    Month = date.Month,
                    Day = date.Day
                });
            }
            multiDate.SelectedDates = dates.ToArray();
        }
    }
}

#endif

#if (NET8_0 || NET9_0) && !ANDROID && !IOS && !MACCATALYST && !WINDOWS
public partial class CalendarViewHandler : ViewHandler<ICalendarView, object>
{

    protected override object CreatePlatformView()
    {
        throw new NotImplementedException();
    }

    public static void MapSelectionMode(CalendarViewHandler handler, ICalendarView calendarView)
    {
    }

    public static void MapSelectedDate(CalendarViewHandler handler, ICalendarView calendarView)
    {
    }

    public static void MapSelectedDates(CalendarViewHandler handler, ICalendarView calendarView)
    {
    }
}
#endif

public partial class CalendarViewHandler
{
    public CalendarViewHandler() : base(CalendarViewPropertyMapper)
    {

    }

    public static IPropertyMapper<ICalendarView, CalendarViewHandler> CalendarViewPropertyMapper => new PropertyMapper<ICalendarView, CalendarViewHandler>(ViewMapper)
    {
        [nameof(ICalendarView.SelectionMode)] = MapSelectionMode,
        [nameof(ICalendarView.SelectedDate)] = MapSelectedDate,
        [nameof(ICalendarView.SelectedDates)] = MapSelectedDates
    };
}