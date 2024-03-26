using System.Windows.Input;

namespace UraniumUI.Views;
public class StatefulContentView : ContentView, IStatefulView
{
    public event EventHandler<EventArgs> Pressed;
    public event EventHandler<EventArgs> LongPressed;
    public event EventHandler<EventArgs> Hovered;
    public event EventHandler<EventArgs> HoverExited;
    public event EventHandler<EventArgs> Tapped;

    #region Focus Handling

    protected override void OnHandlerChanged()
    {
        base.OnHandlerChanged();

        UpdateFocus();
    }

    protected void UpdateFocus()
    {
        if (UpdateFocusWindows()) return;
        if (UpdateFocusMACorIOS()) return;
        if (UpdateFocusAndroid26OrHigher()) return;
        UpdateFocusGeneric();
    }

    protected bool UpdateFocusWindows()
    {
#if WINDOWS
        var view = Handler?.PlatformView as Microsoft.Maui.Platform.ContentPanel;

        if (view != null)
        {
            view.IsTabStop = !IsNotFocusable;

            return true;
        }

#endif
        return false;
    }

    protected bool UpdateFocusMACorIOS()
    {
#if IOS || MACCATALYST
            var view = Handler?.PlatformView as UIKit.UIView;

            if (view != null)
            {
                view.ExclusiveTouch = IsNotFocusable;

                return true;
            }
#endif
        return false;
    }

    protected bool UpdateFocusAndroid26OrHigher()
    {
#if ANDROID26_0_OR_GREATER
        var view = Handler?.PlatformView as PlatformContentViewGroup;

        if (view != null)
        {
#pragma warning disable CA1416 // Validate platform compatibility
            view.SetFocusable(IsNotFocusable ? Android.Views.ViewFocusability.NotFocusable : Android.Views.ViewFocusability.FocusableAuto);

            return true;
#pragma warning restore CA1416 // Validate platform compatibility
        }
#endif
        return false;
    }

    protected void UpdateFocusGeneric()
    {
        //Fallback to focused event
        if (Handler == null)
        {
            Focused -= StatefulContentView_Focused;
        }
        else
        {
            Focused += StatefulContentView_Focused;
        }
    }

    private void StatefulContentView_Focused(object sender, FocusEventArgs e)
    {
        if (IsNotFocusable)
        {
            var controlToFocus = UraniumUI.Extensions.ViewExtensions.GetNextFocusableElement(Parent, this) as IView;

            if (controlToFocus != null)
            {
                //Attempt to focus, I guess just ignore failures for now
                //Maybe loop to next until we find ourselves?
                controlToFocus.Focus();
            }
        }
    }

    #endregion Focus Handling

    internal void InvokePressed() => Pressed?.Invoke(this, EventArgs.Empty);

    internal void InvokeLongPressed() => LongPressed?.Invoke(this, EventArgs.Empty);

    internal void InvokeHovered() => Hovered?.Invoke(this, EventArgs.Empty);

    internal void InvokeHoverExited() => HoverExited?.Invoke(this, EventArgs.Empty);

    internal void InvokeTapped() => Tapped?.Invoke(this, EventArgs.Empty);

    public ICommand PressedCommand { get => (ICommand)GetValue(PressedCommandProperty); set => SetValue(PressedCommandProperty, value); }

    public static BindableProperty PressedCommandProperty = BindableProperty.Create(nameof(PressedCommand), typeof(ICommand), typeof(StatefulContentView));

    public ICommand HoverCommand { get => (ICommand)GetValue(HoverCommandProperty); set => SetValue(HoverCommandProperty, value); }

    public static BindableProperty HoverCommandProperty = BindableProperty.Create(nameof(HoverCommand), typeof(ICommand), typeof(StatefulContentView));

    public ICommand HoverExitCommand { get => (ICommand)GetValue(HoverExitCommandProperty); set => SetValue(HoverExitCommandProperty, value); }

    public static BindableProperty HoverExitCommandProperty = BindableProperty.Create(nameof(HoverExitCommand), typeof(ICommand), typeof(StatefulContentView));

    public ICommand LongPressCommand { get => (ICommand)GetValue(LongPressCommandProperty); set => SetValue(LongPressCommandProperty, value); }

    public static BindableProperty LongPressCommandProperty = BindableProperty.Create(nameof(LongPressCommand), typeof(ICommand), typeof(StatefulContentView));

    public ICommand TappedCommand { get => (ICommand)GetValue(TappedCommandProperty); set => SetValue(TappedCommandProperty, value); }

    public static BindableProperty TappedCommandProperty = BindableProperty.Create(nameof(TappedCommand), typeof(ICommand), typeof(StatefulContentView));

    public object CommandParameter { get => GetValue(CommandParameterProperty); set => SetValue(CommandParameterProperty, value); }

    public static BindableProperty CommandParameterProperty = BindableProperty.Create(nameof(CommandParameter), typeof(object), typeof(StatefulContentView));

    public bool IsNotFocusable { get => (bool)GetValue(IsNotFocusableProperty); set => SetValue(IsNotFocusableProperty, value); }

    public static BindableProperty IsNotFocusableProperty = BindableProperty.Create(nameof(IsNotFocusable), typeof(bool), typeof(StatefulContentView), false, BindingMode.TwoWay);
}
