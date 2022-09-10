using Microsoft.Maui.Controls;
using Microsoft.Maui.Handlers;
using System.Diagnostics;
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
#if ANDROID
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
        // TODO: Find a better way to do this
        //PlatformView.AddGestureRecognizer(new UIContinousGestureRecognizer(Tapped));
        base.ConnectHandler(platformView);
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
    protected override Microsoft.UI.Xaml.Controls.Button CreatePlatformView()
    {
        var nativeView = base.CreatePlatformView();

        var element = VirtualView as View;

        nativeView.PointerEntered += (s, e) => VisualStateManager.GoToState(element, "Hover");
        nativeView.PointerExited += (s, e) => VisualStateManager.GoToState(element, "Normal");

        nativeView.AddHandler(
            Microsoft.UI.Xaml.Controls.Button.PointerPressedEvent,
            new PointerEventHandler(NativeView_PointerPressed), true);

        nativeView.AddHandler(
            Microsoft.UI.Xaml.Controls.Button.PointerReleasedEvent,
            new PointerEventHandler(NativeView_PointerReleased), true);

        return nativeView;
    }
    private void NativeView_PointerPressed(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
    {
        var element = VirtualView as View;

        VisualStateManager.GoToState(element, "Pressed");
    }

    private void NativeView_PointerReleased(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
    {
        var element = VirtualView as View;

        VisualStateManager.GoToState(element, "Normal");
    }
#endif
}
