using Microsoft.Maui.Platform;
using Plainer.Maui.Controls;
using UraniumUI.Resources;

namespace UraniumUI.Material.Controls;
public partial class EditorField : InputField
{
    public EditorView EditorView => Content as EditorView;
    public event EventHandler<TextChangedEventArgs> TextChanged;
    public event EventHandler Completed;

    public override View Content { get; set; } = new EditorView
    {
        Margin = new Thickness(10, 0),
        BackgroundColor = Colors.Transparent,
        VerticalOptions = LayoutOptions.Center,
        AutoSize = EditorAutoSizeOption.TextChanges,
    };

    public EditorField()
    {
        base.RegisterForEvents();
        EditorView.SetBinding(Editor.TextProperty, new Binding(nameof(Text), source: this));
        EditorView.SetBinding(Editor.SelectionLengthProperty, new Binding(nameof(SelectionLength), source: this));
        EditorView.SetBinding(Editor.CursorPositionProperty, new Binding(nameof(CursorPosition), source: this));
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
        if (Handler is null)
        {
            EditorView.TextChanged -= EditorView_TextChanged;
            EditorView.Completed -= EditorView_Completed;
        }
        else
        {
            EditorView.TextChanged += EditorView_TextChanged;
            EditorView.Completed += EditorView_Completed;
        }
    }

    private void EditorView_Completed(object sender, EventArgs e)
    {
        // adding implementaion, but does not work due to bug #5730:
        // https://github.com/dotnet/maui/issues/5730
        Completed?.Invoke(this, e);
    }

    private void EditorView_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (string.IsNullOrEmpty(e.OldTextValue) || string.IsNullOrEmpty(e.NewTextValue))
        {
            UpdateState();
        }

        if (e.NewTextValue != null)
        {
            CheckAndShowValidations();
        }

        TextChanged?.Invoke(this, e);
    }

    public override bool HasValue { get => !string.IsNullOrEmpty(EditorView?.Text); }

    protected override object GetValueForValidator()
    {
        return EditorView.Text;
    }

    public override void ResetValidation()
    {
        EditorView.Text = string.Empty;
        base.ResetValidation();
    }
}
