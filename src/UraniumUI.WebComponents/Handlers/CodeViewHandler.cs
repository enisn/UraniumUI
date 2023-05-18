using Microsoft.Maui.Handlers;
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
#if WINDOWS
    protected override void ConnectHandler(WebView2 platformView)
    {
        base.ConnectHandler(platformView);
        RegisterEvents();
    }
    protected override void DisconnectHandler(WebView2 platformView)
    {
        base.DisconnectHandler(platformView);
        UnregisterEvents();
    }
#elif ANDROID
    protected override void ConnectHandler(Android.Webkit.WebView platformView)
    {
        base.ConnectHandler(platformView);
        RegisterEvents();
    }
    protected override void DisconnectHandler(Android.Webkit.WebView platformView)
    {
        base.DisconnectHandler(platformView);
        UnregisterEvents();
    }
#elif IOS || MACCATALYST
    protected override void ConnectHandler(WKWebView platformView)
    {
        base.ConnectHandler(platformView);
        RegisterEvents();
    }
    protected override void DisconnectHandler(WKWebView platformView)
    {
        base.DisconnectHandler(platformView);
        UnregisterEvents();
    }
#endif

    protected virtual void RegisterEvents()
    {
        if (this.VirtualView is WebView webView)
        {
            webView.Navigated += WebView_Navigated;
        };
    }

    protected virtual void UnregisterEvents()
    {
        if (this.VirtualView is WebView webView)
        {
            webView.Navigated -= WebView_Navigated;
        };
    }

    private async void WebView_Navigated(object sender, WebNavigatedEventArgs e)
    {
        isInitialized = true;
        await ExecuteScriptAsync($"setLanguage('{CodeView.Language}')");
        await ExecuteScriptAsync($"setContent('{XmlEscape(CodeView.SourceCode)}')");
        await ExecuteScriptAsync($"highlight()");
    }

    protected bool isInitialized;
    protected async Task ExecuteScriptAsync(string javascriptCode)
    {
        if (isInitialized)
        {
#if WINDOWS
            await PlatformView.ExecuteScriptAsync(javascriptCode);
#elif ANDROID
            PlatformView.EvaluateJavascript(javascriptCode, null);
#elif IOS || MACCATALYST
            await PlatformView.EvaluateJavaScriptAsync(javascriptCode);
#endif
        }
    }

    public static string XmlEscape(string unescaped)
    {
        XmlDocument doc = new XmlDocument();
        XmlNode node = doc.CreateElement("root");
        node.InnerText = unescaped;
        return node.InnerXml.Replace("\n", "\\n").Replace("    ", "\\t");
    }
}