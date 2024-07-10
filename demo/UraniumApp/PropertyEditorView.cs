using InputKit.Shared.Controls;
using InputKit.Shared.Layouts;
using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UraniumApp.Inputs.ColorPicking;
using UraniumUI;
using UraniumUI.Dialogs;
using UraniumUI.Material.Controls;
using UraniumUI.Resources;
using UraniumUI.Views;

namespace UraniumApp
{
    public class PropertyEditorView : ContentView
    {

        public object Value { get => GetValue(ValueProperty); set => SetValue(ValueProperty, value); }
        public static readonly BindableProperty ValueProperty = BindableProperty.Create(
            nameof(Value),
            typeof(object),
            typeof(PropertyEditorView),
            propertyChanged: (bindable, oldValue, newValue) => (bindable as PropertyEditorView).Render());

        public bool ShowMissingProperties { get; set; } = true;

        public bool Hierarchical { get; set; } = false;

        public Type HierarchyLimitType { get; set; } = typeof(object);

        public static Dictionary<Type, Func<BindableProperty, object, View>> EditorMapping = new()
        {
            { typeof(string), EditorForString },
            { typeof(int), EditorForNumeric },
            { typeof(double), EditorForNumeric },
            { typeof(float), EditorForNumeric },
            { typeof(bool), EditorForBoolean },
            { typeof(Color), EditorForColor },
            { typeof(Keyboard), EditorForKeyboard },
            { typeof(Enum), EditorForEnum }
        };

        private VerticalStackLayout _propertiesContainer = new VerticalStackLayout
        {
            Spacing = 20,
            Padding = 10,
        };

        private ContentView _footerView = new ContentView();
        public View Footer
        {
            get => _footerView.Content; set
            {
                _footerView.Content = value;
                if (_propertiesContainer.Children.Count > 0 && !_propertiesContainer.Contains(_footerView))
                {
                    _propertiesContainer.Children.Add(_footerView);
                }
            }
        }

        public IDialogService DialogService { get; private set; }

        public PropertyEditorView()
        {
            DialogService = UraniumServiceProvider.Current.GetService<IDialogService>();

            var _titleContainer = new Label()
            {
                Text = "Properties",
                FontSize = 20,
                Margin = 10,
                FontAttributes = FontAttributes.Bold,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            };
            var _grid = new Grid
            {
                RowDefinitions =
                {
                    new RowDefinition(GridLength.Auto),
                    new RowDefinition(GridLength.Auto),
                    new RowDefinition(GridLength.Star),
                },
                Children =
                {
                    _titleContainer,
                    new BoxView { StyleClass = new List<string> { "Divider" }, Margin = 0 },
                    new ScrollView
                    {
                        Content = _propertiesContainer
                    },
                }
            };

            for (int i = 0; i < _grid.Children.Count; i++)
            {
                Grid.SetRow(_grid.Children[i] as View, i);
            }

            Content = _grid;
        }

        public List<BindableProperty> EditingProperties { get => (List<BindableProperty>)GetValue(EditingPropertiesProperty); set => SetValue(EditingPropertiesProperty, value); }

        public static readonly BindableProperty EditingPropertiesProperty = BindableProperty.Create(
            nameof(EditingProperties),
            typeof(List<BindableProperty>),
            typeof(PropertyEditorView),
            defaultBindingMode: BindingMode.OneWayToSource,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                if (bindable is PropertyEditorView propertyEditor && propertyEditor.EditingProperties != (List<BindableProperty>)newValue)
                {
                    (bindable as PropertyEditorView).Render();
                }
            });

        public void Render()
        {
            try
            {


                if (Value is null)
                {
                    _propertiesContainer.Children.Clear();
                    return;
                }
                var flags = BindingFlags.Static | BindingFlags.Public;
                if (Hierarchical)
                {
                    flags |= BindingFlags.FlattenHierarchy;
                }

                EditingProperties = Value.GetType().
                    GetFields(flags)
                    .Where(x => x.FieldType == typeof(BindableProperty) && HierarchyLimitType.IsAssignableFrom(x.DeclaringType))
                    .Select(x => x.GetValue(null) as BindableProperty)
                    .ToList();

                foreach (var bindableProperty in EditingProperties)
                {
                    var createEditor = EditorMapping.FirstOrDefault(x => x.Key.IsAssignableFrom(bindableProperty.ReturnType)).Value;
                    if (createEditor != null)
                    {
                        _propertiesContainer.Children.Add(createEditor(bindableProperty, Value));
                    }
                    else if (ShowMissingProperties)
                    {
                        _propertiesContainer.Children.Add(new Label
                        {
                            Text = $"No editor for {bindableProperty.PropertyName} ({bindableProperty.ReturnType})",
                            FontAttributes = FontAttributes.Italic
                        });
                    }
                }

                if (_footerView.Content != null)
                {
                    _propertiesContainer.Children.Add(_footerView);
                }
            }
            catch (Exception ex)
            {
                DialogService?.DisplayViewAsync("Error", new Label
                {
                    Text = ex.Message,
                    Margin = 20
                });
            }
        }

        public static View EditorForString(BindableProperty bindableProperty, object source)
        {
            var editor = new TextField();
            editor.SetBinding(TextField.TextProperty, new Binding(bindableProperty.PropertyName, source: source));
            editor.AllowClear = true;
            editor.Title = bindableProperty.PropertyName;

            return editor;
        }

        public static View EditorForNumeric(BindableProperty bindableProperty, object source)
        {
            var editor = new TextField();
            editor.SetBinding(TextField.TextProperty, new Binding(bindableProperty.PropertyName, source: source));
            editor.Title = bindableProperty.PropertyName;
            editor.AllowClear = false;
            editor.Keyboard = Keyboard.Numeric;

            return editor;
        }

        public static View EditorForBoolean(BindableProperty bindableProperty, object source)
        {
            var editor = new UraniumUI.Material.Controls.CheckBox();
            editor.SetBinding(UraniumUI.Material.Controls.CheckBox.IsCheckedProperty, new Binding(bindableProperty.PropertyName, source: source));
            editor.Text = bindableProperty.PropertyName;

            return editor;
        }

        public static View EditorForEnum(BindableProperty bindableProperty, object source)
        {
            var editor = new PickerField();

            var values = Enum.GetValues(bindableProperty.ReturnType);
            if (values.Length <= 5)
            {
                return CreateSelectionViewForValues(values, bindableProperty, source);
            }

            editor.ItemsSource = values;
            editor.SetBinding(PickerField.SelectedItemProperty, new Binding(bindableProperty.PropertyName, source: source));
            editor.Title = bindableProperty.PropertyName;
            editor.AllowClear = false;
            return editor;
        }

        private static View CreateSelectionViewForValues(Array values, BindableProperty bindableProperty, object source)
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

            editor.SetBinding(SelectionView.SelectedItemProperty, new Binding(bindableProperty.PropertyName, source: source));

            return new VerticalStackLayout
            {
                Spacing = 6,
                Children = {
                    new Label { Text = bindableProperty.PropertyName },
                    editor
                }
            };
        }

        public static View EditorForColor(BindableProperty bindableProperty, object source)
        {
            var boxPreview = new BoxView { HeightRequest = 25, WidthRequest = 25 };
            boxPreview.SetBinding(BoxView.ColorProperty, new Binding(bindableProperty.PropertyName, source: source));
            var labelPropertyName = new Label { Text = bindableProperty.PropertyName, VerticalOptions = LayoutOptions.Center };

            var editor = new StatefulContentView
            {
                Content = new HorizontalStackLayout
                {
                    Spacing = 12,
                    Children =
                    {
                        boxPreview,
                        labelPropertyName
                    }
                },
                TappedCommand = new Command(async () =>
                {
                    await UraniumServiceProvider.Current.GetRequiredService<IColorPicker>()
                    .PickCollorForAsync(source, bindableProperty.PropertyName);
                })
            };

            return editor;
        }

        public static View EditorForKeyboard(BindableProperty bindableProperty, object source)
        {
            var editor = new PickerField();

            editor.ItemsSource = typeof(Keyboard)
                .GetProperties(BindingFlags.Public | BindingFlags.Static)
                .Select(x => x.GetValue(null))
                .ToArray();

            editor.SetBinding(PickerField.SelectedItemProperty, new Binding(bindableProperty.PropertyName, source: source));
            editor.Title = bindableProperty.PropertyName;
            editor.AllowClear = false;
            return editor;
        }
    }
}
