using System.Collections;

namespace UraniumUI.Controls;

public interface IAutoCompleteView : IView
{
    string Text { get; set; }
    string SelectedText { get; set; }
    Color TextColor { get; set; }
    IList ItemsSource { get; set; }
    int Threshold { get; set; }

    void Completed();
}