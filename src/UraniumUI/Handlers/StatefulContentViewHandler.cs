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

namespace UraniumUI.Handlers;

/// <summary>
/// A handler for <see cref="StatefulContentView"/>.
/// </summary>
public class StatefulContentViewHandler : ContentViewHandler
{
    public StatefulContentView StatefulView => VirtualView as StatefulContentView;
#if ANDROID
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
#if NET6_0
            GoToState(StatefulView, "PointerOver");
#else
            GoToState(StatefulView, CommonStates.PointerOver);
#endif
            ExecuteCommandIfCan(StatefulView.HoverCommand);
            return;
        }

        if (e.Event.Action == MotionEventActions.HoverExit)
        {
            GoToState(StatefulView, CommonStates.Normal);
            ExecuteCommandIfCan(StatefulView.HoverExitCommand);
        }
    }

    private void OnTouch(object sender, Android.Views.View.TouchEventArgs e)
    {
        if (e.Event.Action == MotionEventActions.Down)
        {
            GoToState(StatefulView, "Pressed");
            ExecuteCommandIfCan(StatefulView.PressedCommand);
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
    }

    private void PlatformView_LongClick(object sender, Android.Views.View.LongClickEventArgs e)
    {
        ExecuteCommandIfCan(StatefulView.LongPressCommand);
    }
#endif

#if WINDOWS
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
        return platformView;
    }

    protected override void DisconnectHandler(ContentPanel platformView)
    {
        platformView.PointerEntered -= PlatformView_PointerEntered;
        platformView.PointerExited -= PlatformView_PointerExited;
        base.DisconnectHandler(platformView);
    }

    private void PlatformView_PointerExited(object sender, PointerRoutedEventArgs e)
    {
        GoToState(StatefulView, CommonStates.Normal);
        ExecuteCommandIfCan(StatefulView.HoverExitCommand);
    }

    private void PlatformView_PointerEntered(object sender, PointerRoutedEventArgs e)
    {
#if NET6_0
        GoToState(StatefulView, "PointerOver");
#else
        GoToState(StatefulView, CommonStates.PointerOver);
#endif
        ExecuteCommandIfCan(StatefulView.HoverCommand);
    }

    long lastPressed;

    private void PlatformView_PointerPressed(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
    {
        lastPressed = DateTime.Now.Ticks;

        GoToState(StatefulView, "Pressed");
        ExecuteCommandIfCan(StatefulView.PressedCommand);
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
    }
#endif

#if IOS || MACCATALYST
    protected override void ConnectHandler(Microsoft.Maui.Platform.ContentView platformView)
    {
        platformView.AddGestureRecognizer(new UIContinousGestureRecognizer(Tapped));
        platformView.AddGestureRecognizer(new UIHoverGestureRecognizer(OnHover));
        platformView.AddGestureRecognizer(new UILongPressGestureRecognizer(OnLongPress));
        base.ConnectHandler(platformView);
    }

    private void OnLongPress(UILongPressGestureRecognizer recognizer)
    {
        ExecuteCommandIfCan(StatefulView.LongPressCommand);
    }

    private void OnHover(UIHoverGestureRecognizer recognizer)
    {
        switch (recognizer.State)
        {
            case UIGestureRecognizerState.Began:

#if NET6_0
                GoToState(StatefulView, "PointerOver");
#else
                GoToState(StatefulView, CommonStates.PointerOver);
#endif
                ExecuteCommandIfCan(StatefulView.HoverCommand);
                break;
            case UIGestureRecognizerState.Ended:
            case UIGestureRecognizerState.Cancelled:
            case UIGestureRecognizerState.Failed:
                GoToState(StatefulView, CommonStates.Normal);
                ExecuteCommandIfCan(StatefulView.HoverExitCommand);
                break;
        }
    }

    private void Tapped(UIGestureRecognizer recognizer)
    {
        switch (recognizer.State)
        {
            case UIGestureRecognizerState.Began:
                GoToState(StatefulView, "Pressed");
                ExecuteCommandIfCan(StatefulView.PressedCommand);

                break;
            case UIGestureRecognizerState.Ended:
                GoToState(StatefulView, CommonStates.Normal);
                ExecuteCommandIfCan(StatefulView.TappedCommand);

                //// TODO: Fix working of native gesture recognizers of MAUI
                foreach (var item in StatefulView.GestureRecognizers)
                {
                    Debug.WriteLine(item.GetType().Name);
                    if (item is TapGestureRecognizer tgr)
                    {
                        tgr.Command.Execute(StatefulView);
                    }
                }

                break;
        }
    }

    internal class UIContinousGestureRecognizer : UIGestureRecognizer
    {
        private readonly Action<UIGestureRecognizer> action;

        public UIContinousGestureRecognizer(Action<UIGestureRecognizer> action)
        {
            this.action = action;
        }
        
        public override void TouchesBegan(NSSet touches, UIEvent evt)
        {
            State = UIGestureRecognizerState.Began;

            action(this);

            base.TouchesBegan(touches, evt);
        }

        public override void TouchesEnded(NSSet touches, UIEvent evt)
        {
            State = UIGestureRecognizerState.Ended;

            action(this);

            base.TouchesEnded(touches, evt);
        }
    }
#endif

    private void ExecuteCommandIfCan(ICommand command)
    {
        if (command?.CanExecute(StatefulView.CommandParameter) ?? false)
        {
            command.Execute(StatefulView.CommandParameter);
        }
    }
}
