#if IOS || MACCATALYST
using Foundation;
using System.Diagnostics;
using UIKit;

namespace UraniumUI.Material.Handlers;
public partial class ButtonViewHandler
{
    private UIContinousGestureRecognizer _tapRecognizer;
    private UIHoverGestureRecognizer _hoverRecognizer;
    private UILongPressGestureRecognizer _longPressRecognizer;

    protected override void ConnectHandler(Microsoft.Maui.Platform.ContentView platformView)
    {
        base.ConnectHandler(platformView);

        _tapRecognizer = new UIContinousGestureRecognizer(Tapped)
        {
            CancelsTouchesInView = false,
            Delegate = new IgnoreInteractiveChildTouchesGestureDelegate(platformView)
        };

        platformView.AddGestureRecognizer(_tapRecognizer);
        if (OperatingSystem.IsIOSVersionAtLeast(13))
        {
            _hoverRecognizer = new UIHoverGestureRecognizer(OnHover);
            platformView.AddGestureRecognizer(_hoverRecognizer);
        }

        _longPressRecognizer = new UILongPressGestureRecognizer(OnLongPress)
        {
            CancelsTouchesInView = false,
            Delegate = new IgnoreInteractiveChildTouchesGestureDelegate(platformView)
        };

        platformView.AddGestureRecognizer(_longPressRecognizer);
    }

    protected override void DisconnectHandler(Microsoft.Maui.Platform.ContentView platformView)
    {
        if (_tapRecognizer != null)
        {
            platformView.RemoveGestureRecognizer(_tapRecognizer);
            _tapRecognizer = null;
        }

        if (_hoverRecognizer != null)
        {
            platformView.RemoveGestureRecognizer(_hoverRecognizer);
            _hoverRecognizer = null;
        }

        if (_longPressRecognizer != null)
        {
            platformView.RemoveGestureRecognizer(_longPressRecognizer);
            _longPressRecognizer = null;
        }

        base.DisconnectHandler(platformView);
    }

    internal sealed class IgnoreInteractiveChildTouchesGestureDelegate(UIView ownerView) : UIGestureRecognizerDelegate
    {
        public override bool ShouldReceiveTouch(UIGestureRecognizer recognizer, UITouch touch)
        {
            var view = touch.View;

            while (view is not null && view != ownerView)
            {
                if (view is UIControl)
                {
                    return false;
                }

                view = view.Superview;
            }

            return true;
        }
    }

    private void OnLongPress(UILongPressGestureRecognizer recognizer)
    {
        ExecuteCommandIfCan(StatefulView.LongPressCommand);
        StatefulView.InvokeLongPressed();
    }

    private void OnHover(UIHoverGestureRecognizer recognizer)
    {
        switch (recognizer.State)
        {
            case UIGestureRecognizerState.Began:

                VisualStateManager.GoToState(StatefulView, VisualStateManager.CommonStates.PointerOver);
                ExecuteCommandIfCan(StatefulView.HoverCommand);
                StatefulView.InvokeHovered();
                break;
            case UIGestureRecognizerState.Ended:
            case UIGestureRecognizerState.Cancelled:
            case UIGestureRecognizerState.Failed:
                VisualStateManager.GoToState(StatefulView, VisualStateManager.CommonStates.Normal);
                ExecuteCommandIfCan(StatefulView.HoverExitCommand);
                StatefulView.InvokeHoverExited();
                break;
        }
    }

    private void Tapped(UIGestureRecognizer recognizer)
    {
        switch (recognizer.State)
        {
            case UIGestureRecognizerState.Began:
                VisualStateManager.GoToState(StatefulView, "Pressed");
                ExecuteCommandIfCan(StatefulView.PressedCommand);
                StatefulView.InvokePressed();

                break;
            case UIGestureRecognizerState.Ended:
                VisualStateManager.GoToState(StatefulView, VisualStateManager.CommonStates.Normal);
                ExecuteCommandIfCan(StatefulView.TappedCommand);
                StatefulView.InvokeTapped();

                //// TODO: Fix working of native gesture recognizers of MAUI
                foreach (var item in StatefulView.GestureRecognizers)
                {
                    Debug.WriteLine(item.GetType().Name + " is executing manually by " + this.GetType().Name);
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
}
#endif
