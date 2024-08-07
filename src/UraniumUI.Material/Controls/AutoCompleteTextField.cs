using Plainer.Maui.Controls;
using UraniumUI.Controls;
using UraniumUI.Pages;
using UraniumUI.Resources;
using UraniumUI.ViewExtensions;
using Path = Microsoft.Maui.Controls.Shapes.Path;

namespace UraniumUI.Material.Controls;
public class AutoCompleteTextField : InputField
{
    private bool _clearTapped;

    public AutoCompleteView AutoCompleteView => this.FindByViewQueryIdInVisualTreeDescendants<AutoCompleteView>("AutoCompleteView");

    public override View Content { get; set; } = new AutoCompleteView
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

    public AutoCompleteTextField()
    {
        base.RegisterForEvents();

        ItemsSource = new List<string>();

        var autoCompleteView = Content as AutoCompleteView;

        autoCompleteView.SetId("AutoCompleteView");

        Content = autoCompleteView;

        iconClear.GestureRecognizers.Add(new TapGestureRecognizer
        {
            Command = new Command(OnClearTapped)
        });

        autoCompleteView.SetBinding(AutoCompleteView.TextProperty, new Binding(nameof(Text), source: this));
        autoCompleteView.SetBinding(AutoCompleteView.SelectedTextProperty, new Binding(nameof(SelectedText), source: this));
        autoCompleteView.SetBinding(AutoCompleteView.ItemsSourceProperty, new Binding(nameof(ItemsSource), source: this));
        autoCompleteView.SetBinding(AutoCompleteView.TextColorProperty, new Binding(nameof(TextColor), source: this));
        autoCompleteView.SetBinding(AutoCompleteView.ThresholdProperty, new Binding(nameof(Threshold), source: this));
    }

    public override bool HasValue => !string.IsNullOrEmpty(Text);

    protected override object GetValueForValidator()
    {
        return AutoCompleteView.Text;
    }

    protected override void OnHandlerChanged()
    {
        base.OnHandlerChanged();

        AutoCompleteView.TextChanged += AutoCompleteView_TextChanged;

        if (Handler is null)
        {
            AutoCompleteView.TextChanged -= AutoCompleteView_TextChanged;
        }
    }

    private void AutoCompleteView_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (string.IsNullOrEmpty(e.OldTextValue) || string.IsNullOrEmpty(e.NewTextValue))
        {
            UpdateState();
        }

        if (!string.IsNullOrEmpty(e.NewTextValue))
        {
            CheckAndShowValidations();
        }
        else
        {
            if (!_clearTapped)
            {
                SelectedText = string.Empty;
            }
            else
            {
                _clearTapped = false;
            }
        }

        if (AllowClear)
        {
            iconClear.IsVisible = !string.IsNullOrEmpty(e.NewTextValue);
        }

        TextChanged?.Invoke(this, e);
    }

    public event EventHandler<TextChangedEventArgs> TextChanged;

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

    protected virtual void OnClearTapped()
    {
        _clearTapped = true;
        AutoCompleteView.Text = string.Empty;
    }

    public string Text { get => (string)GetValue(TextProperty); set => SetValue(TextProperty, value); }

    public static readonly BindableProperty TextProperty = BindableProperty.Create(
        nameof(Text),
        typeof(string),
        typeof(AutoCompleteView),
        string.Empty,
        BindingMode.TwoWay);
   
    public string SelectedText { get => (string)GetValue(SelectedTextProperty); set => SetValue(SelectedTextProperty, value); }

    public static readonly BindableProperty SelectedTextProperty = BindableProperty.Create(
        nameof(SelectedText),
        typeof(string),
        typeof(AutoCompleteView),
        string.Empty,
        BindingMode.TwoWay);

    public IList<string> ItemsSource { get => (IList<string>)GetValue(ItemsSourceProperty); set => SetValue(ItemsSourceProperty, value); }

    public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(nameof(ItemsSource),
            typeof(IList<string>),
            typeof(AutoCompleteView),
            null);

    public Color TextColor { get => (Color)GetValue(TextColorProperty); set => SetValue(TextColorProperty, value); }

    public static readonly BindableProperty TextColorProperty = BindableProperty.Create(
        nameof(TextColor),
        typeof(Color),
        typeof(AutoCompleteView),
        ColorResource.GetColor("OnBackground", "OnBackgroundDark", Colors.DarkGray));

    public bool AllowClear { get => (bool)GetValue(AllowClearProperty); set => SetValue(AllowClearProperty, value); }

    public static BindableProperty AllowClearProperty = BindableProperty.Create(
        nameof(AllowClear),
        typeof(bool), typeof(TextField),
        false,
        propertyChanged: (bindable, oldValue, newValue) => (bindable as AutoCompleteTextField).OnAllowClearChanged());

    public int Threshold { get => (int)GetValue(ThresholdProperty); set => SetValue(ThresholdProperty, value); }

    public static BindableProperty ThresholdProperty = BindableProperty.Create(
        nameof(Threshold),
        typeof(int),
        typeof(AutoCompleteTextField),
        AutoCompleteView.ThresholdProperty.DefaultValue,
        propertyChanged: (bindable, oldValue, newValue) => (bindable as AutoCompleteTextField).AutoCompleteView.Threshold = (int)newValue);
}
