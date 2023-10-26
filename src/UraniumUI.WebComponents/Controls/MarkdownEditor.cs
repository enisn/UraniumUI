using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UraniumUI.WebComponents.Controls;
public class MarkdownEditor : WebView, IMarkdownEditor
{
    public MarkdownEditor()
    {
        BackgroundColor = Colors.Transparent;
        Source = (WebViewSource)new WebViewSourceTypeConverter().ConvertFrom("markdowneditor.html");
    }

    public string Content { get; set; }
}
