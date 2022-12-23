using Plainer.Maui.Controls;
using UraniumUI.Pages;
using UraniumUI.Resources;
using Path = Microsoft.Maui.Controls.Shapes.Path;

namespace UraniumUI.Material.Controls;

[ContentProperty(nameof(Validations))]
public partial class TextField : InputField
{
    public EntryView EntryView => Content as EntryView;

    public override View Content { get; set; } = new EntryView
    {
        Margin = new Thickness(10, 0),
        BackgroundColor = Colors.Transparent,
        VerticalOptions = LayoutOptions.Center
    };

    protected ContentView iconClear = new ContentView
    {
        VerticalOptions = LayoutOptions.Center,
        HorizontalOptions = LayoutOptions.End,
        IsVisible = false,
        Padding = 10,
        Content = new Path
        {
            Data = UraniumShapes.X,
            Fill = ColorResource.GetColor("OnBackground", "OnBackgroundDark", Colors.DarkGray).WithAlpha(.5f),
        }
    };

    public override bool HasValue { get => !string.IsNullOrEmpty(Text); }

    public TextField()
    {
        iconClear.GestureRecognizers.Add(new TapGestureRecognizer
        {
            Command = new Command(Clear)
        });

        UpdateClearIconState();

        EntryView.SetBinding(Entry.TextProperty, new Binding(nameof(Text), source: this));
        EntryView.SetBinding(Entry.ReturnCommandParameterProperty, new Binding(nameof(ReturnCommandParameter), source: this));
        EntryView.SetBinding(Entry.ReturnCommandProperty, new Binding(nameof(ReturnCommand), source: this));
        EntryView.SetBinding(Entry.SelectionLengthProperty, new Binding(nameof(SelectionLength), source: this));
        EntryView.SetBinding(Entry.CursorPositionProperty, new Binding(nameof(CursorPosition), source: this));
        EntryView.SetBinding(Entry.IsEnabledProperty, new Binding(nameof(IsEnabled), source: this));
        EntryView.SetBinding(Entry.IsReadOnlyProperty, new Binding(nameof(IsReadOnly), source: this));
        EntryView.TextChanged += EntryView_TextChanged;

#if WINDOWS
		EntryView.HandlerChanged += (s, e) =>
		{
			var textBox = EntryView.Handler.PlatformView as Microsoft.UI.Xaml.Controls.TextBox;

			textBox.FocusVisualPrimaryThickness = new Microsoft.UI.Xaml.Thickness(0);
			textBox.FocusVisualSecondaryThickness = new Microsoft.UI.Xaml.Thickness(0);
			textBox.SelectionHighlightColor = new Microsoft.UI.Xaml.Media.SolidColorBrush(Windows.UI.Color.FromArgb(0, 0, 0, 0));
			textBox.BorderThickness = new Microsoft.UI.Xaml.Thickness(0);
		};
#endif
    }

    ~TextField()
    {
        EntryView.TextChanged -= EntryView_TextChanged;
    }

    public void ClearValue()
    {
        if (IsEnabled)
        {
            Text = string.Empty;
        }
    }

    private void EntryView_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (string.IsNullOrEmpty(e.OldTextValue) || string.IsNullOrEmpty(e.NewTextValue))
        {
            UpdateState();
        }

        if (!string.IsNullOrEmpty(e.NewTextValue))
        {
            CheckAndShowValidations();
        }

        if (AllowClear)
        {
            iconClear.IsVisible = !string.IsNullOrEmpty(e.NewTextValue);
        }
    }

    protected override object GetValueForValidator()
    {
        return EntryView.Text;
    }

    protected virtual void OnClearTapped()
    {
        EntryView.Text = string.Empty;
    }

    protected virtual void OnAllowClearChanged()
    {
        UpdateClearIconState();
    }

    protected virtual void UpdateClearIconState()
    {
        if (AllowClear)
        {
            if (!endIconsContainer.Contains(iconClear))
            {
                endIconsContainer.Add(iconClear);
            }
        }
        else
        {
            endIconsContainer.Remove(iconClear);
        }
    }
}
