using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UraniumUI.Handlers;
public class SelectableLabelHandler : LabelHandler
{
    public SelectableLabelHandler()
    {

    }

#if WINDOWS
    protected override Microsoft.UI.Xaml.Controls.TextBlock CreatePlatformView()
    {
        var textBlock = base.CreatePlatformView();

        textBlock.IsTextSelectionEnabled = true;

        return textBlock;
    }
#endif

#if ANDROID
    protected override AndroidX.AppCompat.Widget.AppCompatTextView CreatePlatformView()
    {
        var textView = base.CreatePlatformView();

        textView.SetTextIsSelectable(true);

        return textView;
    }
#endif

#if IOS || MACCATALYST
    protected override MauiLabel CreatePlatformView()
    {
        var label = base.CreatePlatformView();

        label.UserInteractionEnabled = true;
        label.AddGestureRecognizer(new UIKit.UITapGestureRecognizer(() =>
        {
            label.BecomeFirstResponder();
            var menuController = UIKit.UIMenuController.SharedMenuController;
            var copyMenuItem = new UIKit.UIMenuItem("Copy", new ObjCRuntime.Selector("Copy:"));
            menuController.MenuItems = new[] { copyMenuItem };
            menuController.SetTargetRect(label.Bounds, label);
            menuController.SetMenuVisible(true, true);
        }));

        return label;
    }   
#endif
}
