using Plainer.Maui.Controls;

namespace UraniumUI.Material.Controls;

[ContentProperty(nameof(Validations))]
public partial class TextField : InputField
{
    public EntryView EntryView => Content as EntryView;

    public override View Content { get; set; } = new EntryView
    {
        Margin = new Thickness(5, 1),
        BackgroundColor = Colors.Transparent,
    };

    public override bool HasValue { get => !string.IsNullOrEmpty(Text); }

    public TextField()
    {
        EntryView.SetBinding(Entry.TextProperty, new Binding(nameof(Text), source: this));
        EntryView.SetBinding(Entry.ReturnCommandParameterProperty, new Binding(nameof(ReturnCommand), source: this));
        EntryView.SetBinding(Entry.ReturnCommandProperty, new Binding(nameof(ReturnCommand), source: this));
        EntryView.SetBinding(Entry.SelectionLengthProperty, new Binding(nameof(SelectionLength), source: this));
        EntryView.SetBinding(Entry.CursorPositionProperty, new Binding(nameof(CursorPosition), source: this));

        EntryView.TextChanged += EntryView_TextChanged;

#if WINDOWS
        EntryView.HandlerChanged += (s, e) =>
        {
            var textBox = EntryView.Handler.PlatformView as Microsoft.UI.Xaml.Controls.TextBox;
            Console.WriteLine(EntryView.Handler.PlatformView as Microsoft.UI.Xaml.Controls.TextBox);

            textBox.FocusVisualPrimaryThickness = new Microsoft.UI.Xaml.Thickness(0);
            textBox.FocusVisualSecondaryThickness = new Microsoft.UI.Xaml.Thickness(0);
            textBox.SelectionHighlightColor = new Microsoft.UI.Xaml.Media.SolidColorBrush(Windows.UI.Color.FromArgb(0,0,0,0));
            textBox.BorderThickness = new Microsoft.UI.Xaml.Thickness(0);
        };
#endif
    }

    private void EntryView_TextChanged(object sender, TextChangedEventArgs e)
    {
        CheckAndShowValidations();
    }

    protected override object GetValueForValidator()
    {
        return EntryView.Text;
    }
}
