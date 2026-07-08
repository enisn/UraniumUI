using Microsoft.Maui.Controls;
using Microsoft.Maui.Handlers;
using System.Diagnostics;
using static Microsoft.Maui.Controls.VisualStateManager;
#if MACCATALYST || IOS
using UIKit;
using Foundation;
#endif
#if ANDROID
using Android.Views;
using Google.Android.Material.Button;
#endif
#if WINDOWS
using Microsoft.UI.Xaml.Input;
#endif

namespace UraniumUI.Handlers;
public class StatefulButtonHandler : ButtonHandler
{
    // Navigation can interrupt a press before the native release event arrives.
    private void ResetState()
    {
        if (VirtualView is Microsoft.Maui.Controls.View element)
        {
            GoToStateIfConnected(element, CommonStates.Normal);
        }
    }

    private void GoToStateIfConnected(Microsoft.Maui.Controls.View element, string state)
    {
        if (element.Handler == this && PlatformView is not null)
        {
            GoToState(element, state);
        }
    }

    private void ConnectVirtualViewEvents()
    {
        if (VirtualView is Button button)
        {
            button.Unloaded -= VirtualView_Unloaded;
            button.Unloaded += VirtualView_Unloaded;
        }
    }

    private void DisconnectVirtualViewEvents()
    {
        if (VirtualView is Button button)
        {
            button.Unloaded -= VirtualView_Unloaded;
        }
    }

    private void VirtualView_Unloaded(object sender, EventArgs e) => ResetState();

#if ANDROID
    protected override void ConnectHandler(MaterialButton platformView)
    {
        base.ConnectHandler(platformView);
        ConnectVirtualViewEvents();
    }

    protected override void DisconnectHandler(MaterialButton platformView)
    {
        platformView.Touch -= OnTouch;
        DisconnectVirtualViewEvents();
        ResetState();
        base.DisconnectHandler(platformView);
    }

    protected override MaterialButton CreatePlatformView()
    {
        var button = base.CreatePlatformView();

        button.Touch += OnTouch;

        return button;
    }

    private void OnTouch(object sender, global::Android.Views.View.TouchEventArgs e)
    {
        var element = VirtualView as Button;

        if (e.Event.Action == MotionEventActions.Down)
        {
            Microsoft.Maui.Controls.VisualStateManager.GoToState(element, "Pressed");
        }
        else if (e.Event.Action == MotionEventActions.Up || e.Event.Action == MotionEventActions.Cancel)
        {
            Microsoft.Maui.Controls.VisualStateManager.GoToState(element, "Normal");
        }
    }
#endif

#if MACCATALYST || IOS
    protected override void ConnectHandler(UIButton platformView)
    {
        base.ConnectHandler(platformView);
        ConnectVirtualViewEvents();

        // TODO: Find a better way to do this
        if (OperatingSystem.IsIOSVersionAtLeast(13))
        {
            PlatformView.AddGestureRecognizer(new UIHoverGestureRecognizer(OnHover));
        }
        //PlatformView.AddGestureRecognizer(new UIContinousGestureRecognizer(Tapped));
    }

    protected override void DisconnectHandler(UIButton platformView)
    {
        DisconnectVirtualViewEvents();
        ResetState();
        base.DisconnectHandler(platformView);
    }

    private void Tapped(UIGestureRecognizer recognizer)
    {
        var element = VirtualView as View;
        switch (recognizer.State)
        {
            case UIGestureRecognizerState.Began:
                VisualStateManager.GoToState(element, "Pressed");

                break;
            case UIGestureRecognizerState.Ended:
                VisualStateManager.GoToState(element, "Normal");

                //// TODO: Fix working of native gesture recognizers of MAUI
                foreach (var item in element.GestureRecognizers)
                {
                    Debug.WriteLine(item.GetType().Name);
                    if (item is TapGestureRecognizer tgr)
                    {
                        tgr.Command.Execute(element);
                    }
                }

                break;
        }
    }

    private void OnHover(UIHoverGestureRecognizer recognizer)
    {
        switch (recognizer.State)
        {
            case UIGestureRecognizerState.Began:
                GoToState(VirtualView as View, "Hover");
                break;
            case UIGestureRecognizerState.Ended:
            case UIGestureRecognizerState.Cancelled:
            case UIGestureRecognizerState.Failed:
                GoToState(VirtualView as View, CommonStates.Normal);
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

#if WINDOWS
    protected override void ConnectHandler(Microsoft.UI.Xaml.Controls.Button platformView)
    {
        base.ConnectHandler(platformView);
        ConnectVirtualViewEvents();

        platformView.PointerCanceled += NativeView_PointerCanceled;
        platformView.PointerCaptureLost += NativeView_PointerCaptureLost;
        platformView.Unloaded += NativeView_Unloaded;
    }

    protected override void DisconnectHandler(Microsoft.UI.Xaml.Controls.Button platformView)
    {
        platformView.PointerEntered -= NativeView_PointerEntered;
        platformView.PointerExited -= NativeView_PointerExited;
        platformView.PointerCanceled -= NativeView_PointerCanceled;
        platformView.PointerCaptureLost -= NativeView_PointerCaptureLost;
        platformView.Unloaded -= NativeView_Unloaded;
        platformView.RemoveHandler(
            Microsoft.UI.Xaml.Controls.Button.PointerPressedEvent,
            new PointerEventHandler(NativeView_PointerPressed));
        platformView.RemoveHandler(
            Microsoft.UI.Xaml.Controls.Button.PointerReleasedEvent,
            new PointerEventHandler(NativeView_PointerReleased));

        DisconnectVirtualViewEvents();
        ResetState();
        base.DisconnectHandler(platformView);
    }

    protected override Microsoft.UI.Xaml.Controls.Button CreatePlatformView()
    {
        var nativeView = base.CreatePlatformView();

        nativeView.PointerEntered += NativeView_PointerEntered;
        nativeView.PointerExited += NativeView_PointerExited;

        nativeView.AddHandler(
            Microsoft.UI.Xaml.Controls.Button.PointerPressedEvent,
            new PointerEventHandler(NativeView_PointerPressed), true);

        nativeView.AddHandler(
            Microsoft.UI.Xaml.Controls.Button.PointerReleasedEvent,
            new PointerEventHandler(NativeView_PointerReleased), true);

        return nativeView;
    }

    private void NativeView_PointerEntered(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
    {
        if (VirtualView is View element)
        {
            GoToStateIfConnected(element, "Hover");
        }
    }

    private void NativeView_PointerExited(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
    {
        if (VirtualView is View element)
        {
            GoToStateIfConnected(element, "Normal");
        }
    }

    private void NativeView_PointerPressed(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
    {
        if (VirtualView is View element)
        {
            GoToStateIfConnected(element, "Pressed");
        }
    }

    private void NativeView_PointerReleased(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
    {
        if (VirtualView is View element)
        {
            GoToStateIfConnected(element, "Normal");
        }
    }

    private void NativeView_PointerCanceled(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e) => ResetState();

    private void NativeView_PointerCaptureLost(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e) => ResetState();

    private void NativeView_Unloaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e) => ResetState();
#endif
}
