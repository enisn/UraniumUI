using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

    internal class DataGridTestViewModel : UraniumBindableObject
    {
        private Color lineSeparatorColor;

        public Color LineSeparatorColor { get => lineSeparatorColor; set => SetProperty(ref lineSeparatorColor, value); }
    }
}
