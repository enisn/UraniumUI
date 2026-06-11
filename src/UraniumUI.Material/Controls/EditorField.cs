using Microsoft.Maui.Platform;
using Plainer.Maui.Controls;
using UraniumUI.Resources;

namespace UraniumUI.Material.Controls;
public partial class EditorField : InputField
{
    public static readonly BindableProperty EditorHeightRequestProperty =
          BindableProperty.Create(
              nameof(EditorHeightRequest),
              typeof(double),
              typeof(EditorField),
              -1.0,
              propertyChanged: OnEditorHeightRequestChanged);

    public double EditorHeightRequest
    {
        get => (double)GetValue(EditorHeightRequestProperty);
        set
        {
            SetValue(EditorHeightRequestProperty, value);
        }
    }
    private static void OnEditorHeightRequestChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (EditorField)bindable;
        // Проверяем, действительно ли изменилось значение
        double oldHeight = (double)oldValue;
        double newHeight = (double)newValue;

        if (Math.Abs(oldHeight - newHeight) > 0.001)
        {
            control.Dispatcher.Dispatch(() =>
            {
                control.SetScroll();
            });
        }
    }

    private EditorView _editor;
    ScrollView scrollView;
    private bool _isSettingScroll = false;
    public EditorView EditorView => _editor; //Content as EditorView;
                                             // public EditorView EditorView => Content as EditorView;

    public void SetScroll()
    {
        // Защита от рекурсивных вызовов
        if (_isSettingScroll || _editor == null || scrollView == null || Handler == null) return;
        try
        {
            _isSettingScroll = true;

            if (_editor != null)
            {
                if (EditorHeightRequest > 10)
                {
                    _editor.AutoSize = EditorAutoSizeOption.Disabled;
                    //this.HeightRequest = EditorHeightRequest; // Фиксированная высота с прокруткой
                    _editor.HeightRequest = EditorHeightRequest - 10;
                    scrollView.HeightRequest = EditorHeightRequest - 10;
                }
                else
                {
                    _editor.AutoSize = EditorAutoSizeOption.TextChanges;
                    _editor.HeightRequest = -1;
                    scrollView.HeightRequest = -1;

                }
                InvalidateMeasure();
            }
        }
        finally
        {
            _isSettingScroll = false;
        }
    }







   // public EditorView EditorView => Content as EditorView;
    public event EventHandler<TextChangedEventArgs> TextChanged;
    public event EventHandler Completed;

    public override View Content { get; set; } /*= new EditorView
    {
        Margin = new Thickness(10, 0),
        BackgroundColor = Colors.Transparent,
        VerticalOptions = LayoutOptions.Center,
        AutoSize = EditorAutoSizeOption.TextChanges,
    }; */

    public EditorField()
    {
        // Сначала создаем редактор
        _editor = new EditorView
        {
            Margin = new Thickness(10, 0),
            BackgroundColor = Colors.Transparent,
            VerticalOptions = LayoutOptions.Center,
            AutoSize = EditorAutoSizeOption.TextChanges,
        };

        // Создаем ScrollView и добавляем в него редактор
        scrollView = new ScrollView
        {
            Content = _editor
        };

        // Устанавливаем Content
        Content = scrollView;


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


