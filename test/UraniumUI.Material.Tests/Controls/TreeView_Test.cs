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
    public void TreeViewNodeHolderView_ShouldUseGridContainer()
    {
        var item = new TestTreeItem
        {
            Children = Enumerable.Range(1, 200)
                .Select(i => new TestTreeItem { Name = $"Child {i}" })
                .ToList()
        };

        var treeView = new TreeView
        {
            UseAnimation = false,
            ItemsSource = new[] { item }
        };
        var holder = new TreeViewNodeHolderView(TreeView.DefaultItemTemplate, treeView, new Binding(nameof(TestTreeItem.Children)));

        AnimationReadyHandler.Prepare(treeView, out var handler);
        holder.Handler = handler;
        holder.BindingContext = item;

        var grid = holder.ShouldBeOfType<Grid>();

        // Ensure the layout has two rows: one for header/button (row 0) and one for children container (row 1).
        grid.RowDefinitions.Count.ShouldBe(2);

        // There should be exactly two direct children: header and children container.
        grid.Children.Count.ShouldBe(2);

        // Validate that one child is placed in row 0 and one in row 1.
        var row0Children = grid.Children
            .Where(child => Grid.GetRow((BindableObject)child) == 0)
            .ToList();
        var row1Children = grid.Children
            .Where(child => Grid.GetRow((BindableObject)child) == 1)
            .ToList();

        row0Children.Count.ShouldBe(1);
        row1Children.Count.ShouldBe(1);
    }

    public class TestViewModel : UraniumBindableObject
    {
        private IList itemSource;
        private object selectedItem;

        public IList ItemSource { get => itemSource; set => SetProperty(ref itemSource, value); }

        public object SelectedItem { get => selectedItem; set => SetProperty(ref selectedItem, value); }
    }

    public class TestTreeItem
    {
        public string Name { get; set; }

        public IList<TestTreeItem> Children { get; set; } = new List<TestTreeItem>();
    }
}
