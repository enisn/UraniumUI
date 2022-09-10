using UraniumUI.Material.Resources;

namespace UraniumUI.Material.Controls;

public class DataGridSelectionColumn : DataGridColumn, IDataGridSelectionColumn
{
    public event EventHandler<bool> SelectionChanged;
    public DataGridSelectionColumn()
    {
        this.CellItemTemplate = new DataTemplate(() =>
        {
            var checkBox = new  InputKit.Shared.Controls.CheckBox
            {
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center,
                Type = CheckBox.CheckType.Filled,
                Margin = 10
            };

            checkBox.Children.Remove(checkBox.Children.FirstOrDefault(x => x is Label));

            checkBox.SetBinding(CheckBox.IsCheckedProperty, new Binding(nameof(CellBindingContext.IsSelected), BindingMode.TwoWay));

            var contentView = new ContentView
            {
                Content = checkBox
            };

            checkBox.Color = ColorResource.GetColor("Primary", Colors.Red);
            checkBox.SetAppThemeColor(CheckBox.BorderColorProperty,
                ColorResource.GetColor("OnBackground"),
                ColorResource.GetColor("OnBackgroundDark"));
            checkBox.CheckChanged += (s, e) =>
            {
                SelectionChanged?.Invoke(checkBox, checkBox.IsChecked);
            };

            return contentView;
        });
    }
}