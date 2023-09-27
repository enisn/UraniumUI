using UraniumUI.Extensions;

namespace UraniumUI.Material.Controls;
public class CustomTreeViewHierarchicalSelectBehavior : Behavior<CheckBox>
{
    protected override void OnAttachedTo(CheckBox bindable)
    {
        base.OnAttachedTo(bindable);
        bindable.CheckChanged += CheckBox_CheckChanged;
    }
    protected override void OnDetachingFrom(CheckBox bindable)
    {
        base.OnDetachingFrom(bindable);

        bindable.CheckChanged -= CheckBox_CheckChanged;
    }

    private void CheckBox_CheckChanged(object sender, EventArgs e)
    {
        var checkBox = sender as CheckBox;

        var holder = checkBox.FindInParents<TreeViewNodeHolderView>();

        lock (holder)
        {
            if (holder.TreeView.IsBusy)
            {
                return;
            }

            holder.TreeView.IsBusy = true;

            ApplyHierarchicalSelection(checkBox);

            CheckStateItself(holder);

            holder.TreeView.IsBusy = false;
        }
    }

    protected virtual void ApplyHierarchicalSelection(CheckBox checkBox)
    {
        var holder = checkBox.FindInParents<TreeViewNodeHolderView>() ?? throw new InvalidOperationException("CheckBox isn't in a TreeView ItemTemplate");
        foreach (TreeViewNodeHolderView child in holder.NodeChildren.Where(x => x is TreeViewNodeHolderView))
        {
            var childCheckBox = FindCheckBox(child);
            if (childCheckBox.IsChecked != checkBox.IsChecked)
            {
                childCheckBox.IsChecked = checkBox.IsChecked;
                ApplyHierarchicalSelection(childCheckBox);
            }
        }
    }

    protected virtual void CheckStateItself(TreeViewNodeHolderView holder, bool forcedSemiSelected = false)
    {
        if (holder == null || holder.Parent is null)
        {
            return;
        }

        var mainCheckBox = FindCheckBox(holder);

        if (forcedSemiSelected)
        {
            mainCheckBox.IconGeometry = InputKit.Shared.Controls.PredefinedShapes.Line;
            if (!mainCheckBox.IsChecked)
            {
                mainCheckBox.IsChecked = true;
            }
            return;
        }
        else
        {
            mainCheckBox.IconGeometry = InputKit.Shared.Controls.PredefinedShapes.Check;
        }

        if (holder.NodeChildren.Count > 0)
        {
            var children = holder.NodeChildren.OfType<TreeViewNodeHolderView>();

            var lastItemToCheck = FindCheckBox(children.FirstOrDefault())?.IsChecked ?? throw new InvalidOperationException("CheckBox isn't in a TreeView ItemTemplate");
            foreach (TreeViewNodeHolderView child in holder.NodeChildren.Where(x => x is TreeViewNodeHolderView))
            {
                var checkBox = FindCheckBox(child);
                if (lastItemToCheck != checkBox.IsChecked)
                {
                    mainCheckBox.IconGeometry = InputKit.Shared.Controls.PredefinedShapes.Line;
                    if (!mainCheckBox.IsChecked)
                    {
                        mainCheckBox.IsChecked = true;
                    }
                    CheckStateItself(holder.ParentHolderView, true);
                    return;
                }
            }
            if (mainCheckBox.IsChecked != lastItemToCheck)
            {
                mainCheckBox.IsChecked = lastItemToCheck;
            }
        }
        CheckStateItself(holder.ParentHolderView);
    }

    private static CheckBox FindCheckBox(TreeViewNodeHolderView child)
    {
        if (child.NodeView is Layout layout)
        {
            return layout.FindInChildrenHierarchy<CheckBox>();
        }

        return (child.NodeView as CheckBox);
    }
}

