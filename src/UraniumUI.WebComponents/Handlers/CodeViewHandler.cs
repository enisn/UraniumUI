using Microsoft.Maui.Controls;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
#if WINDOWS
using Microsoft.UI.Xaml.Controls;
#endif
using System.Xml;
using UraniumUI.WebComponents.Controls;
#if IOS || MACCATALYST
using WebKit;
#endif

namespace UraniumUI.WebComponents.Handlers;
public class CodeViewHandler : WebViewHandler
{
    protected ICodeView CodeView => VirtualView as ICodeView;

    public CodeViewHandler() : base(Mapper)
    {
        Mapper.Add(nameof(ICodeView.SourceCode), MapSourceCode);
        Mapper.Add(nameof(ICodeView.Language), MapLanguageCode);
        Mapper.Add(nameof(ICodeView.Theme), MapTheme);
    }

    private async static void MapLanguageCode(IWebViewHandler handler, IWebView view)
    {
        if (view is ICodeView codeView && handler is CodeViewHandler codeViewHandler)
        {
            await codeViewHandler.ExecuteScriptAsync($"setLanguage('{codeView.Language}')");
            await codeViewHandler.ExecuteScriptAsync($"highlight()");
        }
    }

    private async static void MapSourceCode(IWebViewHandler handler, IWebView view)
    {
        if (view is ICodeView codeView && handler is CodeViewHandler codeViewHandler)
        {
            await codeViewHandler.ExecuteScriptAsync($"setContent('{XmlEscape(codeView.SourceCode)}')");
            await codeViewHandler.ExecuteScriptAsync($"highlight()");
        }
    }

    private async static void MapTheme(IWebViewHandler handler, IWebView view)
    {
        if (view is ICodeView codeView && handler is CodeViewHandler codeViewHandler)
        {
            await codeViewHandler.ExecuteScriptAsync($"setTheme('{codeView.Theme}')");
        }
    }

#if IOS || MACCATALYST
    protected override WebKit.WKWebView CreatePlatformView()
    {
        return base.CreatePlatformView();
    }
    protected override void ConnectHandler(WKWebView platformView)
    {
        base.ConnectHandler(platformView);

        if (this.VirtualView is WebView webView)
        {
            webView.Navigated += async (s, e) =>
            {
                isInitialized = true;
                await ExecuteScriptAsync($"setLanguage('{CodeView.Language}')");
                await ExecuteScriptAsync($"setContent('{XmlEscape(CodeView.SourceCode)}')");
                await ExecuteScriptAsync($"highlight()");
            };
        };
        
    }
    protected bool isInitialized;

#elif WINDOWS
    protected override WebView2 CreatePlatformView()
    {
        var platformView = base.CreatePlatformView();
        platformView.DefaultBackgroundColor = Colors.Transparent.ToWindowsColor();
        
        platformView.NavigationCompleted += PlatformView_NavigationCompleted;

        return platformView;
    }

    private async void PlatformView_NavigationCompleted(WebView2 sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs args)
    {
        if (args.IsSuccess)
        {
            isInitialized = true;
            await ExecuteScriptAsync($"setLanguage('{CodeView.Language}')");
            await ExecuteScriptAsync($"setContent('{XmlEscape(CodeView.SourceCode)}')");
            await ExecuteScriptAsync($"highlight()");
        }
    }

#elif ANDROID
    protected override Android.Webkit.WebView CreatePlatformView()
    {
        return base.CreatePlatformView();
    }
#endif

    protected async Task ExecuteScriptAsync(string javascriptCode)
    {
#if WINDOWS
        if (isInitialized)
        {
            await PlatformView.ExecuteScriptAsync(javascriptCode);
        }
#endif
#if ANDROID
        PlatformView.EvaluateJavascript(javascriptCode, null);
#endif

#if IOS || MACCATALYST
        await PlatformView.EvaluateJavaScriptAsync(javascriptCode);
#endif
    }

    public static string XmlEscape(string unescaped)
    {
        XmlDocument doc = new XmlDocument();
        XmlNode node = doc.CreateElement("root");
        node.InnerText = unescaped;
        return node.InnerXml.Replace("\n", "\\n").Replace("    ", "\\t");
    }
}