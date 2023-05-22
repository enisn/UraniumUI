#if IOS || MACCATALYST

using Microsoft.Maui.Controls.Platform;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIKit;

namespace UraniumUI.Handlers;
public partial class SelectableLabelHandler : ViewHandler<ILabel, UITextView>
{
    public static IPropertyMapper<ILabel, SelectableLabelHandler> SelectableLabelMapper =>
        new PropertyMapper<ILabel, SelectableLabelHandler>(ViewHandler.ViewMapper)
        {
            [nameof(ILabel.Font)] = MapFont,
            [nameof(ILabel.LineHeight)] = MapLineHeight,
            [nameof(ILabel.TextDecorations)] = MapTextDecorations,
            [nameof(ILabel.Padding)] = MapPadding,
            [nameof(ILabel.VerticalTextAlignment)] = MapVerticalTextAlignment,
            [nameof(ILabel.HorizontalTextAlignment)] = MapHorizontalTextAlignment,
            [nameof(ILabel.CharacterSpacing)] = MapCharacterSpacing,
            [nameof(ILabel.TextColor)] = MapTextColor,
            [nameof(ILabel.Text)] = MapText,
        };

    public SelectableLabelHandler() : base(SelectableLabelMapper)
    {
        
    }

    protected override UITextView CreatePlatformView()
    {
        return new UITextView()
        {
            BackgroundColor = UIColor.Clear,
            Editable = false,
        };
    }

    public static void MapFont(SelectableLabelHandler handler, ILabel label)
    {
        if (handler.PlatformView is null)
        {
            return;
        }

        var fontManager = handler.Services?.GetRequiredService<IFontManager>();

        handler.PlatformView?.UpdateFont(label, fontManager);
    }
    public static void MapLineHeight(SelectableLabelHandler handler, ILabel label)
    {
        if (handler.PlatformView is null)
        {
            return;
        }

        var modAttrText = handler.PlatformView.AttributedText?.WithLineHeight(label.LineHeight);

        if (modAttrText != null)
            handler.PlatformView.AttributedText = modAttrText;
    }
    public static void MapTextDecorations(SelectableLabelHandler handler, ILabel label)
    {
        if (handler.PlatformView is null)
        {
            return;
        }

        var modAttrText =  handler.PlatformView.AttributedText?.WithDecorations(label.TextDecorations);

        if (modAttrText != null)
            handler.PlatformView.AttributedText = modAttrText;
    }

    public static void MapPadding(SelectableLabelHandler handler, ILabel label)
    {
        if (handler.PlatformView is null)
        {
            return;
        }

        handler.PlatformView.TextContainerInset = new UIEdgeInsets(
                (float)label.Padding.Top,
                (float)label.Padding.Left,
                (float)label.Padding.Bottom,
                (float)label.Padding.Right);
    }
    public static void MapVerticalTextAlignment(SelectableLabelHandler handler, ILabel label)
    {
        //TODO: Make it work.
    }

    public static void MapHorizontalTextAlignment(SelectableLabelHandler handler, ILabel label)
    {
        if (handler.PlatformView is null)
        {
            return;
        }

        handler.PlatformView.TextAlignment = label.HorizontalTextAlignment.ToPlatformHorizontal();
    }

    public static void MapCharacterSpacing(SelectableLabelHandler handler, ILabel label)
    {
        handler.PlatformView?.UpdateCharacterSpacing(label);
    }

    public static void MapTextColor(SelectableLabelHandler handler, ILabel label)
    {
        if (handler.PlatformView is null)
        {
            return;
        }

        handler.PlatformView.TextColor = label.TextColor.ToPlatform();
    }
    public static void MapText(SelectableLabelHandler handler, ILabel label)
    {
        handler.PlatformView.Text = label.Text;

        // Any text update requires that we update any attributed string formatting
        MapFormatting(handler, label);
    }

    public static void MapFormatting(SelectableLabelHandler handler, ILabel label)
    {
        // Update all of the attributed text formatting properties
        handler.UpdateValue(nameof(ILabel.LineHeight));
        handler.UpdateValue(nameof(ILabel.TextDecorations));
        handler.UpdateValue(nameof(ILabel.CharacterSpacing));

        // Setting any of those may have removed text alignment settings,
        // so we need to make sure those are applied, too
        handler.UpdateValue(nameof(ILabel.HorizontalTextAlignment));
    }
}
#endif