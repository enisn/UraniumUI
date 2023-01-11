﻿using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UraniumUI.Material.Controls;

namespace UraniumApp
{
    public class PropertyEditorView : Frame
    {
        public object Value { get => GetValue(ValueProperty); set => SetValue(ValueProperty, value); }
        public static readonly BindableProperty ValueProperty = BindableProperty.Create(
            "Value",
            typeof(object),
            typeof(PropertyEditorView),
            propertyChanged: (bindable, oldValue, newValue)=> (bindable as PropertyEditorView).Render());

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

        public PropertyEditorView()
        {
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

        public void Render()
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

            var bindableProperties = Value.GetType().
                GetFields(flags)
                .Where(x => x.FieldType == typeof(BindableProperty) && HierarchyLimitType.IsAssignableFrom(x.DeclaringType))
                .Select(x => x.GetValue(null) as BindableProperty)
                .ToList();

            foreach (var bindableProperty in bindableProperties)
            {
                var createEditor = EditorMapping.FirstOrDefault(x => x.Key.IsAssignableFrom(bindableProperty.ReturnType)).Value;
                if (createEditor != null)
                {
                    _propertiesContainer.Children.Add(createEditor(bindableProperty, Value));
                }
                else if(ShowMissingProperties)
                {
                    _propertiesContainer.Children.Add(new Label
                    {
                        Text = $"No editor for {bindableProperty.PropertyName} ({bindableProperty.ReturnType})",
                        FontAttributes = FontAttributes.Italic
                    });
                }
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

            editor.ItemsSource = Enum.GetValues(bindableProperty.ReturnType);
            editor.SetBinding(PickerField.SelectedItemProperty, new Binding(bindableProperty.PropertyName, source: source));
            editor.Title = bindableProperty.PropertyName;
            editor.AllowClear = false;
            return editor;
        }

        public static View EditorForColor(BindableProperty bindableProperty, object source)
        {
            var editor = new PickerField();

            editor.ItemsSource = typeof(Colors)
                .GetFields(BindingFlags.Public | BindingFlags.Static)
                .Select(x => x.GetValue(null))
                .ToArray();

            editor.SetBinding(PickerField.SelectedItemProperty, new Binding(bindableProperty.PropertyName, source: source));
            editor.Title = bindableProperty.PropertyName;
            editor.AllowClear = false;
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