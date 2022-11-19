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
        return platformView;
    }

    protected override void DisconnectHandler(ContentViewGroup platformView)
    {
        platformView.Touch -= OnTouch;
        platformView.Hover -= NativeView_Hover;
        base.DisconnectHandler(platformView);
    }

    private void NativeView_Hover(object sender, Android.Views.View.HoverEventArgs e)
    {
        if (e.Event.Action == MotionEventActions.HoverEnter)
        {
            GoToState(StatefulView, CommonStates.PointerOver);
            StatefulView.HoverCommand?.Execute(StatefulView.CommandParameter);
            return;
        }

        if (e.Event.Action == MotionEventActions.HoverExit)
        {
            GoToState(StatefulView, CommonStates.Normal);
            StatefulView.HoverExitCommand?.Execute(StatefulView.CommandParameter);
        }
    }

    private void OnTouch(object sender, Android.Views.View.TouchEventArgs e)
    {
        if (e.Event.Action == MotionEventActions.Down)
        {
            GoToState(StatefulView, "Pressed");
            StatefulView.PressedCommand?.Execute(StatefulView.CommandParameter);
        }
        else if (e.Event.Action == MotionEventActions.Up || e.Event.Action == MotionEventActions.Cancel)
        {
            GoToState(StatefulView, CommonStates.Normal);
            StatefulView.TappedCommand?.Execute(StatefulView.CommandParameter);
        }
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
        StatefulView.HoverExitCommand?.Execute(StatefulView.CommandParameter);
    }

    private void PlatformView_PointerEntered(object sender, PointerRoutedEventArgs e)
    {
        GoToState(StatefulView, CommonStates.PointerOver);
        StatefulView.HoverCommand?.Execute(StatefulView.CommandParameter);
    }

    private void PlatformView_PointerPressed(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
    {
        GoToState(StatefulView, "Pressed");
        StatefulView.PressedCommand?.Execute(StatefulView.CommandParameter);
    }
    private void PlatformView_PointerReleased(object sender, PointerRoutedEventArgs e)
    {
        GoToState(StatefulView, CommonStates.Normal);
        StatefulView.TappedCommand?.Execute(StatefulView.CommandParameter);
    }
#endif

#if IOS || MACCATALYST
    protected override void ConnectHandler(Microsoft.Maui.Platform.ContentView platformView)
    {
        platformView.AddGestureRecognizer(new UIContinousGestureRecognizer(Tapped));
        platformView.AddGestureRecognizer(new UIHoverGestureRecognizer(OnHover));
        base.ConnectHandler(platformView);
    }

    private void OnHover(UIHoverGestureRecognizer recognizer)
    {
        switch (recognizer.State)
        {
            case UIGestureRecognizerState.Began:
                GoToState(StatefulView, CommonStates.PointerOver);
                StatefulView.HoverCommand?.Execute(StatefulView.CommandParameter);
                break;
            case UIGestureRecognizerState.Ended:
            case UIGestureRecognizerState.Cancelled:
            case UIGestureRecognizerState.Failed:
                GoToState(StatefulView, CommonStates.Normal);
                StatefulView.HoverExitCommand?.Execute(StatefulView.CommandParameter);
                break;
        }
    }

    private void Tapped(UIGestureRecognizer recognizer)
    {
        switch (recognizer.State)
        {
            case UIGestureRecognizerState.Began:
                GoToState(StatefulView, "Pressed");
                StatefulView.PressedCommand?.Execute(StatefulView.CommandParameter);

                break;
            case UIGestureRecognizerState.Ended:
                GoToState(StatefulView, CommonStates.Normal);
                StatefulView.TappedCommand?.Execute(StatefulView.CommandParameter);

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
}
