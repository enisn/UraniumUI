using Microsoft.Maui.Handlers;

namespace UraniumUI.Handlers;
#if WINDOWS
public partial class SelectableLabelHandler : LabelHandler
{
    protected override Microsoft.UI.Xaml.Controls.TextBlock CreatePlatformView()
    {
        var textBlock = base.CreatePlatformView();

        textBlock.IsTextSelectionEnabled = true;

        return textBlock;
    }
}
#endif

#if ANDROID
public partial class SelectableLabelHandler : LabelHandler
{
    protected override AndroidX.AppCompat.Widget.AppCompatTextView CreatePlatformView()
    {
        var textView = base.CreatePlatformView();

        textView.SetTextIsSelectable(true);

        return textView;
    }
}
#endif

#if !ANDROID && !WINDOWS && !IOS && !MACCATALYST

public partial class SelectableLabelHandler : LabelHandler
{

}
#endif