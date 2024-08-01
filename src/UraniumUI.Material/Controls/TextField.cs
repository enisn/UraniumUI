using Plainer.Maui.Controls;
using System.Windows.Input;
using UraniumUI.Converters;
using UraniumUI.Material.Extensions;
using UraniumUI.Pages;
using UraniumUI.Resources;
using UraniumUI.ViewExtensions;
using UraniumUI.Views;
using Path = Microsoft.Maui.Controls.Shapes.Path;

namespace UraniumUI.Material.Controls;

[ContentProperty(nameof(Validations))]
public partial class TextField : InputField
{
    public EntryView EntryView => this.FindByViewQueryIdInVisualTreeDescendants<EntryView>("EntryView");

    public override View Content { get; set; } = new EntryView
    {
        Margin = new Thickness(10, 0),
        BackgroundColor = Colors.Transparent,
        VerticalOptions = LayoutOptions.Center,
    };

    public override bool HasValue { get => !string.IsNullOrEmpty(Text); }

    public IList<Behavior> EntryBehaviors => EntryView?.Behaviors;

    public event EventHandler<TextChangedEventArgs> TextChanged;
    public event EventHandler Completed;

    public ICommand ClearCommand { get; protected set; }

    public TextField()
    {
        var entryView = Content as EntryView;

        entryView.SetId("EntryView");

        UpdateClearIconState();

        entryView.SetBinding(Entry.TextProperty, new Binding(nameof(Text), BindingMode.TwoWay, source: this));
        entryView.SetBinding(Entry.TextColorProperty, new Binding(nameof(TextColor), BindingMode.OneWay, source: this));
        entryView.SetBinding(Entry.ReturnCommandParameterProperty, new Binding(nameof(ReturnCommandParameter), BindingMode.TwoWay, source: this));
        entryView.SetBinding(Entry.ReturnCommandProperty, new Binding(nameof(ReturnCommand), BindingMode.TwoWay, source: this));
        entryView.SetBinding(Entry.SelectionLengthProperty, new Binding(nameof(SelectionLength), BindingMode.TwoWay, source: this));
        entryView.SetBinding(Entry.CursorPositionProperty, new Binding(nameof(CursorPosition), BindingMode.TwoWay, source: this));
        entryView.SetBinding(Entry.IsEnabledProperty, new Binding(nameof(IsEnabled), BindingMode.OneWay, source: this));
        entryView.SetBinding(Entry.IsReadOnlyProperty, new Binding(nameof(IsReadOnly), BindingMode.OneWay, source: this));

        AfterConstructor();
    }

    partial void AfterConstructor();

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
    }

    protected override void OnHandlerChanged()
    {
        base.OnHandlerChanged();

        if (Handler is null)
        {
            EntryView.TextChanged -= EntryView_TextChanged;
            EntryView.Completed -= EntryView_Completed;
        }
        else
        {
            EntryView.TextChanged += EntryView_TextChanged;
            EntryView.Completed += EntryView_Completed;

            ApplyAttachedProperties();
        }
    }

    protected virtual void ApplyAttachedProperties()
    {
        EntryProperties.SetSelectionHighlightColor(EntryView, SelectionHighlightColor);
    }

    private void EntryView_TextChanged(object sender, TextChangedEventArgs e)
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
        var existing = endIconsContainer.FindByViewQueryIdInVisualTreeDescendants<StatefulContentView>("ClearIcon");
        if (AllowClear)
        {
            if (existing == null)
            {
                var iconClear = CreateIconClear();
                endIconsContainer.Add(iconClear);
            }
        }
        else
        {
            endIconsContainer?.Remove(existing);
        }
    }

    public override void ResetValidation()
    {
        EntryView.Text = string.Empty;
        base.ResetValidation();
    }

    protected virtual View CreateIconClear()
    {
        var contentView = new StatefulContentView
        {
            VerticalOptions = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.End,
            IsVisible = false,
            Padding = new Thickness(5, 0),
            Margin = new Thickness(0, 0, 5, 0),
            TappedCommand = new Command(OnClearTapped),
            Content = new Path
            {
                StyleClass = new[] { "TextField.ClearIcon" },
                Data = UraniumShapes.X,
                Fill = ColorResource.GetColor("OnBackground", "OnBackgroundDark", Colors.DarkGray).WithAlpha(.5f),
            }
        };
        contentView.SetId("ClearIcon");
        contentView.SetBinding(StatefulContentView.IsFocusableProperty, new Binding(nameof(DisallowClearButtonFocus), source: this));
        contentView.SetBinding(StatefulContentView.IsVisibleProperty, new Binding(nameof(Text), converter: UraniumConverters.StringIsNotNullOrEmptyConverter, source: this));
        return contentView;
    }
}
