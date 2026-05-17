using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.Collections.ObjectModel;
using System.Linq;
using UraniumUI.Dialogs;
using UraniumUI.Infrastructure;
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
            builder.Services.AddSingleton<MockDialogService>();
            builder.Services.AddSingleton<IDialogService>(services => services.GetRequiredService<MockDialogService>());
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

    [Fact]
    public void ChangeSelectedItemsColor_ShouldUpdateExistingChips()
    {
        var control = AnimationReadyHandler.Prepare(new MultiplePickerField());
        control.SelectedItems = new ObservableCollection<object>
        {
            "Option 1"
        };

        control.SelectedItemsColor = Colors.Red;

        GetChips(control).Single().BackgroundColor.ShouldBe(Colors.Red);
    }

    [Fact]
    public void PickSelections_ShouldPassSelectedItemsColorToDialogService()
    {
        var control = AnimationReadyHandler.Prepare(new MultiplePickerField());
        var dialogService = UraniumServiceProvider.Current.GetRequiredService<MockDialogService>();
        control.ItemsSource = new[] { "Option 1" };
        control.SelectedItems = new ObservableCollection<object>();
        control.SelectedItemsColor = Colors.Red;

        ((TapGestureRecognizer)control.GestureRecognizers[0]).Command.Execute(null);

        dialogService.LastCheckBoxPromptColor.ShouldBe(Colors.Red);
    }

    [Fact]
    public void SetSelectedItems_ShouldRefreshLayout()
    {
        var control = AnimationReadyHandler.Prepare(new TestMultiplePickerField());

        control.SelectedItems = new ObservableCollection<object>
        {
            "Option 1"
        };

        control.RefreshChipLayoutCallCount.ShouldBe(1);
    }

    [Fact]
    public void ChangeSelectedItems_ShouldRefreshLayout()
    {
        var selectedItems = new ObservableCollection<object>();
        var control = AnimationReadyHandler.Prepare(new TestMultiplePickerField());
        control.SelectedItems = selectedItems;

        selectedItems.Add("Option 1");

        control.RefreshChipLayoutCallCount.ShouldBe(2);
    }

    private sealed class TestMultiplePickerField : MultiplePickerField
    {
        public int RefreshChipLayoutCallCount { get; private set; }

        protected override void RefreshChipLayout()
        {
            RefreshChipLayoutCallCount++;
            base.RefreshChipLayout();
        }
    }

    private static IReadOnlyList<Chip> GetChips(MultiplePickerField control)
        => ((FlexLayout)control.MainContentView.Content).Children.OfType<Chip>().ToList();

    public class TestViewModel : UraniumBindableObject
    {
        public string[] ItemsSource { get; set; } = new string[] { "Option 1", "Option 2", "Option 3", "Option 4", };

        public ObservableCollection<string> SelectedItems { get; } = new();
    }
}
