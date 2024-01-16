#if WINDOWS
using Microsoft.Maui.Platform;
#endif
using Plainer.Maui.Controls;
using UraniumUI.Pages;
using UraniumUI.Resources;
using UraniumUI.Views;
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

    protected StatefulContentView iconClear = new StatefulContentView
    {
        VerticalOptions = LayoutOptions.Center,
        HorizontalOptions = LayoutOptions.End,
        IsVisible = false,
        Padding = new Thickness(5, 0),
        Margin = new Thickness(0, 0, 5, 0),
        Content = new Path
        {
            Data = UraniumShapes.X,
            Fill = ColorResource.GetColor("OnBackground", "OnBackgroundDark", Colors.DarkGray).WithAlpha(.5f),
        }
    };

    public override bool HasValue { get => !string.IsNullOrEmpty(Text); }

    public IList<Behavior> EntryBehaviors => EntryView?.Behaviors;

    public event EventHandler<TextChangedEventArgs> TextChanged;
    public event EventHandler Completed;

    public TextField()
    {
        iconClear.TappedCommand = new Command(OnClearTapped);
        
        UpdateClearIconState();
        EntryView.SetBinding(Entry.TextProperty, new Binding(nameof(Text), source: this));
        EntryView.SetBinding(Entry.ReturnCommandParameterProperty, new Binding(nameof(ReturnCommandParameter), source: this));
        EntryView.SetBinding(Entry.ReturnCommandProperty, new Binding(nameof(ReturnCommand), source: this));
        EntryView.SetBinding(Entry.SelectionLengthProperty, new Binding(nameof(SelectionLength), source: this));
        EntryView.SetBinding(Entry.CursorPositionProperty, new Binding(nameof(CursorPosition), source: this));
        EntryView.SetBinding(Entry.IsEnabledProperty, new Binding(nameof(IsEnabled), source: this));
        EntryView.SetBinding(Entry.IsReadOnlyProperty, new Binding(nameof(IsReadOnly), source: this));
    }

    protected override void OnHandlerChanged()
    {
        base.OnHandlerChanged();
#if WINDOWS
        if (EntryView.Handler.PlatformView is Microsoft.UI.Xaml.Controls.TextBox textBox)
        {
            textBox.SelectionHighlightColor = new Microsoft.UI.Xaml.Media.SolidColorBrush(ColorResource.GetColor("Primary", "PrimaryDark", Colors.Purple).ToWindowsColor());
            textBox.BorderThickness = new Microsoft.UI.Xaml.Thickness(0);

            textBox.Style = null;
        }
#endif

        if (Handler is null)
        {
            EntryView.TextChanged -= EntryView_TextChanged;
            EntryView.Completed -= EntryView_Completed;
        }
        else
        {
            EntryView.TextChanged += EntryView_TextChanged;
            EntryView.Completed += EntryView_Completed;
        }
    }

    private void EntryView_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (string.IsNullOrEmpty(e.OldTextValue) || string.IsNullOrEmpty(e.NewTextValue))
        {
            UpdateState();
        }

#if !WINDOWS // Workaround for https://github.com/enisn/UraniumUI/issues/373
        if (e.NewTextValue != null)
        {
            CheckAndShowValidations();
        }
#endif

        if (AllowClear)
        {
            iconClear.IsVisible = !string.IsNullOrEmpty(e.NewTextValue);
        }

        TextChanged?.Invoke(this, e);
    }

    protected override void OnFocusChanged(object sender, FocusEventArgs args)
    {
        base.OnFocusChanged(sender, args);

#if WINDOWS // Workaround for https://github.com/enisn/UraniumUI/issues/373
        if (!args.IsFocused)
        {
            CheckAndShowValidations();
        }
#endif
    }

    private void EntryView_Completed(object sender, EventArgs e)
    {
        Completed?.Invoke(this, e);
    }

    public void ClearValue()
    {
        if (IsEnabled)
        {
            Text = string.Empty;
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

    public override void ResetValidation()
    {
        EntryView.Text = string.Empty;
        base.ResetValidation();
    }
}
