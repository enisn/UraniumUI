using System.Collections.ObjectModel;
using UraniumUI.Material.Controls;
using UraniumUI.Tests.Core;

namespace UraniumUI.Material.Tests.Controls;

public class MultiplePickerField_Test
{
    public MultiplePickerField_Test()
    {
        ApplicationExtensions.CreateAndSetMockApplication();
    }
    
    private string[] GetItemsSource() => new string[] { "Option 1", "Option 2", "Option 3", "Option 4", };

    [Fact]
    public void Initialize_WithSelectedItems()
    {
        var control = AnimationReadyHandler.Prepare(new MultiplePickerField());
        var viewModel = new TestViewModel();
        viewModel.SelectedItems.Add(viewModel.ItemsSource[0]);
        viewModel.SelectedItems.Add(viewModel.ItemsSource[2]);

        control.BindingContext = viewModel;
        control.ItemsSource = viewModel.ItemsSource;

        control.SetBinding(MultiplePickerField.SelectedItemsProperty, new Binding(nameof(TestViewModel.SelectedItems)));

        // Assert
        Assert.True(control.SelectedItems.Contains(viewModel.ItemsSource[0]));
        Assert.True(control.SelectedItems.Contains(viewModel.ItemsSource[2]));
    }

    public class TestViewModel : UraniumBindableObject
    {
        public string[] ItemsSource { get; set; } = new string[] { "Option 1", "Option 2", "Option 3", "Option 4", };

        public ObservableCollection<string> SelectedItems { get; } = new();
    }
}
