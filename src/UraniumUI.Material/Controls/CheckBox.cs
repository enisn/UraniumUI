using Microsoft.Maui.Platform;
#if WINDOWS
using Microsoft.UI.Xaml.Controls;
#endif

namespace UraniumUI.Material.Controls;

[ContentProperty(nameof(Validations))]
public class CheckBox : InputKit.Shared.Controls.CheckBox
{
    public CheckBox()
    {
    }

#if WINDOWS
    protected override void OnHandlerChanged()
    {
        base.OnHandlerChanged();
        if (Handler != null)
        {
            if (Handler.PlatformView is Panel contentPanel)
            {
                contentPanel.IsTabStop = true;
                contentPanel.UseSystemFocusVisuals = true;
                contentPanel.KeyDown += PlatformView_KeyDown;
                contentPanel.KeyUp += PlatformView_KeyUp;
            }
        }
        else
        {
            if (Handler.PlatformView is Panel contentPanel)
            {
                contentPanel.KeyDown -= PlatformView_KeyDown;
                contentPanel.KeyUp -= PlatformView_KeyUp;
            }
        }
    }

    private void PlatformView_KeyDown(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
    {
        if (IsActionKey(e.Key))
        {
            VisualStateManager.GoToState(this, "Pressed");
        }
    }

    private void PlatformView_KeyUp(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
    {
        if (IsActionKey(e.Key))
        {
            if (!IsDisabled)
            {
                IsChecked = !IsChecked;
            }
            VisualStateManager.GoToState(this, VisualStateManager.CommonStates.Normal);
        }
    }

    private bool IsActionKey(Windows.System.VirtualKey key)
    {
        return key == Windows.System.VirtualKey.Enter || key == Windows.System.VirtualKey.Space;
    }
#endif
}
