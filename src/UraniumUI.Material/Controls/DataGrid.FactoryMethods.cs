namespace UraniumUI.Material.Controls;
public partial class DataGrid
{
    public Func<string, Label> LabelFactory { get; set; }

    public Func<View> HorizontalLineFactory { get; set; }

    private void InitializeFactoryMethods()
    {
        LabelFactory = CreateLabel;
        HorizontalLineFactory = CreateHorizontalLine;
    }

    protected virtual Label CreateLabel(string propertyName)
    {
        var label = new Label
        {
            Margin = 20,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center
        };

        label.SetBinding(Label.TextProperty, propertyName);

        return label;
    }

    protected virtual View CreateHorizontalLine()
    {
        var boxView = new BoxView
        {
            HorizontalOptions = LayoutOptions.Fill,
            HeightRequest = 2,
            CornerRadius = 1,
            Opacity = .4
        };

        boxView.SetBinding(BoxView.ColorProperty, new Binding(nameof(LineSeperatorColor), source: this));

        return boxView;
    }
}