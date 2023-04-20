using Plainer.Maui.Controls;

namespace UraniumUI.Material.Controls;
public partial class EditorField : InputField
{
    public EditorView EditorView => Content as EditorView;
    
    public override View Content { get; set; } = new EditorView
    {
        Margin = new Thickness(10, 0),
        BackgroundColor = Colors.Transparent,
#if ANDROID
        TranslationY = 5,
#endif
        AutoSize = EditorAutoSizeOption.TextChanges,
    };

    public EditorField()
    {
        EditorView.SetBinding(Editor.TextProperty, new Binding(nameof(Text), source: this));
        EditorView.SetBinding(EditorView.SelectionLengthProperty, new Binding(nameof(SelectionLength), source: this));
        EditorView.SetBinding(EditorView.CursorPositionProperty, new Binding(nameof(CursorPosition), source: this));
    }

    public override bool HasValue { get => !string.IsNullOrEmpty(EditorView?.Text); }

    protected override object GetValueForValidator()
    {
        return EditorView.Text;
    }
}
