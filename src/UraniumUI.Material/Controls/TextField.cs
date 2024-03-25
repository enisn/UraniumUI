using Plainer.Maui.Controls;
using UraniumUI.Material.Extensions;
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
            StyleClass = new[] { "TextField.ClearIcon" },
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

        AfterConstructor();
    }

    partial void AfterConstructor();

    protected override void OnHandlerChanged()
    {
        base.OnHandlerChanged();

        if (Handler is null)
        {
            EntryView.TextChanged -= EntryView_TextChanged;
            EntryView.Completed -= EntryView_Completed;
            iconClear.Focused -= IconClear_Focused;
        }
        else
        {
            EntryView.TextChanged += EntryView_TextChanged;
            EntryView.Completed += EntryView_Completed;
            iconClear.Focused += IconClear_Focused;

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

        if (AllowClear)
        {
            iconClear.IsVisible = !string.IsNullOrEmpty(e.NewTextValue);
        }

        TextChanged?.Invoke(this, e);
    }

    private void EntryView_Completed(object sender, EventArgs e)
    {
        Completed?.Invoke(this, e);
    }

    private void IconClear_Focused(object sender, FocusEventArgs e)
    {
        ValidateClearButtonFocus();
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

    #region DisallowClearButtonFocus Logic

    protected void ValidateClearButtonFocus()
    {
        if (DisallowClearButtonFocus)
        {
            var controlToFocus = GetNextExternalFocusableControl();

            if (controlToFocus != null)
            {
                //Attempt to focus, I guess just ignore failures for now
                //Maybe loop to next until we find ourselves?
                controlToFocus.Focus();
            }
        }
    }

    protected IView GetNextExternalFocusableControl()
    {
        return UraniumUI.Extensions.ViewExtensions.GetNextElement(this.Parent, this) as IView;
    }

    #endregion DisallowClearButtonFocus Logic
}
