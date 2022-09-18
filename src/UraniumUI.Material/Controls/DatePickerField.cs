using Plainer.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UraniumUI.Resources;
using UraniumUI.Pages;
using Path = Microsoft.Maui.Controls.Shapes.Path;

namespace UraniumUI.Material.Controls;
public class DatePickerField : InputField
{
    public DatePickerView DatePickerView => Content as DatePickerView;
    public override View Content { get; set; } = new DatePickerView
    {
        VerticalOptions = LayoutOptions.Center,
        Margin = new Thickness(10, 0),
        Opacity = 0,
    };

    protected ContentView iconClear = new ContentView
    {
        VerticalOptions = LayoutOptions.Center,
        HorizontalOptions = LayoutOptions.End,
        WidthRequest = 30,
        IsVisible = false,
        Content = new Path
        {
            Data = UraniumShapes.X,
            Fill = ColorResource.GetColor("Surface", "SurfaceDark", Colors.LightGray),
        }
    };

    public override bool HasValue => Date != null; // DateTime cannot be null

    public DatePickerField()
    {
        var clearGestureRecognizer = new TapGestureRecognizer();
        clearGestureRecognizer.Tapped += OnClearTapped;
        iconClear.GestureRecognizers.Add(clearGestureRecognizer);

        endIconsContainer.Add(iconClear);

        DatePickerView.SetBinding(DatePickerView.DateProperty, new Binding(nameof(Date), source: this));
    }

    protected override object GetValueForValidator()
    {
        return DatePickerView.Date;
    }
    protected void OnClearTapped(object sender, EventArgs e)
    {
        Date = null;
    }

    protected virtual void OnDateChanged()
    {
        OnPropertyChanged(nameof(Date));
        CheckAndShowValidations();
        DatePickerView.Opacity = Date == null ? 0 : 1;
        iconClear.IsVisible = Date != null;

        UpdateState();
    }

    public DateTime? Date { get => (DateTime?)GetValue(DateProperty); set => SetValue(DateProperty, value); }

    public static readonly BindableProperty DateProperty = BindableProperty.Create(
        nameof(Date), typeof(DateTime?), typeof(DatePickerField), defaultValue: null, defaultBindingMode: BindingMode.TwoWay,
        propertyChanged: (bindable, oldValue, newValue) => (bindable as DatePickerField).OnDateChanged()
        );
}
