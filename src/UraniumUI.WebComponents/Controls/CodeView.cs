
namespace UraniumUI.WebComponents.Controls;
public class CodeView : WebView, ICodeView
{
    public string SourceCode { get => (string)GetValue(SourceCodeProperty); set => SetValue(SourceCodeProperty, value); }

    public static readonly BindableProperty SourceCodeProperty = BindableProperty.Create(
        nameof(SourceCode),
        typeof(string),
        typeof(CodeView),
        default(string));

    public string Language { get => (string)GetValue(LanguageProperty); set => SetValue(LanguageProperty, value); }

    public static readonly BindableProperty LanguageProperty = BindableProperty.Create(
        nameof(Language),
        typeof(string),
        typeof(CodeView),
        default(string));

    public string Theme { get => (string)GetValue(ThemeProperty); set => SetValue(ThemeProperty, value); }

    public static readonly BindableProperty ThemeProperty = BindableProperty.Create(
        nameof(Theme),
        typeof(string),
        typeof(CodeView),
        default(string));

    public CodeView()
    {
        this.BackgroundColor = Colors.Transparent;
        this.Source = (WebViewSource)new WebViewSourceTypeConverter().ConvertFrom("codeview.html");
        this.SetAppTheme(ThemeProperty, "github", "github-dark");
    }
}
