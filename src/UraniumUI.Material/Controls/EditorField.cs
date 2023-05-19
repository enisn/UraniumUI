using Microsoft.Maui.Platform;
using Plainer.Maui.Controls;
using UraniumUI.Resources;

namespace UraniumUI.Material.Controls;
public partial class EditorField : InputField
{
    public EditorView EditorView => Content as EditorView;
    
    public override View Content { get; set; } = new EditorView
    {
        Margin = new Thickness(10, 0),
        BackgroundColor = Colors.Transparent,
        VerticalOptions = LayoutOptions.Center,
        AutoSize = EditorAutoSizeOption.TextChanges,
    };

    public EditorField()
    {
        EditorView.SetBinding(Editor.TextProperty, new Binding(nameof(Text), source: this));
        EditorView.SetBinding(EditorView.SelectionLengthProperty, new Binding(nameof(SelectionLength), source: this));
        EditorView.SetBinding(EditorView.CursorPositionProperty, new Binding(nameof(CursorPosition), source: this));
    }

    protected override void OnHandlerChanged()
    {

#if WINDOWS
        if (EditorView.Handler.PlatformView is Microsoft.UI.Xaml.Controls.TextBox textBox)
        {
            textBox.AcceptsReturn = true;
            textBox.TextWrapping = Microsoft.UI.Xaml.TextWrapping.Wrap;
            textBox.SelectionHighlightColor = new Microsoft.UI.Xaml.Media.SolidColorBrush(ColorResource.GetColor("Primary", "PrimaryDark", Colors.Purple).ToWindowsColor());

            textBox.BorderThickness = new Microsoft.UI.Xaml.Thickness(0);

            textBox.Style = null;
        }
#endif
    }

    public override bool HasValue { get => !string.IsNullOrEmpty(EditorView?.Text); }

    protected override object GetValueForValidator()
    {
        return EditorView.Text;
    }
}
