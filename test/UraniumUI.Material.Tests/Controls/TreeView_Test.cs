using Shouldly;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UraniumUI.Material.Controls;
using UraniumUI.Tests.Core;

namespace UraniumUI.Material.Tests.Controls;
public class TreeView_Test
{
    public TreeView_Test()
    {
        ApplicationExtensions.CreateAndSetMockApplication();
    }

    [Fact]
    public void ItemsSource_ShouldBeSet_FromViewModel()
    {
        var itemSource = new[] { "1", "2", "3", "4" };
        var viewModel = new TestViewModel { ItemSource = itemSource };

        var control = new TreeView();
        control.BindingContext = viewModel;
        // Act
        control.SetBinding(TreeView.ItemsSourceProperty, new Binding(nameof(TestViewModel.ItemSource)));

        // Assert
        control.ItemsSource.ShouldBe(itemSource);
    }

    public class TestViewModel : UraniumBindableObject
    {
        private IList itemSource;

        public IList ItemSource { get => itemSource; set => SetProperty(ref itemSource, value); }
    }
}
