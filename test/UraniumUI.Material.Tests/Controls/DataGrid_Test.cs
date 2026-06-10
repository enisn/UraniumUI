using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shouldly;
using UraniumUI.Material.Controls;
using UraniumUI.Tests.Core;

namespace UraniumUI.Material.Tests.Controls;
public class DataGrid_Test
{
    public DataGrid_Test()
    {
        ApplicationExtensions.CreateAndSetMockApplication();
    }

    [Fact]
    public void LineSeparatorColor_BindingForInitialization_FromSource()
    {
        var control = AnimationReadyHandler.Prepare(new DataGrid());
        var viewModel = new { LineSeparatorColor = Colors.Black };
        control.BindingContext = viewModel;
        control.SetBinding(DataGrid.LineSeparatorColorProperty, new Binding(nameof(viewModel.LineSeparatorColor)));

        // Assert
        control.LineSeparatorColor.ShouldBe(viewModel.LineSeparatorColor);
    }

    [Fact]
    public void LineSeparatorColor_ShouldBeUpdated_FromSource()
    {
        var control = AnimationReadyHandler.Prepare(new DataGrid());
        var viewModel = new DataGridTestViewModel { LineSeparatorColor = Colors.Black };
        control.BindingContext = viewModel;
        control.SetBinding(DataGrid.LineSeparatorColorProperty, new Binding(nameof(viewModel.LineSeparatorColor)));

        // Act
        viewModel.LineSeparatorColor = Colors.Red;

        // Assert
        control.LineSeparatorColor.ShouldBe(viewModel.LineSeparatorColor);
    }

    [Fact]
    public void ShowHeaders_ShouldHideHeaderViews_AndMoveFirstRowToTop()
    {
        var control = AnimationReadyHandler.Prepare(new DataGrid
        {
            ShowHeaders = false,
            Columns =
            [
                new DataGridColumn { Title = "Id", ValueBinding = new Binding(nameof(DataGridRow.Id)) },
                new DataGridColumn { Title = "Name", ValueBinding = new Binding(nameof(DataGridRow.Name)) },
            ],
            ItemsSource = new List<DataGridRow>
            {
                new() { Id = 1, Name = "One" },
                new() { Id = 2, Name = "Two" },
            }
        });

        var rootGrid = control.Content.ShouldBeOfType<Grid>();
        rootGrid.Children.OfType<Label>().Count().ShouldBe(0);
        rootGrid.Children.OfType<ContentView>().Select(Grid.GetRow).Distinct().OrderBy(x => x).ToArray().ShouldBe([0, 2]);
    }

    [Fact]
    public void ShowHeaders_ShouldRerender_WhenUpdated()
    {
        var control = AnimationReadyHandler.Prepare(new DataGrid
        {
            Columns =
            [
                new DataGridColumn { Title = "Id", ValueBinding = new Binding(nameof(DataGridRow.Id)) },
                new DataGridColumn { Title = "Name", ValueBinding = new Binding(nameof(DataGridRow.Name)) },
            ],
            ItemsSource = new List<DataGridRow>
            {
                new() { Id = 1, Name = "One" },
            }
        });

        var rootGrid = control.Content.ShouldBeOfType<Grid>();
        rootGrid.Children.OfType<Label>().Count().ShouldBe(2);
        rootGrid.Children.OfType<ContentView>().Select(Grid.GetRow).Distinct().Single().ShouldBe(2);

        control.ShowHeaders = false;

        rootGrid = control.Content.ShouldBeOfType<Grid>();
        rootGrid.Children.OfType<Label>().Count().ShouldBe(0);
        rootGrid.Children.OfType<ContentView>().Select(Grid.GetRow).Distinct().Single().ShouldBe(0);
    }

    private sealed class DataGridRow
    {
        public int Id { get; init; }

        public string Name { get; init; }
    }

    internal class DataGridTestViewModel : UraniumBindableObject
    {
        private Color lineSeparatorColor;

        public Color LineSeparatorColor { get => lineSeparatorColor; set => SetProperty(ref lineSeparatorColor, value); }
    }
}
