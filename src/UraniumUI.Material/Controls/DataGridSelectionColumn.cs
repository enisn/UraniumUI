using UraniumUI.Material.Resources;
using CheckBox = InputKit.Shared.Controls.CheckBox;

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

            var contentView = new ContentView
            {
                Content = checkBox
            };

            checkBox.Color = ThemeResource.GetColor("Primary", Colors.Red);
            checkBox.SetAppThemeColor(CheckBox.BorderColorProperty, 
                ThemeResource.GetColor("OnBackground"),
                ThemeResource.GetColor("Background"));
            checkBox.CheckChanged += (s, e) =>
            {
                SelectionChanged?.Invoke(checkBox, checkBox.IsChecked);
            };

            return contentView;
        });
    }
}