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

        var control = AnimationReadyHandler.Prepare(new TreeView()); ;
        control.BindingContext = viewModel;
        // Act
        control.SetBinding(TreeView.ItemsSourceProperty, new Binding(nameof(TestViewModel.ItemSource)));

        // Assert
        control.ItemsSource.ShouldBe(itemSource);
    }

    [Fact]
    public void SelectedItem_ShouldBeSet_FromViewModel()
    {
        var itemSource = new[] { "1", "2", "3", "4" };
        var viewModel = new TestViewModel { ItemSource = itemSource };

        var control = AnimationReadyHandler.Prepare(new TreeView());
        control.BindingContext = viewModel;
        control.SetBinding(TreeView.ItemsSourceProperty, new Binding(nameof(TestViewModel.ItemSource)));
        control.SetBinding(TreeView.SelectedItemProperty, new Binding(nameof(TestViewModel.SelectedItem)));

        // Act
        viewModel.SelectedItem = itemSource[0];

        // Assert
        control.SelectedItem.ShouldBe(itemSource[0]);
    }

    [Fact]
    public void SelectedItem_ShouldBeSet_FromControl()
    {
        var itemSource = new[] { "1", "2", "3", "4" };
        var viewModel = new TestViewModel { ItemSource = itemSource };

        var control = AnimationReadyHandler.Prepare(new TreeView()); ;
        control.BindingContext = viewModel;
        control.SetBinding(TreeView.ItemsSourceProperty, new Binding(nameof(TestViewModel.ItemSource)));
        control.SetBinding(TreeView.SelectedItemProperty, new Binding(nameof(TestViewModel.SelectedItem)));

        // Act
        control.SelectedItem = itemSource[0];

        // Assert
        viewModel.SelectedItem.ShouldBe(itemSource[0]);
    }

    public class TestViewModel : UraniumBindableObject
    {
        private IList itemSource;
        private object selectedItem;

        public IList ItemSource { get => itemSource; set => SetProperty(ref itemSource, value); }

        public object SelectedItem { get => selectedItem; set => SetProperty(ref selectedItem, value); }
    }
}
