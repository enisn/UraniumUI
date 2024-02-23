using InputKit.Shared.Controls;
using System.Reflection;
using UraniumUI.Controls;
using UraniumUI.Extensions;
using UraniumUI.Material.Controls;
using UraniumUI.Options;
using UraniumUI.Resources;

namespace UraniumUI.Material.Extensions;

public static class AutoFormViewMaterialConfigurationExtension
{
    public static MauiAppBuilder ConfigureAutoFormViewForMaterial(this MauiAppBuilder builder)
    {
        builder.Services.Configure<AutoFormViewOptions>(options =>
        {
            options.EditorMapping[typeof(string)] = EditorForString;
            options.EditorMapping[typeof(int)] = EditorForNumeric;
            options.EditorMapping[typeof(double)] = EditorForNumeric;
            options.EditorMapping[typeof(float)] = EditorForNumeric;
            options.EditorMapping[typeof(bool)] = EditorForBoolean;
            options.EditorMapping[typeof(Keyboard)] = EditorForKeyboard;
            options.EditorMapping[typeof(Enum)] = EditorForEnum;
            options.EditorMapping[typeof(DateTime)] = EditorForDateTime;
            options.EditorMapping[typeof(TimeSpan)] = EditorForTimeSpan;
        });

        return builder;
    }

    public static View EditorForString(PropertyInfo property, object source)
    {
        var editor = new TextField();
        editor.SetBinding(TextField.TextProperty, new Binding(property.Name, source: source));
        editor.AllowClear = true;
        editor.Title = property.Name;

        return editor;
    }

    public static View EditorForNumeric(PropertyInfo property, object source)
    {
        var editor = new TextField();
        editor.SetBinding(TextField.TextProperty, new Binding(property.Name, source: source));
        editor.Title = property.Name;
        editor.AllowClear = false;
        editor.Keyboard = Keyboard.Numeric;

        return editor;
    }

    public static View EditorForBoolean(PropertyInfo property, object source)
    {
        var editor = new UraniumUI.Material.Controls.CheckBox();
        editor.SetBinding(UraniumUI.Material.Controls.CheckBox.IsCheckedProperty, new Binding(property.Name, source: source));
        editor.Text = property.Name;

        return editor;
    }

    public static View EditorForEnum(PropertyInfo property, object source)
    {
        var editor = new PickerField();

        var values = Enum.GetValues(property.PropertyType.AsNonNullable());
        if (values.Length <= 5)
        {
            return CreateSelectionViewForValues(values, property, source);
        }

        editor.ItemsSource = values;
        editor.SetBinding(PickerField.SelectedItemProperty, new Binding(property.Name, source: source));
        editor.Title = property.Name;
        editor.AllowClear = false;
        return editor;
    }

    private static View CreateSelectionViewForValues(Array values, PropertyInfo property, object source)
    {
        var shouldUseSingleColumn = values.Length > 3;
        var editor = new SelectionView
        {
            Color = ColorResource.GetColor("Primary", "PrimaryDark"),
            ColumnSpacing = -2,
            RowSpacing = shouldUseSingleColumn ? 5 : -2,
            SelectionType = shouldUseSingleColumn ? InputKit.Shared.SelectionType.RadioButton : InputKit.Shared.SelectionType.Button,
            ColumnNumber = shouldUseSingleColumn ? 1 : values.Length,
            ItemsSource = values
        };

        editor.SetBinding(SelectionView.SelectedItemProperty, new Binding(property.Name, source: source));

        return new VerticalStackLayout
        {
            Spacing = 6,
            Children = {
                new Label { Text = property.Name },
                editor
            }
        };
    }

    public static View EditorForKeyboard(PropertyInfo property, object source)
    {
        var editor = new PickerField();

        editor.ItemsSource = typeof(Keyboard)
            .GetProperties(BindingFlags.Public | BindingFlags.Static)
            .Select(x => x.GetValue(null))
            .ToArray();

        editor.SetBinding(PickerField.SelectedItemProperty, new Binding(property.Name, source: source));
        editor.Title = property.Name;
        editor.AllowClear = false;
        return editor;
    }

    public static View EditorForDateTime(PropertyInfo property, object source)
    {
        var editor = new DatePickerField();
        editor.SetBinding(DatePickerField.DateProperty, new Binding(property.Name, source: source));
        editor.Title = property.Name;
        editor.AllowClear = false;
        return editor;
    }

    public static View EditorForTimeSpan(PropertyInfo property, object source)
    {
        var editor = new TimePickerField();
        editor.SetBinding(TimePickerField.TimeProperty, new Binding(property.Name, source: source));
        editor.Title = property.Name;
        editor.AllowClear = false;
        return editor;
    }
}
