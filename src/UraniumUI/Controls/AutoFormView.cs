using InputKit.Shared.Controls;
using System.Reflection;
using UraniumUI.Extensions;
using UraniumUI.Resources;
using UraniumUI.Options;
using Microsoft.Extensions.Options;
using InputKit.Shared.Abstraction;
using InputCheckBox = InputKit.Shared.Controls.CheckBox;

namespace UraniumUI.Controls;
public class AutoFormView : FormView
{
    public object Source { get => GetValue(SourceProperty); set => SetValue(SourceProperty, value); }
    public static readonly BindableProperty SourceProperty = BindableProperty.Create(
        nameof(Source),
        typeof(object),
        typeof(AutoFormView),
        propertyChanged: (bindable, oldValue, newValue) => (bindable as AutoFormView).OnSourceChanged());

    public IEnumerable<AutoFormProperty> PropertyDefinitions { get => (IEnumerable<AutoFormProperty>)GetValue(PropertyDefinitionsProperty); set => SetValue(PropertyDefinitionsProperty, value); }
    public static readonly BindableProperty PropertyDefinitionsProperty = BindableProperty.Create(
        nameof(PropertyDefinitions),
        typeof(IEnumerable<AutoFormProperty>),
        typeof(AutoFormView),
        propertyChanged: (bindable, oldValue, newValue) => (bindable as AutoFormView).OnSourceChanged());

    public Func<object, IEnumerable<AutoFormProperty>> PropertyProvider { get => (Func<object, IEnumerable<AutoFormProperty>>)GetValue(PropertyProviderProperty); set => SetValue(PropertyProviderProperty, value); }
    public static readonly BindableProperty PropertyProviderProperty = BindableProperty.Create(
        nameof(PropertyProvider),
        typeof(Func<object, IEnumerable<AutoFormProperty>>),
        typeof(AutoFormView),
        propertyChanged: (bindable, oldValue, newValue) => (bindable as AutoFormView).OnSourceChanged());

    public PropertyInfo[] EditingProperties { get; protected set; } = Array.Empty<PropertyInfo>();

    public IReadOnlyList<AutoFormProperty> EditingPropertyDefinitions { get; protected set; } = Array.Empty<AutoFormProperty>();

    public bool ShowSubmitButton { get => (bool)GetValue(ShowSubmitbuttonProperty); set => SetValue(ShowSubmitbuttonProperty, value); }

    public static readonly BindableProperty ShowSubmitbuttonProperty = BindableProperty.Create(
        nameof(ShowSubmitButton), typeof(bool), typeof(AutoFormView), defaultValue: true,
        propertyChanged: (bindable, oldValue, newValue) => (bindable as AutoFormView).OnShowSubmitButtonChanged());

    public bool ShowResetButton { get => (bool)GetValue(ShowResetButtonProperty); set => SetValue(ShowResetButtonProperty, value); }

    public static readonly BindableProperty ShowResetButtonProperty = BindableProperty.Create(
        nameof(ShowResetButton), typeof(bool), typeof(AutoFormView), defaultValue: true,
        propertyChanged: (bindable, oldValue, newValue) => (bindable as AutoFormView).OnShowResetButtonChanged());

    public string SubmitButtonText { get => (string)GetValue(SubmitButtonTextProperty); set => SetValue(SubmitButtonTextProperty, value); }

    public static readonly BindableProperty SubmitButtonTextProperty = BindableProperty.Create(
        nameof(SubmitButtonText), typeof(string), typeof(AutoFormView), defaultValue: "Submit",
        propertyChanged: (bindable, oldValue, newValue) => (bindable as AutoFormView).OnSubmitButtonTextChanged());

    public string ResetButtonText { get => (string)GetValue(ResetButtonTextProperty); set => SetValue(ResetButtonTextProperty, value); }

    public static readonly BindableProperty ResetButtonTextProperty = BindableProperty.Create(
        nameof(ResetButtonText), typeof(string), typeof(AutoFormView), defaultValue: "Reset",
        propertyChanged: (bindable, oldValue, newValue) => (bindable as AutoFormView).OnResetButtonTextChanged());

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

    private Layout _footerLayout = new VerticalStackLayout { Spacing = 12 };

    public Layout FooterLayout
    {
        get => _footerLayout;
        set
        {
            ItemsLayout.Remove(_footerLayout);
            while (ItemsLayout.Children.Count > 0)
            {
                var item = _footerLayout.Children.First();
                ItemsLayout.Children.Remove(item);
                value?.Children.Add(item);
            }
            _footerLayout = value;
            if (ItemsLayout.Children.Count > 0 && _footerLayout != null)
            {
                ItemsLayout.Children.Add(_footerLayout);
            }
        }
    }

    protected AutoFormViewOptions Options { get; }
    public AutoFormView()
    {
        Children.Add(_itemsLayout);

        Options = UraniumServiceProvider.Current.GetRequiredService<IOptions<AutoFormViewOptions>>().Value;
    }

    protected void OnSourceChanged()
    {
        ValidationModel = Source;

        EditingPropertyDefinitions = Source is null
            ? Array.Empty<AutoFormProperty>()
            : ResolvePropertyDefinitions().ToArray();
        EditingProperties = EditingPropertyDefinitions
            .Select(x => x.PropertyInfo)
            .Where(x => x != null)
            .ToArray();

        Render();
    }

    protected virtual IEnumerable<AutoFormProperty> ResolvePropertyDefinitions()
    {
        if (PropertyDefinitions != null)
        {
            return PropertyDefinitions;
        }

        if (PropertyProvider != null)
        {
            return PropertyProvider(Source) ?? Enumerable.Empty<AutoFormProperty>();
        }

        if (Options.PropertyProvider != null)
        {
            return Options.PropertyProvider(Source) ?? Enumerable.Empty<AutoFormProperty>();
        }

        return CreatePropertyDefinitionsFromReflection();
    }

    protected virtual IEnumerable<AutoFormProperty> CreatePropertyDefinitionsFromReflection()
    {
        var flags = BindingFlags.Public | BindingFlags.Instance;
        if (Hierarchical)
        {
            flags |= BindingFlags.FlattenHierarchy;
        }

        return Source?.GetType()
            .GetProperties(flags)
            .Where(x => HierarchyLimitType.IsAssignableFrom(x.PropertyType))
            .Select(AutoFormProperty.FromPropertyInfo)
            ?? Enumerable.Empty<AutoFormProperty>();
    }

    private void Render()
    {
        if (Source is null)
        {
            _itemsLayout.Children.Clear();
            return;
        }

        using (_itemsLayout.Batch())
        {
            _itemsLayout.Children.Clear();

            foreach (var property in EditingPropertyDefinitions)
            {
                var editor = CreateEditor(property);
                if (editor != null)
                {
                    SetEditorValidationPath(editor, property.Name);

                    if (property.PropertyInfo != null)
                    {
                        foreach (var action in Options.PostEditorActions)
                        {
                            action(editor, property.PropertyInfo);
                        }
                    }

                    foreach (var action in Options.PostPropertyEditorActions)
                    {
                        action(editor, property);
                    }

                    if (editor is IValidatable validatable)
                    {
                        validatable.Validations.AddRange(Options.CreateValidations(property));
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

            if (!_itemsLayout.Children.Contains(_footerLayout))
            {
                _itemsLayout.Children.Add(_footerLayout);
                OnShowSubmitButtonChanged();
                OnShowResetButtonChanged();
            }
        }
    }

    protected virtual View CreateEditor(AutoFormProperty property)
    {
        var createLegacyEditor = Options.GetLegacyEditor(property);
        if (createLegacyEditor != null)
        {
            return createLegacyEditor(property.PropertyInfo, Options.PropertyNameFactory, Source);
        }

        var createEditor = Options.GetPropertyEditor(property);
        return createEditor?.Invoke(property, Options.GetPropertyDisplayName, Source);
    }

    Button? submitButton;
    protected virtual void OnShowSubmitButtonChanged()
    {
        if (ShowSubmitButton)
        {
            if (submitButton is null)
            {
                submitButton = new Button
                {
                    Text = SubmitButtonText,
                    StyleClass = new[] { "FilledButton" },
                    Command = buttonSubmitCommand,
                };
                SetIsSubmitButton(submitButton, true);
                _footerLayout.Children.Insert(0, submitButton);
            }
        }
        else
        {
            if (submitButton is not null)
            {
                _footerLayout.Children.Remove(submitButton);
                submitButton = null;
            }
        }
    }

    Button? resetButton;
    protected virtual void OnShowResetButtonChanged()
    {
        if (ShowResetButton)
        {
            if (resetButton is null)
            {
                resetButton = new Button
                {
                    Text = ResetButtonText,
                    StyleClass = new[] { "OutlinedButton" },
                    Command = buttonResetCommand,
                };
                SetIsResetButton(resetButton, true);
                _footerLayout.Children.Add(resetButton);
            }
        }
        else
        {
            if (resetButton != null)
            {
                _footerLayout.Children.Remove(resetButton);
                resetButton = null;
            }
        }
    }

    protected virtual void OnSubmitButtonTextChanged()
    {
        if (submitButton != null)
        {
            submitButton.Text = SubmitButtonText;
        }
    }

    protected virtual void OnResetButtonTextChanged()
    {
        if (resetButton != null)
        {
            resetButton.Text = ResetButtonText;
        }
    }

    private static void SetEditorValidationPath(Element editor, string validationPath)
    {
        if (editor is BindableObject bindableObject)
        {
            SetValidationPath(bindableObject, validationPath);
        }

        foreach (var child in GetEditorChildren(editor))
        {
            SetEditorValidationPath(child, validationPath);
        }
    }

    private static IEnumerable<Element> GetEditorChildren(Element editor)
    {
        if (editor is Layout layout)
        {
            foreach (var child in layout.Children.OfType<Element>())
            {
                yield return child;
            }
        }

        if (editor is ContentView contentView && contentView.Content is Element content)
        {
            yield return content;
        }

        if (editor is ScrollView scrollView && scrollView.Content is Element scrollContent)
        {
            yield return scrollContent;
        }

        if (editor is Border border && border.Content is Element borderContent)
        {
            yield return borderContent;
        }
    }

    public static View EditorForString(PropertyInfo property, Func<PropertyInfo, string> propertyNameFactory, object source)
    {
        return EditorForString(AutoFormProperty.FromPropertyInfo(property), _ => propertyNameFactory(property), source);
    }

    public static View EditorForString(AutoFormProperty property, Func<AutoFormProperty, string> propertyNameFactory, object source)
    {
        var editor = new Entry();
        editor.SetBinding(Entry.TextProperty, new Binding(property.Name, source: source));

        return new VerticalStackLayout
        {
            new Label { Text = propertyNameFactory(property) },
            editor
        };
    }

    public static View EditorForNumeric(PropertyInfo property, Func<PropertyInfo, string> propertyNameFactory, object source)
    {
        return EditorForNumeric(AutoFormProperty.FromPropertyInfo(property), _ => propertyNameFactory(property), source);
    }

    public static View EditorForNumeric(AutoFormProperty property, Func<AutoFormProperty, string> propertyNameFactory, object source)
    {
        var editor = new Entry();
        editor.SetBinding(Entry.TextProperty, new Binding(property.Name, source: source));
        editor.Keyboard = Keyboard.Numeric;

        return new VerticalStackLayout
        {
            new Label { Text = propertyNameFactory(property) },
            editor
        };
    }

    public static View EditorForBoolean(PropertyInfo property, Func<PropertyInfo, string> propertyNameFactory, object source)
    {
        return EditorForBoolean(AutoFormProperty.FromPropertyInfo(property), _ => propertyNameFactory(property), source);
    }

    public static View EditorForBoolean(AutoFormProperty property, Func<AutoFormProperty, string> propertyNameFactory, object source)
    {
        var editor = new InputCheckBox();
        editor.SetBinding(InputCheckBox.IsCheckedProperty, new Binding(property.Name, source: source));
        editor.Text = propertyNameFactory(property);

        return editor;
    }

    public static View EditorForEnum(PropertyInfo property, Func<PropertyInfo, string> propertyNameFactory, object source)
    {
        return EditorForEnum(AutoFormProperty.FromPropertyInfo(property), _ => propertyNameFactory(property), source);
    }

    public static View EditorForEnum(AutoFormProperty property, Func<AutoFormProperty, string> propertyNameFactory, object source)
    {
        var editor = new Picker();

        var values = Enum.GetValues(property.PropertyType.AsNonNullable());
        if (values.Length <= 5)
        {
            return CreateSelectionViewForValues(values, property, propertyNameFactory, source);
        }

        editor.ItemsSource = values;
        editor.SetBinding(Picker.SelectedItemProperty, new Binding(property.Name, source: source));
        var title = propertyNameFactory(property);
        editor.Title = title;
        return new VerticalStackLayout
        {
            new Label { Text = title },
            editor
        };
    }

    public static View CreateSelectionViewForValues(Array values, PropertyInfo property, Func<PropertyInfo, string> propertyNameFactory, object source)
    {
        return CreateSelectionViewForValues(values, AutoFormProperty.FromPropertyInfo(property), _ => propertyNameFactory(property), source);
    }

    public static View CreateSelectionViewForValues(Array values, AutoFormProperty property, Func<AutoFormProperty, string> propertyNameFactory, object source)
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
                    new Label { Text = propertyNameFactory(property) },
                    editor
                }
        };
    }

    public static View EditorForKeyboard(PropertyInfo property, Func<PropertyInfo, string> propertyNameFactory, object source)
    {
        return EditorForKeyboard(AutoFormProperty.FromPropertyInfo(property), _ => propertyNameFactory(property), source);
    }

    public static View EditorForKeyboard(AutoFormProperty property, Func<AutoFormProperty, string> propertyNameFactory, object source)
    {
        var editor = new Picker();

        editor.ItemsSource = typeof(Keyboard)
            .GetProperties(BindingFlags.Public | BindingFlags.Static)
            .Select(x => x.GetValue(null))
            .ToArray();

        editor.SetBinding(Picker.SelectedItemProperty, new Binding(property.Name, source: source));
        var title = propertyNameFactory(property);
        editor.Title = title;
        return new VerticalStackLayout
        {
            new Label { Text = title },
            editor
        };
    }

    public static View EditorForDateTime(PropertyInfo property, Func<PropertyInfo, string> propertyNameFactory, object source)
    {
        return EditorForDateTime(AutoFormProperty.FromPropertyInfo(property), _ => propertyNameFactory(property), source);
    }

    public static View EditorForDateTime(AutoFormProperty property, Func<AutoFormProperty, string> propertyNameFactory, object source)
    {
        var editor = new DatePicker();
        editor.SetBinding(DatePicker.DateProperty, new Binding(property.Name, source: source));

        return new VerticalStackLayout
        {
            new Label { Text = propertyNameFactory(property) },
            editor
        };
    }

    public static View EditorForTimeSpan(PropertyInfo property, Func<PropertyInfo, string> propertyNameFactory, object source)
    {
        return EditorForTimeSpan(AutoFormProperty.FromPropertyInfo(property), _ => propertyNameFactory(property), source);
    }

    public static View EditorForTimeSpan(AutoFormProperty property, Func<AutoFormProperty, string> propertyNameFactory, object source)
    {
        var editor = new TimePicker();
        editor.SetBinding(TimePicker.TimeProperty, new Binding(property.Name, source: source));

        return new VerticalStackLayout
        {
            new Label { Text = propertyNameFactory(property) },
            editor
        };
    }
}
