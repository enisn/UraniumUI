using InputKit.Shared.Controls;
using System.Reflection;
using UraniumUI.Extensions;
using UraniumUI.Resources;
using UraniumUI.Options;
using Microsoft.Extensions.Options;
using InputKit.Shared.Abstraction;

namespace UraniumUI.Controls;
public class AutoFormView : FormView
{
    public object Source { get => GetValue(SourceProperty); set => SetValue(SourceProperty, value); }
    public static readonly BindableProperty SourceProperty = BindableProperty.Create(
        nameof(Source),
        typeof(object),
        typeof(AutoFormView),
        propertyChanged: (bindable, oldValue, newValue) => (bindable as AutoFormView).OnSourceChanged());

    public PropertyInfo[] EditingProperties { get; protected set; }

    private ContentView _footerView = new ContentView();
    public View FooterView
    {
        get => _footerView.Content; set
        {
            _footerView.Content = value;
            if (_itemsLayout.Children.Count > 0 && !_itemsLayout.Contains(_footerView))
            {
                _itemsLayout.Children.Add(_footerView);
            }
        }
    }

    public bool ShowMissingProperties { get; set; } = true;

    public bool Hierarchical { get; set; } = false;

    public Type HierarchyLimitType { get; set; } = typeof(object);

    private Layout _itemsLayout = new VerticalStackLayout
    {
        Spacing = 20,
        Padding = 10,
    };

    public Layout ItemsLayout
    {
        get => _itemsLayout;
        set
        {
            Children.Remove(_itemsLayout);
            while (_itemsLayout.Children.Count > 0)
            {
                var item = _itemsLayout.Children.First();
                _itemsLayout.Children.Remove(item);
                value?.Children.Add(item);
            }
            _itemsLayout = value;
            if (_itemsLayout != null)
            {
                Children.Add(_itemsLayout);
            }
        }
    }

    protected Dictionary<Type, AutoFormViewOptions.EditorForType> EditorMapping { get; }

    protected AutoFormViewOptions Options { get; }
    public AutoFormView()
    {
        Children.Add(_itemsLayout);

        Options = UraniumServiceProvider.Current.GetRequiredService<IOptions<AutoFormViewOptions>>().Value;
        EditorMapping = Options.EditorMapping;
    }

    protected void OnSourceChanged()
    {
        var flags = BindingFlags.Public | BindingFlags.Instance;
        if (Hierarchical)
        {
            flags |= BindingFlags.FlattenHierarchy;
        }
        var props = Source.GetType()
            .GetProperties(flags)
            .Where(x => HierarchyLimitType.IsAssignableFrom(x.PropertyType))
            .ToArray();

        EditingProperties = props;

        Render();
    }

    private void Render()
    {
        if (Source is null)
        {
            _itemsLayout.Children.Clear();
            return;
        }

        foreach (var property in EditingProperties)
        {
            var createEditor = EditorMapping.FirstOrDefault(x => x.Key.IsAssignableFrom(property.PropertyType.AsNonNullable())).Value;
            if (createEditor != null)
            {
                var editor = createEditor(property, Source);

                foreach (var action in Options.PostEditorActions)
                {
                    action(editor, property);
                }

                if (editor is IValidatable validatable && Options.ValidationFactory != null)
                {
                    validatable.Validations.AddRange(Options.ValidationFactory(property));
                }

                _itemsLayout.Children.Add(editor);
            }
            else if (ShowMissingProperties)
            {
                _itemsLayout.Children.Add(new Label
                {
                    Text = $"No editor for {property.Name} ({property.PropertyType})",
                    FontAttributes = FontAttributes.Italic
                });
            }
        }

        if (!_itemsLayout.Children.Contains(_footerView))
        {
            _itemsLayout.Children.Add(_footerView);
        }

        if (_footerView.Content == null)
        {
            _footerView.Content = GetFooter();
        }
    }

    protected virtual View GetFooter()
    {
        var submitButton = new Button
        {
            Text = "Submit",
            StyleClass = new List<string> { "FilledButton" },
        };

        var resetButton = new Button
        {
            Text = "Reset",
            StyleClass = new List<string> { "OutlinedButton" },
        };

        FormView.SetIsSubmitButton(submitButton, true);
        FormView.SetIsResetButton(resetButton, true);
        return new StackLayout
        {
            Margin = new Thickness(10),
            Spacing = 12,
            Children = { submitButton, resetButton }
        };
    }

    public static View EditorForString(PropertyInfo property, object source)
    {
        var editor = new Entry();
        editor.SetBinding(Entry.TextProperty, new Binding(property.Name, source: source));

        return new VerticalStackLayout
        {
            new Label { Text = property.Name },
            editor
        };
    }

    public static View EditorForNumeric(PropertyInfo property, object source)
    {
        var editor = new Entry();
        editor.SetBinding(Entry.TextProperty, new Binding(property.Name, source: source));
        editor.Keyboard = Keyboard.Numeric;

        return new VerticalStackLayout
        {
            new Label { Text = property.Name },
            editor
        };
    }

    public static View EditorForBoolean(PropertyInfo property, object source)
    {
        var editor = new InputKit.Shared.Controls.CheckBox();
        editor.SetBinding(InputKit.Shared.Controls.CheckBox.IsCheckedProperty, new Binding(property.Name, source: source));
        editor.Text = property.Name;

        return editor;
    }

    public static View EditorForEnum(PropertyInfo property, object source)
    {
        var editor = new Picker();

        var values = Enum.GetValues(property.PropertyType.AsNonNullable());
        if (values.Length <= 5)
        {
            return CreateSelectionViewForValues(values, property, source);
        }

        editor.ItemsSource = values;
        editor.SetBinding(Picker.SelectedItemProperty, new Binding(property.Name, source: source));
        editor.Title = property.Name;
        return new VerticalStackLayout
        {
            new Label { Text = property.Name },
            editor
        };
    }

    public static View CreateSelectionViewForValues(Array values, PropertyInfo property, object source)
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
        var editor = new Picker();

        editor.ItemsSource = typeof(Keyboard)
            .GetProperties(BindingFlags.Public | BindingFlags.Static)
            .Select(x => x.GetValue(null))
            .ToArray();

        editor.SetBinding(Picker.SelectedItemProperty, new Binding(property.Name, source: source));
        editor.Title = property.Name;
        return new VerticalStackLayout
        {
            new Label { Text = property.Name },
            editor
        };
    }

    public static View EditorForDateTime(PropertyInfo property, object source)
    {
        var editor = new DatePicker();
        editor.SetBinding(DatePicker.DateProperty, new Binding(property.Name, source: source));

        return new VerticalStackLayout
        {
            new Label { Text = property.Name },
            editor
        };
    }

    public static View EditorForTimeSpan(PropertyInfo property, object source)
    {
        var editor = new TimePicker();
        editor.SetBinding(TimePicker.TimeProperty, new Binding(property.Name, source: source));

        return new VerticalStackLayout
        {
            new Label { Text = property.Name },
            editor
        };
    }
}
