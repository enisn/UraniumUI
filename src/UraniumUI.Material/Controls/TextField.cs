using Plainer.Maui.Controls;

namespace UraniumUI.Material.Controls;
public partial class TextField : InputField
{
    public EntryView EntryView => Content as EntryView;

    public override View Content { get; set; } = new EntryView
    {
        Margin = new Thickness(5,1),
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
    }
}
