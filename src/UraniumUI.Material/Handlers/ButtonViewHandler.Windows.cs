#if WINDOWS

using Microsoft.Maui.Platform;
using Microsoft.UI.Xaml.Input;

namespace UraniumUI.Material.Handlers;

public partial class ButtonViewHandler
{
    protected override ContentPanel CreatePlatformView()
    {
        var platformView = base.CreatePlatformView();
#if DEBUG        
        platformView.AddHandler(
           Microsoft.UI.Xaml.Controls.Button.PointerPressedEvent,
           new PointerEventHandler(PlatformView_PointerPressed), true);

        platformView.AddHandler(
            Microsoft.UI.Xaml.Controls.Button.PointerReleasedEvent,
            new PointerEventHandler(PlatformView_PointerReleased), true);
#else
        platformView.PointerPressed += PlatformView_PointerPressed;
        platformView.PointerReleased += PlatformView_PointerReleased;
#endif

        platformView.PointerEntered += PlatformView_PointerEntered;
        platformView.PointerExited += PlatformView_PointerExited;

        return platformView;
    }

    protected override void DisconnectHandler(ContentPanel platformView)
    {
#if !DEBUG
        platformView.PointerPressed -= PlatformView_PointerPressed;
        platformView.PointerReleased -= PlatformView_PointerReleased;
#endif

        platformView.PointerEntered -= PlatformView_PointerEntered;
        platformView.PointerExited -= PlatformView_PointerExited;
        base.DisconnectHandler(platformView);
    }

    private void PlatformView_PointerExited(object sender, PointerRoutedEventArgs e)
    {
        VisualStateManager.GoToState(StatefulView, VisualStateManager.CommonStates.Normal);
        ExecuteCommandIfCan(StatefulView.HoverExitCommand);
    }

    private void PlatformView_PointerEntered(object sender, PointerRoutedEventArgs e)
    {
        VisualStateManager.GoToState(StatefulView, VisualStateManager.CommonStates.PointerOver);
        ExecuteCommandIfCan(StatefulView.HoverCommand);
    }

    long lastPressed;

    private void PlatformView_PointerPressed(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
    {
        lastPressed = DateTime.Now.Ticks;

        VisualStateManager.GoToState(StatefulView, "Pressed");
        ExecuteCommandIfCan(StatefulView.PressedCommand);
    }

    private void PlatformView_PointerReleased(object sender, PointerRoutedEventArgs e)
    {
        // TODO: Implement better.
        if (DateTime.Now.Ticks - lastPressed >= TimeSpan.TicksPerMillisecond * 500)
        {
            ExecuteCommandIfCan(StatefulView.LongPressCommand);
        }

        VisualStateManager.GoToState(StatefulView, VisualStateManager.CommonStates.Normal);
        ExecuteCommandIfCan(StatefulView.TappedCommand);
    }
}

#endif