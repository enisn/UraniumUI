using Plainer.Maui.Controls;

namespace UraniumUI.Material.Controls;
public partial class TextField : InputField
{
    protected EntryView mainEntry => Content as EntryView;

    
    public override View Content { get; set; } = new EntryView
    {
        Margin = new Thickness(5,1),
        BackgroundColor = Colors.Transparent,
    };

    public override bool HasValue { get => !string.IsNullOrEmpty(Text); }

    public TextField()
    {
        mainEntry.SetBinding(Entry.TextProperty, new Binding(nameof(Text), source: this));
    }
}
