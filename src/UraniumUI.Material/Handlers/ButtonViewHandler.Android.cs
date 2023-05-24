#if ANDROID

using Android.Text.Method;
using Android.Views;
using Microsoft.Maui.Platform;
using Microsoft.Maui.Controls;

namespace UraniumUI.Material.Handlers;
public partial class ButtonViewHandler
{
    protected override void ConnectHandler(ContentViewGroup platformView)
    {
        base.ConnectHandler(platformView);
        
        platformView.Touch += OnTouch;
        platformView.Hover += NativeView_Hover;
        platformView.LongClick += PlatformView_LongClick;
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
#if NET6_0
            VisualStateManager.GoToState(StatefulView, "PointerOver");
#else
            VisualStateManager.GoToState(StatefulView, VisualStateManager.CommonStates.PointerOver);
#endif
            ExecuteCommandIfCan(StatefulView.HoverCommand);
            return;
        }

        if (e.Event.Action == MotionEventActions.HoverExit)
        {
            VisualStateManager.GoToState(StatefulView, VisualStateManager.CommonStates.Normal);
            ExecuteCommandIfCan(StatefulView.HoverExitCommand);
        }
    }

    private void OnTouch(object sender, Android.Views.View.TouchEventArgs e)
    {
        if (e.Event.Action == MotionEventActions.Down)
        {
            VisualStateManager.GoToState(StatefulView, "Pressed");
            ExecuteCommandIfCan(StatefulView.PressedCommand);
        }
        else if (e.Event.Action == MotionEventActions.Up)
        {
            VisualStateManager.GoToState(StatefulView, VisualStateManager.CommonStates.Normal);
            ExecuteCommandIfCan(StatefulView.TappedCommand);
        }
    }
    private void PlatformView_LongClick(object sender, Android.Views.View.LongClickEventArgs e)
    {
        ExecuteCommandIfCan(StatefulView.LongPressCommand);
    }
}
#endif
