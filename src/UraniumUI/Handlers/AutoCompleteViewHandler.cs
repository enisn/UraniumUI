
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
    public static IPropertyMapper<AutoCompleteView, AutoCompleteViewHandler> IconViewMapper 
        => new PropertyMapper<AutoCompleteView, AutoCompleteViewHandler>(ViewHandler.ViewMapper)
        {
            [nameof(AutoCompleteView.Text)] = MapText,
            [nameof(AutoCompleteView.ItemsSource)] = MapItemsSource,
        };
    public AutoCompleteViewHandler() : base(IconViewMapper)
    {
    }
}

#if (NET7_0 || NET6_0 ) && !ANDROID && !IOS && !MACCATALYST && !WINDOWS
public partial class AutoCompleteViewHandler : ViewHandler<AutoCompleteView, object>
{
    public AutoCompleteViewHandler(IPropertyMapper mapper, CommandMapper commandMapper = null) : base(IconViewMapper, commandMapper)
    {
    }
    
    protected override object CreatePlatformView()
    {
        throw new NotImplementedException();
    }
    
    public static void MapText(AutoCompleteViewHandler handler, AutoCompleteView view)
    {
    }

    public static void MapItemsSource(AutoCompleteViewHandler handler, AutoCompleteView view)
    {
    }
}
#endif