using System;
using System.Windows.Input;

namespace UraniumUI.Controls;
public class AutoCompleteView : View, IAutoCompleteView
{
    public AutoCompleteView()
    {
        ItemsSource = new List<string>();
    }

    public event EventHandler<TextChangedEventArgs> TextChanged;

    public event EventHandler Completed;

    void IAutoCompleteView.Completed()
    {
        Completed?.Invoke(this, EventArgs.Empty);

        if (ReturnCommand?.CanExecute(ReturnCommandParameter) ?? false)
        {
            ReturnCommand?.Execute(ReturnCommandParameter);
        }
    }

    public string Text { get => (string)GetValue(TextProperty); set => SetValue(TextProperty, value); }

    public static readonly BindableProperty TextProperty = BindableProperty.Create(
        nameof(Text),
        typeof(string),
        typeof(AutoCompleteView),
        string.Empty,
        BindingMode.TwoWay,
        propertyChanged: (bindable, oldValue, newValue) =>
        {
            if (bindable is AutoCompleteView view)
            {
                view.TextChanged?.Invoke(view, new TextChangedEventArgs((string)oldValue, (string)newValue));
            }
        });

    public string SelectedText { get => (string)GetValue(SelectedTextProperty); set => SetValue(SelectedTextProperty, value); }

    public static readonly BindableProperty SelectedTextProperty = BindableProperty.Create(
        nameof(SelectedText),
        typeof(string),
        typeof(AutoCompleteView),
        string.Empty,
        BindingMode.TwoWay);

    public Color TextColor { get => (Color)GetValue(TextColorProperty); set => SetValue(TextColorProperty, value); }

    public static readonly BindableProperty TextColorProperty = BindableProperty.Create(
        nameof(TextColor),
        typeof(Color),
        typeof(AutoCompleteView),
        Colors.DarkGray);

    public IList<string> ItemsSource { get => (IList<string>)GetValue(ItemsSourceProperty); set => SetValue(ItemsSourceProperty, value); }

    public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(nameof(ItemsSource),
            typeof(IList<string>),
            typeof(AutoCompleteView),
            null);

    public int Threshold { get => (int)GetValue(ThresholdProperty); set => SetValue(ThresholdProperty, value); }

    public static readonly BindableProperty ThresholdProperty = BindableProperty.Create(nameof(Threshold),
        typeof(int),
        typeof(AutoCompleteView),
        2);

    public ICommand ReturnCommand { get => (ICommand)GetValue(ReturnCommandProperty); set => SetValue(ReturnCommandProperty, value); }

    public static readonly BindableProperty ReturnCommandProperty = BindableProperty.Create(nameof(ReturnCommand),
            typeof(ICommand),
            typeof(AutoCompleteView),
            null);

    public object ReturnCommandParameter { get => GetValue(ReturnCommandParameterProperty); set => SetValue(ReturnCommandParameterProperty, value); }

    public static readonly BindableProperty ReturnCommandParameterProperty = BindableProperty.Create(nameof(ReturnCommandParameter),
            typeof(object),
            typeof(AutoCompleteView),
            null);
}
