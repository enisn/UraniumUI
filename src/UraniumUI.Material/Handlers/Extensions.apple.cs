#if IOS || MACCATALYST

using System;
using System.Diagnostics;
using UIKit;

namespace UraniumUI.Material.Handlers;

public static class Extensions
{
    private static readonly Dictionary<ToUIFontKey, UIFont> _toUiFont = new Dictionary<ToUIFontKey, UIFont>();

    public static bool IsDefault(this Span self)
    {
        return self.FontFamily == null &&
               self.FontAttributes == FontAttributes.None;
    }
    // literally ripped this off from X.Forms source.
    public static T FindDescendantView<T>(this UIView view) where T : UIView
    {
        var queue = new Queue<UIView>();
        queue.Enqueue(view);

        while (queue.Count > 0)
        {
            var descendantView = queue.Dequeue();

            var result = descendantView as T;
            if (result != null)
                return result;

            for (var i = 0; i < descendantView.Subviews.Length; i++)
                queue.Enqueue(descendantView.Subviews[i]);
        }

        return null;
    }
    public static UIWindow GetTopWindow(this UIApplication app)
    {
        return app.Windows.Reverse().FirstOrDefault(x => x.WindowLevel == UIWindowLevel.Normal && !x.Hidden);
    }

    public static UIView GetTopView(this UIApplication app)
    {
        return app.GetTopWindow().Subviews.Last();
    }

    public static UIViewController GetTopViewController(this UIApplication app)
    {
        var viewController = app.KeyWindow.RootViewController;
        while (viewController.PresentedViewController != null)
            viewController = viewController.PresentedViewController;

        return viewController;
    }
    public static UIKeyboardType ToNative(this Keyboard input)
    {
        if (input == Keyboard.Url)
        {
            return UIKeyboardType.Url;
        }
        if (input == Keyboard.Email)
        {
            return UIKeyboardType.EmailAddress;
        }
        if (input == Keyboard.Numeric)
        {
            return UIKeyboardType.NumberPad;
        }
        if (input == Keyboard.Chat)
        {
            return UIKeyboardType.Default;
        }
        if (input == Keyboard.Telephone)
        {
            return UIKeyboardType.PhonePad;
        }
        if (input == Keyboard.Text)
        {
            return UIKeyboardType.Default;
        }
        return UIKeyboardType.Default;
    }

    public static UIFont ToUIFont(this Entry element)
    {
        return ToUIFont(element.FontFamily, (float)element.FontSize, element.FontAttributes);
    }

    private static UIFont _ToUIFont(string family, float size, FontAttributes attributes)
    {
        var bold = (attributes & FontAttributes.Bold) != 0;
        var italic = (attributes & FontAttributes.Italic) != 0;

        if (family != null)
            try
            {
                UIFont result;

                if (UIFont.FamilyNames.Contains(family))
                {
                    var descriptor = new UIFontDescriptor().CreateWithFamily(family);

                    if (bold || italic)
                    {
                        var traits = (UIFontDescriptorSymbolicTraits)0;
                        if (bold)
                            traits = traits | UIFontDescriptorSymbolicTraits.Bold;
                        if (italic)
                            traits = traits | UIFontDescriptorSymbolicTraits.Italic;

                        descriptor = descriptor.CreateWithTraits(traits);
                        result = UIFont.FromDescriptor(descriptor, size);
                        if (result != null)
                            return result;
                    }
                }

                result = UIFont.FromName(family, size);

                if (result != null)
                    return result;
            }
            catch
            {
                Debug.WriteLine("Could not load font named: {0}", family);
            }

        if (bold && italic)
        {
            var defaultFont = UIFont.SystemFontOfSize(size);


            var descriptor = defaultFont.FontDescriptor.CreateWithTraits(UIFontDescriptorSymbolicTraits.Bold | UIFontDescriptorSymbolicTraits.Italic);
            return UIFont.FromDescriptor(descriptor, 0);
        }
        if (italic)
            return UIFont.ItalicSystemFontOfSize(size);

        if (bold)
            return UIFont.BoldSystemFontOfSize(size);

        return UIFont.SystemFontOfSize(size);
    }

    private static UIFont ToUIFont(string family, float size, FontAttributes attributes)
    {
        var key = new ToUIFontKey(family, size, attributes);

        lock (_toUiFont)
        {
            UIFont value;
            if (_toUiFont.TryGetValue(key, out value))
                return value;
        }

        var generatedValue = _ToUIFont(family, size, attributes);

        lock (_toUiFont)
        {
            UIFont value;
            if (!_toUiFont.TryGetValue(key, out value))
                _toUiFont.Add(key, value = generatedValue);
            return value;
        }
    }

    private struct ToUIFontKey
    {
        internal ToUIFontKey(string family, float size, FontAttributes attributes)
        {
            _family = family;
            _size = size;
            _attributes = attributes;
        }
#pragma warning disable 0414 // these are not called explicitly, but they are used to establish uniqueness. allow it!
        // ReSharper disable once NotAccessedField.Local
        private string _family;

        // ReSharper disable once NotAccessedField.Local
        private float _size;

        // ReSharper disable once NotAccessedField.Local
        private FontAttributes _attributes;
#pragma warning restore 0414
    }
}
#endif
