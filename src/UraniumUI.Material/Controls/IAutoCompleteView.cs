namespace UraniumUI.Material.Controls;

public interface IAutoCompleteView : IView
{
    string Text { get; set; }
    Color TextColor { get; set; }
    IList<string> ItemsSource { get; set; }
    void InvokeTextChanged(TextChangedEventArgs args);
}