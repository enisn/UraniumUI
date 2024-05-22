using System.Collections;

namespace UraniumUI.Controls;
public interface IDropdown : IView
{
    object SelectedItem { get; set; }

    IList ItemsSource { get; set; }
}
