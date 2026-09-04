
using Microsoft.Maui.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using UraniumUI.Controls;

namespace UraniumUI.Handlers;

public partial class AutoCompleteViewHandler
{
    public static IPropertyMapper<AutoCompleteView, AutoCompleteViewHandler> AutoCompleteViewMapper 
        => new PropertyMapper<AutoCompleteView, AutoCompleteViewHandler>(ViewHandler.ViewMapper)
        {
            [nameof(AutoCompleteView.Text)] = MapText,
            [nameof(AutoCompleteView.TextColor)] = MapTextColor,
            [nameof(AutoCompleteView.ItemsSource)] = MapItemsSource,
            [nameof(AutoCompleteView.Threshold)] = MapThreshold,
            [nameof(AutoCompleteView.Keyboard)] = MapKeyboard,
        };
    public AutoCompleteViewHandler() : base(AutoCompleteViewMapper)
    {
    }
}

#if (NET9_0 || NET10_0) && !ANDROID && !IOS && !MACCATALYST && !WINDOWS
public partial class AutoCompleteViewHandler : ViewHandler<AutoCompleteView, object>
{
    public AutoCompleteViewHandler(IPropertyMapper mapper, CommandMapper commandMapper = null) : base(AutoCompleteViewMapper, commandMapper)
    {
    }
    
    protected override object CreatePlatformView()
    {
        throw new NotImplementedException();
    }
    
    public static void MapText(AutoCompleteViewHandler handler, AutoCompleteView view)
    {
    }

    public static void MapTextColor(AutoCompleteViewHandler handler, AutoCompleteView view)
    {
    }

    public static void MapItemsSource(AutoCompleteViewHandler handler, AutoCompleteView view)
    {
    }

    public static void MapThreshold(AutoCompleteViewHandler handler, AutoCompleteView view)
    {
    }

    public static void MapKeyboard(AutoCompleteViewHandler handler, AutoCompleteView view)
    {
    }
}
#endif
