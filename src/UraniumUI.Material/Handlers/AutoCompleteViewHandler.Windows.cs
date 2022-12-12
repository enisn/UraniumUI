#if WINDOWS
using Microsoft.Maui.Handlers;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UraniumUI.Material.Controls;

namespace UraniumUI.Material.Handlers;
public partial class AutoCompleteViewHandler : ViewHandler<AutoCompleteView, TextBox>
{
    public static void MapText(AutoCompleteViewHandler handler, AutoCompleteView view)
    {
    }

    public static void MapItemsSource(AutoCompleteViewHandler handler, AutoCompleteView view)
    {
    }


    protected override TextBox CreatePlatformView()
    {
        throw new NotImplementedException();
    }
}
#endif