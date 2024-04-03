#if ANDROID
using Android.Views;
using Microsoft.Maui.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UraniumUI.Views;
using static Microsoft.Maui.Controls.VisualStateManager;

namespace UraniumUI.Handlers;
public partial class StatefulContentViewHandler
{
    protected override ContentViewGroup CreatePlatformView()
    {
        var platformView = base.CreatePlatformView();

        platformView.Touch += OnTouch;
        platformView.Hover += NativeView_Hover;
        platformView.Click += PlatformView_Click;
        platformView.LongClick += PlatformView_LongClick;

        return platformView;
    }

    protected override void DisconnectHandler(ContentViewGroup platformView)
    {
        platformView.Touch -= OnTouch;
        platformView.Hover -= NativeView_Hover;
        platformView.LongClick -= PlatformView_LongClick;
        base.DisconnectHandler(platformView);
    }

    private void NativeView_Hover(object sender, Android.Views.View.HoverEventArgs e)
    {
        if (e.Event.Action == MotionEventActions.HoverEnter)
        {
            GoToState(StatefulView, CommonStates.PointerOver);
            ExecuteCommandIfCan(StatefulView.HoverCommand);
            StatefulView.InvokeHovered();
            return;
        }

        if (e.Event.Action == MotionEventActions.HoverExit)
        {
            GoToState(StatefulView, CommonStates.Normal);
            ExecuteCommandIfCan(StatefulView.HoverExitCommand);
            StatefulView.InvokeHoverExited();
        }
    }

    private void OnTouch(object sender, Android.Views.View.TouchEventArgs e)
    {
        if (e.Event.Action == MotionEventActions.Down)
        {
            GoToState(StatefulView, "Pressed");
            ExecuteCommandIfCan(StatefulView.PressedCommand);
            StatefulView.InvokePressed();
            e.Handled = false;
        }
        else if (e.Event.Action == MotionEventActions.Up)
        {
            GoToState(StatefulView, CommonStates.Normal);
            e.Handled = false;
        }
    }

    private void PlatformView_Click(object sender, EventArgs e)
    {
        GoToState(StatefulView, CommonStates.Normal);
        ExecuteCommandIfCan(StatefulView.TappedCommand);
        StatefulView.InvokeTapped();
    }

    private void PlatformView_LongClick(object sender, Android.Views.View.LongClickEventArgs e)
    {
        ExecuteCommandIfCan(StatefulView.LongPressCommand);
        StatefulView.InvokeLongPressed();
    }

    public static void MapIsFocusable(StatefulContentViewHandler handler, StatefulContentView view)
    {
        handler.StatefulView.IsFocusable = view.IsFocusable;
    }
}
#endif