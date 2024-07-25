#if WINDOWS
using Microsoft.Maui.Platform;
using Microsoft.UI.Xaml.Input;
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
    protected override ContentPanel CreatePlatformView()
    {
        var platformView = base.CreatePlatformView();

        platformView.AddHandler(
           Microsoft.UI.Xaml.Controls.Button.PointerPressedEvent,
           new PointerEventHandler(PlatformView_PointerPressed), true);

        platformView.AddHandler(
            Microsoft.UI.Xaml.Controls.Button.PointerReleasedEvent,
            new PointerEventHandler(PlatformView_PointerReleased), true);

        platformView.PointerEntered += PlatformView_PointerEntered;
        platformView.PointerExited += PlatformView_PointerExited;

        platformView.IsTabStop = true;
        platformView.UseSystemFocusVisuals = true;
        platformView.KeyDown += PlatformView_KeyDown;
        platformView.KeyUp += PlatformView_KeyUp;

        return platformView;
    }

    protected override void DisconnectHandler(ContentPanel platformView)
    {
        platformView.PointerEntered -= PlatformView_PointerEntered;
        platformView.PointerExited -= PlatformView_PointerExited;
        platformView.KeyDown -= PlatformView_KeyDown;
        platformView.KeyUp -= PlatformView_KeyUp;
        base.DisconnectHandler(platformView);
    }

    private void PlatformView_PointerExited(object sender, PointerRoutedEventArgs e)
    {
        GoToState(StatefulView, CommonStates.Normal);
        ExecuteCommandIfCan(StatefulView.HoverExitCommand);
        StatefulView.InvokeHoverExited();
    }

    private void PlatformView_PointerEntered(object sender, PointerRoutedEventArgs e)
    {
        GoToState(StatefulView, CommonStates.PointerOver);
        ExecuteCommandIfCan(StatefulView.HoverCommand);
        StatefulView.InvokeHovered();
    }

    long lastPressed;

    private void PlatformView_PointerPressed(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
    {
        lastPressed = DateTime.Now.Ticks;

        GoToState(StatefulView, "Pressed");
        ExecuteCommandIfCan(StatefulView.PressedCommand);
        StatefulView.InvokePressed();
    }

    private void PlatformView_PointerReleased(object sender, PointerRoutedEventArgs e)
    {
        // TODO: Implement better.
        if (DateTime.Now.Ticks - lastPressed >= TimeSpan.TicksPerMillisecond * 500)
        {
            ExecuteCommandIfCan(StatefulView.LongPressCommand);
        }

        GoToState(StatefulView, CommonStates.Normal);
        ExecuteCommandIfCan(StatefulView.TappedCommand);
        StatefulView.InvokeTapped();
    }

    private void PlatformView_KeyDown(object sender, KeyRoutedEventArgs e)
    {
        if (IsActionKey(e.Key))
        {
            GoToState(StatefulView, "Pressed");
            ExecuteCommandIfCan(StatefulView.PressedCommand);
            StatefulView.InvokePressed();
        }
    }

    private void PlatformView_KeyUp(object sender, KeyRoutedEventArgs e)
    {
        if (IsActionKey(e.Key))
        {
            ExecuteCommandIfCan(StatefulView.TappedCommand);
            StatefulView.InvokeTapped();
            GoToState(StatefulView, CommonStates.Normal);
        }
    }

    private bool IsActionKey(Windows.System.VirtualKey key)
    {
        return key == Windows.System.VirtualKey.Enter || key == Windows.System.VirtualKey.Space;
    }

    internal void UpdateFocusable()
    {
        PlatformView.IsTabStop = StatefulView.IsFocusable;
    }

    public static void MapIsFocusable(StatefulContentViewHandler handler, StatefulContentView view)
    {
        handler.PlatformView.IsTabStop = view.IsFocusable;
    }
}
#endif