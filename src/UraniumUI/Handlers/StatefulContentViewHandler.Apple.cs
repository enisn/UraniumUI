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

    protected override void ConnectHandler(Microsoft.Maui.Platform.ContentView platformView)
    {
        platformView.AddGestureRecognizer(new UIContinousGestureRecognizer(Tapped));
        if (OperatingSystem.IsIOSVersionAtLeast(13))
        {
            platformView.AddGestureRecognizer(new UIHoverGestureRecognizer(OnHover));
        }
        platformView.AddGestureRecognizer(new UILongPressGestureRecognizer(OnLongPress));
        base.ConnectHandler(platformView);
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
        var statefulView = StatefulView;
        if (statefulView is null)
        {
            return;
        }

        switch (recognizer.State)
        {
            case UIGestureRecognizerState.Began:
                GoToState(statefulView, "Pressed");
                statefulView.InvokePressed();
                ExecuteCommandIfCan(statefulView.PressedCommand);

                break;
            case UIGestureRecognizerState.Ended:
                // The tap handler may remove the view from the tree, which disconnects the handler.
                var tapGestureRecognizers = statefulView.GestureRecognizers.OfType<TapGestureRecognizer>().ToArray();

                GoToState(statefulView, CommonStates.Normal);
                statefulView.InvokeTapped();
                ExecuteCommandIfCan(statefulView.TappedCommand);

                //// TODO: Fix working of native gesture recognizers of MAUI
                foreach (var item in tapGestureRecognizers)
                {
                    Debug.WriteLine(item.GetType().Name);
                    item.Command?.Execute(statefulView);
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
