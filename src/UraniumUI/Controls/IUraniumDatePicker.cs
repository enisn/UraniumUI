namespace UraniumUI.Controls;
public interface IUraniumDatePicker : IView
{
    DateTime? Date { get; set; }
    event EventHandler<DateTime> DateChanged;
}
