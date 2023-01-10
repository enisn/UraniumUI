using Plainer.Maui.Controls;

namespace UraniumUI.Material.Controls;
public class EditorField : InputField
{
    public EditorView EditorView => Content as EditorView;
    
    public override View Content { get; set; } = new EditorView
    {
        Margin = new Thickness(10, 0),
        BackgroundColor = Colors.Transparent,
        VerticalOptions = LayoutOptions.Center,
        AutoSize = EditorAutoSizeOption.TextChanges,
    };
    public override bool HasValue { get => !string.IsNullOrEmpty(EditorView?.Text); }

    protected override object GetValueForValidator()
    {
        return EditorView.Text;
    }
}
