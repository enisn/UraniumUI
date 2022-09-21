using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using Path = Microsoft.Maui.Controls.Shapes.Path;

namespace UraniumUI.Material.Controls;
public partial class TreeView : VerticalStackLayout
{
    public static DataTemplate DefaultItemTemplate = new DataTemplate(() =>
    {
        var label = new Label { VerticalOptions = LayoutOptions.Center };
        label.SetBinding(Label.TextProperty, new Binding("Name"));
        return label;
    });

    public static DataTemplate CheckBoxItemTemplate = new DataTemplate(() =>
    {
        var checkBox = new CheckBox();
        checkBox.SetBinding(CheckBox.TextProperty, new Binding("Name"));

        checkBox.CheckChanged += (s, e) =>
        {
            var lastState = TreeView.GetIsSelected(checkBox);

            if (lastState == null)
            {
                TreeView.SetIsSelected(checkBox, false);
            }
            else
            {
                TreeView.SetIsSelected(checkBox, !lastState);
            }
        };

        VisualStateManager.SetVisualStateGroups(checkBox, new VisualStateGroupList
        {
            new VisualStateGroup
            {
                Name = "SelectionVisualStateGroup",
                States =
                {
                    new VisualState
                    {
                        Name = "Selected",
                        Setters =
                        {
                            new Setter { Property = CheckBox.IsCheckedProperty, Value = true },
                            new Setter { Property = CheckBox.IconGeometryProperty, Value = InputKit.Shared.Controls.PredefinedShapes.Check }
                        }
                    },
                    new VisualState
                    {
                        Name = "SemiSelected",
                        Setters =
                        {
                            new Setter { Property = CheckBox.IsCheckedProperty, Value = true },
                            new Setter { Property = CheckBox.IconGeometryProperty, Value = InputKit.Shared.Controls.PredefinedShapes.Line }
                        }
                    },
                    new VisualState
                    {
                        Name = "UnSelected",
                        Setters =
                        {
                            new Setter { Property = CheckBox.IsCheckedProperty, Value = false },
                            new Setter { Property = CheckBox.IconGeometryProperty, Value = InputKit.Shared.Controls.PredefinedShapes.Check }
                        }
                    }
                }
            }
        });

        return checkBox;
    });
    public TreeView()
    {
        BindableLayout.SetItemTemplate(this, new DataTemplate(() =>
        {
            return new TreeViewNodeHolderView(ItemTemplate, IsExpandedPropertyName);
        }));
    }

    public BindingBase ChildrenBinding { get; set; } = new Binding("Children");

    private string isExpandedBinding;

    public string IsExpandedPropertyName
    {
        get => isExpandedBinding;
        set
        {
            isExpandedBinding = value;
            foreach (TreeViewNodeHolderView view in this.Children.Where(x => x is TreeViewNodeHolderView))
            {
                view.IsTextendedProperty = value;
            }
        }
    }

    protected virtual void OnItemsSourceSet()
    {
        BindableLayout.SetItemsSource(this, ItemsSource);
    }

    private void OnItemTemplateChanged()
    {
        BindableLayout.SetItemTemplate(this, new DataTemplate(() =>
        {
            return new TreeViewNodeHolderView(ItemTemplate, IsExpandedPropertyName);
        }));
    }

    public IList ItemsSource { get => (IList)GetValue(ItemsSourceProperty); set => SetValue(ItemsSourceProperty, value); }

    public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(
        nameof(ItemsSource), typeof(IList), typeof(TreeView), null,
        propertyChanged: (b, o, v) => (b as TreeView).OnItemsSourceSet());

    public DataTemplate ItemTemplate { get => (DataTemplate)GetValue(ItemTemplateProperty); set => SetValue(ItemTemplateProperty, value); }

    public static readonly BindableProperty ItemTemplateProperty = BindableProperty.Create(
        nameof(ItemTemplate), typeof(DataTemplate), typeof(TreeView),
        defaultValue: CheckBoxItemTemplate, propertyChanged: (b, o, n) => (b as TreeView).OnItemTemplateChanged());

    public static readonly BindableProperty IsExpandedProperty =
        BindableProperty.CreateAttached("IsExpanded", typeof(bool), typeof(TreeViewNodeHolderView), false,
            propertyChanged: (bindable, oldValue, newValue) => (bindable as TreeViewNodeHolderView).OnIsExpandedChanged());

    public static bool GetIsExpanded(BindableObject view) => (bool)view.GetValue(IsExpandedProperty);

    public static void SetIsExpanded(BindableObject view, bool value) => view.SetValue(IsExpandedProperty, value);


    public static readonly BindableProperty IsSelectedProperty =
        BindableProperty.CreateAttached("IsSelected", typeof(bool?), typeof(View), false,
            propertyChanged: (bindable, oldValue, newValue) => ((bindable as View).Parent.Parent.Parent as TreeViewNodeHolderView).OnIsSelectedChanged());

    public static bool? GetIsSelected(BindableObject view) => (bool?)view.GetValue(IsSelectedProperty);

    public static void SetIsSelected(BindableObject view, bool? value) => view.SetValue(IsSelectedProperty, value);
}
