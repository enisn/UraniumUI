namespace UraniumUI.Material.Controls;

public interface IAutoCompleteView : IView
{
    string Text { get; set; }
    string SelectedText { get; set; }
    Color TextColor { get; set; }
    IList<string> ItemsSource { get; set; }

    void Completed();
}