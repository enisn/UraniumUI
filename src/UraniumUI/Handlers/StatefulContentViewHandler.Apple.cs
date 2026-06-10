#if IOS || MACCATALYST
using Foundation;
using System.Diagnostics;
using UraniumUI.Views;
using UIKit;
using static Microsoft.Maui.Controls.VisualStateManager;

namespace UraniumUI.Handlers;
public partial class StatefulContentViewHandler
{
    protected override Microsoft.Maui.Platform.ContentView CreatePlatformView()
    {
        var platformView = new StatefulUIContentView();
        platformView.IsFocusable = StatefulView.IsFocusable;
        return platformView;
    }

    private UIContinousGestureRecognizer _tapRecognizer;
    private UIHoverGestureRecognizer _hoverRecognizer;
    private UILongPressGestureRecognizer _longPressRecognizer;
    private bool _isConnected;

    protected override void ConnectHandler(Microsoft.Maui.Platform.ContentView platformView)
    {
        _tapRecognizer = new UIContinousGestureRecognizer(Tapped);
        platformView.AddGestureRecognizer(_tapRecognizer);
        if (OperatingSystem.IsIOSVersionAtLeast(13))
        {
            _hoverRecognizer = new UIHoverGestureRecognizer(OnHover);
            platformView.AddGestureRecognizer(_hoverRecognizer);
        }
        _longPressRecognizer = new UILongPressGestureRecognizer(OnLongPress);
        platformView.AddGestureRecognizer(_longPressRecognizer);
        _isConnected = true;
        base.ConnectHandler(platformView);
    }

    protected override void DisconnectHandler(Microsoft.Maui.Platform.ContentView platformView)
    {
        _isConnected = false;
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

    private void OnLongPress(UILongPressGestureRecognizer recognizer)
    {
        StatefulView.InvokeLongPressed();
        ExecuteCommandIfCan(StatefulView.LongPressCommand);
    }

    private void OnHover(UIHoverGestureRecognizer recognizer)
    {
        switch (recognizer.State)
        {
            case UIGestureRecognizerState.Began:

                GoToState(StatefulView, CommonStates.PointerOver);
                StatefulView.InvokeHovered();
                ExecuteCommandIfCan(StatefulView.HoverCommand);
                break;
            case UIGestureRecognizerState.Ended:
            case UIGestureRecognizerState.Cancelled:
            case UIGestureRecognizerState.Failed:
                GoToState(StatefulView, CommonStates.Normal);
                StatefulView.InvokeHoverExited();
                ExecuteCommandIfCan(StatefulView.HoverExitCommand);
                break;
        }
    }

    private void Tapped(UIGestureRecognizer recognizer)
    {
        if (!_isConnected)
            return;

        switch (recognizer.State)
        {
            case UIGestureRecognizerState.Began:
                GoToState(StatefulView, "Pressed");
                StatefulView.InvokePressed();
                ExecuteCommandIfCan(StatefulView.PressedCommand);

                break;
            case UIGestureRecognizerState.Ended:
                GoToState(StatefulView, CommonStates.Normal);
                StatefulView.InvokeTapped();
                ExecuteCommandIfCan(StatefulView.TappedCommand);

                //// TODO: Fix working of native gesture recognizers of MAUI
                foreach (var item in StatefulView.GestureRecognizers)
                {
                    Debug.WriteLine(item.GetType().Name);
                    if (item is TapGestureRecognizer tgr)
                    {
                        tgr.Command?.Execute(StatefulView);
                    }
                }

                break;
        }
    }

    // TODO: Move it to the different file
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

    public static void MapIsFocusable(StatefulContentViewHandler handler, StatefulContentView view)
    {
        if (handler.PlatformView is StatefulUIContentView uiView)
        {
            uiView.IsFocusable = view.IsFocusable;
        }
    }

    internal void UpdateFocusable()
    {
        
    }

    // TODO: Move it to the different file
    public class StatefulUIContentView : Microsoft.Maui.Platform.ContentView
    {
        public bool IsFocusable { get; set; }
        public override bool CanBecomeFocused => IsFocusable;
    }
}
#endif
