using Shouldly;
using System.Collections.ObjectModel;
using UraniumUI.Dialogs;
using UraniumUI.Material.Controls;
using UraniumUI.Material.Tests.Mocks;
using UraniumUI.Tests.Core;

namespace UraniumUI.Material.Tests.Controls;

public class MultiplePickerField_Test
{
    public MultiplePickerField_Test()
    {
        ApplicationExtensions.CreateAndSetMockApplication(builder =>
        {
            builder.Services.AddSingleton<IDialogService, MockDialogService>();
        });
    }

    [Fact]
    public void Initialize_WithSelectedItems()
    {
        var control = AnimationReadyHandler.Prepare(new MultiplePickerField());
        var viewModel = new TestViewModel();
        viewModel.SelectedItems.Add(viewModel.ItemsSource[0]);

        control.BindingContext = viewModel;
        control.ItemsSource = viewModel.ItemsSource;

        control.SetBinding(MultiplePickerField.SelectedItemsProperty, new Binding(nameof(TestViewModel.SelectedItems)));

        // Assert
        control.SelectedItems.Count.ShouldBe(1);
        control.SelectedItems[0].ShouldBe(viewModel.ItemsSource[0]);
    }

    [Fact]
    public void SelectedItemsColor_SetProperty_ShouldBeStored()
    {
        var control = AnimationReadyHandler.Prepare(new MultiplePickerField());

        control.SelectedItemsColor = Colors.Red;

        control.SelectedItemsColor.ShouldBe(Colors.Red);
    }

    [Fact]
    public void SelectedItemsColor_Default_ShouldBeNull()
    {
        var control = AnimationReadyHandler.Prepare(new MultiplePickerField());

        control.SelectedItemsColor.ShouldBeNull();
    }

    public class TestViewModel : UraniumBindableObject
    {
        public string[] ItemsSource { get; set; } = new string[] { "Option 1", "Option 2", "Option 3", "Option 4", };

        public ObservableCollection<string> SelectedItems { get; } = new();
    }
}
