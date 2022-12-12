#if IOS || MACCATALYST
using Microsoft.Maui.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIKit;
using UraniumUI.Material.Controls;

namespace UraniumUI.Material.Handlers;
public partial class AutoCompleteViewHandler : ViewHandler<AutoCompleteView, UITextField>
{

    protected override UITextField CreatePlatformView()
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