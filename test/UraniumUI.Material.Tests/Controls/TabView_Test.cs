using Shouldly;
using System.Collections.ObjectModel;
using System.Linq;
using UraniumUI.Dialogs;
using UraniumUI.Material.Controls;
using UraniumUI.Material.Tests.Mocks;
using UraniumUI.Tests.Core;
using UraniumUI.Views;

namespace UraniumUI.Material.Tests.Controls;

public class TabView_Test
{
    public TabView_Test()
    {
        ApplicationExtensions.CreateAndSetMockApplication(builder =>
        {
            builder.Services.AddSingleton<IDialogService, MockDialogService>();
        });
    }

    [Fact]
    public void CacheOnCodeBehind_ShouldKeepMultiplePickerFieldSelection_WhenSwitchingTabs()
    {
        var viewModel = new MultiplePickerViewModel();
        viewModel.SelectedItems.Add(viewModel.ItemsSource[0]);

        var pickerField = AnimationReadyHandler.Prepare(new TestMultiplePickerField());
        pickerField.SetBinding(MultiplePickerField.ItemsSourceProperty, new Binding(nameof(MultiplePickerViewModel.ItemsSource)));
        pickerField.SetBinding(MultiplePickerField.SelectedItemsProperty, new Binding(nameof(MultiplePickerViewModel.SelectedItems)));

        var firstTabContent = new VerticalStackLayout();
        firstTabContent.Add(pickerField);

        var tabView = AnimationReadyHandler.Prepare(new TabView
        {
            BindingContext = viewModel,
            TabHeaderItemTemplate = CreateTestHeaderTemplate(),
            UseAnimation = false,
        });

        tabView.Tabs.Add(new TabItem { Title = "Picker", Content = firstTabContent });
        tabView.Tabs.Add(new TabItem { Title = "Other", Content = new Label { Text = "Other" } });

        pickerField.GetChipTexts().ShouldBe(new[] { "Option 1" });

        tabView.SelectedTab = tabView.Tabs[1];
        tabView.SelectedTab = tabView.Tabs[0];

        pickerField.SelectedItems.ShouldBeSameAs(viewModel.SelectedItems);
        pickerField.GetChipTexts().ShouldBe(new[] { "Option 1" });

        viewModel.SelectedItems.Add(viewModel.ItemsSource[1]);

        pickerField.GetChipTexts().ShouldBe(new[] { "Option 1", "Option 2" });
    }

    [Fact]
    public void CacheOnCodeBehind_ShouldKeepPickerFieldSelectedIndex_WhenSwitchingTabs()
    {
        var viewModel = new PickerViewModel { SelectedIndex = 1 };

        var pickerField = AnimationReadyHandler.Prepare(new PickerField());
        pickerField.SetBinding(PickerField.ItemsSourceProperty, new Binding(nameof(PickerViewModel.ItemsSource)));
        pickerField.SetBinding(PickerField.SelectedIndexProperty, new Binding(nameof(PickerViewModel.SelectedIndex)));

        var firstTabContent = new VerticalStackLayout();
        firstTabContent.Add(pickerField);

        var tabView = AnimationReadyHandler.Prepare(new TabView
        {
            BindingContext = viewModel,
            TabHeaderItemTemplate = CreateTestHeaderTemplate(),
            UseAnimation = false,
        });

        tabView.Tabs.Add(new TabItem { Title = "Picker", Content = firstTabContent });
        tabView.Tabs.Add(new TabItem { Title = "Other", Content = new Label { Text = "Other" } });

        pickerField.SelectedIndex.ShouldBe(1);

        tabView.SelectedTab = tabView.Tabs[1];
        tabView.SelectedTab = tabView.Tabs[0];

        pickerField.SelectedIndex.ShouldBe(1);
        viewModel.SelectedIndex.ShouldBe(1);
    }

    [Fact]
    public void CustomHeaderTemplate_ShouldBeWrappedInFocusableHeader_WithSemanticState()
    {
        var tabView = AnimationReadyHandler.Prepare(new TabView
        {
            TabHeaderItemTemplate = CreateTestHeaderTemplate(),
            UseAnimation = false,
        });

        tabView.Tabs.Add(new TabItem { Title = "First", Content = new Label { Text = "First content" } });
        tabView.Tabs.Add(new TabItem { Title = "Second", Content = new Label { Text = "Second content" } });

        var firstHeader = tabView.Tabs[0].Header.ShouldBeOfType<StatefulContentView>();
        var secondHeader = tabView.Tabs[1].Header.ShouldBeOfType<StatefulContentView>();

        firstHeader.IsFocusable.ShouldBeTrue();
        firstHeader.TappedCommand.ShouldNotBeNull();
        SemanticProperties.GetDescription(firstHeader).ShouldBe("First, selected");
        SemanticProperties.GetHint(firstHeader).ShouldBe("Selects this tab.");

        tabView.SelectedTab = tabView.Tabs[1];

        SemanticProperties.GetDescription(firstHeader).ShouldBe("First");
        SemanticProperties.GetDescription(secondHeader).ShouldBe("Second, selected");
    }

    [Fact]
    public void TabHeaderItemTemplate_ShouldUseItemsSourceItemBindingContext()
    {
        var items = new ObservableCollection<HeaderItem>
        {
            new("First"),
            new("Second"),
        };

        var tabView = AnimationReadyHandler.Prepare(new TabView
        {
            TabHeaderItemTemplate = CreateItemsSourceHeaderTemplate(),
            ItemTemplate = CreateTestHeaderTemplate(),
            ItemsSource = items,
            UseAnimation = false,
        });

        tabView.Tabs.Count.ShouldBe(2);
        var firstHeaderContent = GetWrappedHeaderContent<Label>(tabView.Tabs[0]);

        firstHeaderContent.BindingContext.ShouldBeSameAs(items[0]);
        firstHeaderContent.Text.ShouldBe("First");
    }

    [Fact]
    public void TabHeaderItemTemplate_ShouldExposeSelectedStateForItemsSourceHeaders()
    {
        var items = new ObservableCollection<HeaderItem>
        {
            new("First"),
            new("Second"),
        };

        var tabView = AnimationReadyHandler.Prepare(new TabView
        {
            TabHeaderItemTemplate = CreateItemsSourceHeaderTemplate(),
            ItemTemplate = CreateTestHeaderTemplate(),
            ItemsSource = items,
            UseAnimation = false,
        });

        var firstHeaderContent = GetWrappedHeaderContent<Label>(tabView.Tabs[0]);
        var secondHeaderContent = GetWrappedHeaderContent<Label>(tabView.Tabs[1]);

        TabView.GetIsHeaderSelected(firstHeaderContent).ShouldBeTrue();
        TabView.GetIsHeaderSelected(secondHeaderContent).ShouldBeFalse();

        tabView.SelectedTab = tabView.Tabs[1];

        TabView.GetIsHeaderSelected(firstHeaderContent).ShouldBeFalse();
        TabView.GetIsHeaderSelected(secondHeaderContent).ShouldBeTrue();
    }

    [Fact]
    public void TabHeaderItemTemplate_ShouldSelectItemsSourceTabWithoutTemplateCommand()
    {
        var items = new ObservableCollection<HeaderItem>
        {
            new("First"),
            new("Second"),
        };

        var tabView = AnimationReadyHandler.Prepare(new TabView
        {
            TabHeaderItemTemplate = CreateItemsSourceHeaderTemplate(),
            ItemTemplate = CreateTestHeaderTemplate(),
            ItemsSource = items,
            UseAnimation = false,
        });

        var secondHeader = tabView.Tabs[1].Header.ShouldBeOfType<StatefulContentView>();

        secondHeader.TappedCommand.Execute(null);

        tabView.SelectedTab.ShouldBeSameAs(tabView.Tabs[1]);
        tabView.CurrentItem.ShouldBeSameAs(items[1]);
    }

    [Fact]
    public void ContentTemplate_ShouldKeepContentOwnBindingContextBinding()
    {
        var viewModel = new ParentViewModel();

        var tabView = AnimationReadyHandler.Prepare(new TabView
        {
            BindingContext = viewModel,
            TabHeaderItemTemplate = CreateTestHeaderTemplate(),
            UseAnimation = false,
        });

        tabView.Tabs.Add(new TabItem { Title = "Child", ContentTemplate = CreateChildBoundContentTemplate() });

        tabView.Tabs[0].Content.BindingContext.ShouldBeSameAs(viewModel.Child);
    }

    [Fact]
    public void ContentTemplate_ShouldKeepContentOwnBindingContextBinding_WhenSwitchingTabs()
    {
        var viewModel = new ParentViewModel();

        var tabView = AnimationReadyHandler.Prepare(new TabView
        {
            BindingContext = viewModel,
            TabHeaderItemTemplate = CreateTestHeaderTemplate(),
            UseAnimation = false,
        });

        tabView.Tabs.Add(new TabItem { Title = "Child", ContentTemplate = CreateChildBoundContentTemplate() });
        tabView.Tabs.Add(new TabItem { Title = "Other", Content = new Label { Text = "Other" } });

        tabView.SelectedTab = tabView.Tabs[1];
        tabView.SelectedTab = tabView.Tabs[0];

        tabView.Tabs[0].Content.BindingContext.ShouldBeSameAs(viewModel.Child);
    }

    private sealed class TestMultiplePickerField : MultiplePickerField
    {
        public string[] GetChipTexts()
        {
            return chipsHolderLayout.Children.OfType<Chip>().Select(chip => chip.Text).ToArray();
        }
    }

    private static DataTemplate CreateTestHeaderTemplate()
    {
        return new DataTemplate(() => new Label());
    }

    private static DataTemplate CreateItemsSourceHeaderTemplate()
    {
        return new DataTemplate(() =>
        {
            var label = new Label();
            label.SetBinding(Label.TextProperty, nameof(HeaderItem.Title));
            return label;
        });
    }

    private static TView GetWrappedHeaderContent<TView>(TabItem tabItem)
        where TView : View
    {
        var header = tabItem.Header.ShouldBeOfType<StatefulContentView>();
        return header.Content.ShouldBeOfType<TView>();
    }

    private static DataTemplate CreateChildBoundContentTemplate()
    {
        return new DataTemplate(() =>
        {
            var content = new Label();
            content.SetBinding(BindableObject.BindingContextProperty, nameof(ParentViewModel.Child));
            return content;
        });
    }

    private sealed class ParentViewModel
    {
        public object Child { get; } = new();
    }

    private sealed class HeaderItem
    {
        public HeaderItem(string title)
        {
            Title = title;
        }

        public string Title { get; }
    }

    private sealed class MultiplePickerViewModel
    {
        public ObservableCollection<string> ItemsSource { get; } = new()
        {
            "Option 1",
            "Option 2",
            "Option 3",
        };

        public ObservableCollection<string> SelectedItems { get; } = new();
    }

    private sealed class PickerViewModel
    {
        public ObservableCollection<string> ItemsSource { get; } = new()
        {
            "Option 1",
            "Option 2",
            "Option 3",
        };

        public int SelectedIndex { get; set; }
    }
}
