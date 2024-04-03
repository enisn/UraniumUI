#if ANDROID
using Android.Views;
#endif
#if WINDOWS
using Microsoft.UI.Xaml.Input;
#endif
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if IOS || MACCATALYST
using Foundation;
using UIKit;
#endif
using UraniumUI.Views;
using static Microsoft.Maui.Controls.VisualStateManager;
using System.Windows.Input;
using UraniumUI.Resources;

namespace UraniumUI.Handlers;

/// <summary>
/// A handler for <see cref="StatefulContentView"/>.
/// </summary>
public partial class StatefulContentViewHandler : ContentViewHandler
{
    public static IPropertyMapper<StatefulContentView, StatefulContentViewHandler> StatefulContentViewMapper
    => new PropertyMapper<StatefulContentView, StatefulContentViewHandler>(ContentViewHandler.Mapper)
    {
        [nameof(StatefulContentView.IsFocusable)] = MapIsFocusable,
    };

    public StatefulContentViewHandler() : base(StatefulContentViewMapper)
    {
    }

    public StatefulContentView StatefulView => VirtualView as StatefulContentView;

    private void ExecuteCommandIfCan(ICommand command)
    {
        if (command?.CanExecute(StatefulView.CommandParameter) ?? false)
        {
            command.Execute(StatefulView.CommandParameter);
        }
    }
#if NET8_0 && !ANDROID && !IOS && !MACCATALYST && !WINDOWS
    public static void MapIsFocusable(StatefulContentViewHandler handler, StatefulContentView view)
    {

    }
#endif
}
