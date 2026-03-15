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

    [Fact]
    public void Node_ShouldBeUnregistered_WhenHandlerIsReleased()
    {
        var tree = AnimationReadyHandler.Prepare(new TreeView());
        var node = AnimationReadyHandler.Prepare(new TreeViewNodeHolderView(TreeView.DefaultItemTemplate, tree, new Binding("Children")));

        tree.AllNodeViews.ShouldContain(node);

        node.Handler = null;

        tree.AllNodeViews.ShouldNotContain(node);
    }

    [Fact]
    public void ReleasingNode_ShouldDetachChildItemsSource()
    {
        var tree = AnimationReadyHandler.Prepare(new TreeView());
        var node = AnimationReadyHandler.Prepare(new TreeViewNodeHolderView(TreeView.DefaultItemTemplate, tree, new Binding(nameof(TestTreeNode.Children))));

        node.BindingContext = new TestTreeNode
        {
            Children = new[] { new TestTreeNode() }
        };

        node.NodeChildren.ShouldNotBeNull();
        node.NodeChildren.ItemsSource.ShouldNotBeNull();

        node.Handler = null;

        node.NodeChildren.ItemsSource.ShouldBeNull();
    }

    public class TestViewModel : UraniumBindableObject
    {
        private IList itemSource;
        private object selectedItem;

        public IList ItemSource { get => itemSource; set => SetProperty(ref itemSource, value); }

        public object SelectedItem { get => selectedItem; set => SetProperty(ref selectedItem, value); }
    }

    public class TestTreeNode
    {
        public IEnumerable<TestTreeNode> Children { get; set; }
    }
}
