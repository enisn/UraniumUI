using UraniumUI.Extensions;
using UraniumUI.Material.Controls;
using CheckBox = UraniumUI.Material.Controls.CheckBox;

namespace UraniumApp;

public class TreeViewChooseAllBehavior : Behavior<CheckBox>
{
    public static object lockingObj = new object();
    protected override void OnAttachedTo(CheckBox bindable)
    {
        base.OnAttachedTo(bindable);

        bindable.CheckChanged += Bindable_CheckChanged;
    }

    protected override void OnDetachingFrom(CheckBox bindable)
    {
        base.OnDetachingFrom(bindable);

        bindable.CheckChanged -= Bindable_CheckChanged;
    }

    private void Bindable_CheckChanged(object sender, EventArgs e)
    {
        lock (lockingObj)
        {
            var checkBox = sender as CheckBox;

            var holder = checkBox.FindInParents<TreeViewNodeHolderView>();
            if (holder == null)
            {
                throw new InvalidOperationException("CheckBox isn't in a TreeView ItemTemplate");
            }

            foreach (TreeViewNodeHolderView child in holder.NodeChildrens.Where(x => x is TreeViewNodeHolderView))
            {
                var childCheckBox = (child.NodeView as CheckBox);
                if (childCheckBox.IsChecked != checkBox.IsChecked)
                {
                    childCheckBox.IsChecked = checkBox.IsChecked;
                }
            }
        }
    }
}

public class TreeViewSemiSelectParentBehavior : Behavior<CheckBox>
{
    protected override void OnAttachedTo(CheckBox bindable)
    {
        base.OnAttachedTo(bindable);

        bindable.CheckChanged += Bindable_CheckChanged;
    }

    protected override void OnDetachingFrom(CheckBox bindable)
    {
        base.OnDetachingFrom(bindable);

        bindable.CheckChanged -= Bindable_CheckChanged;
    }

    private void Bindable_CheckChanged(object sender, EventArgs e)
    {
        var checkBox = sender as CheckBox;

        var holder = checkBox.FindInParents<TreeViewNodeHolderView>();

        CheckItSelf(holder);
    }

    private void CheckItSelf(TreeViewNodeHolderView holder, bool forcedSemiSelected = false)
    {
        lock (TreeViewChooseAllBehavior.lockingObj)
        {
            if (holder == null || holder.Parent is null)
            {
                return;
            }

            var mainCheckBox = holder.NodeView as CheckBox;

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

            if (holder.NodeChildrens.Count > 0)
            {
                var children = holder.NodeChildrens.OfType<TreeViewNodeHolderView>();

                var lastItemToCheck = (children.FirstOrDefault().NodeView as CheckBox)?.IsChecked ?? throw new InvalidOperationException("CheckBox isn't in a TreeView ItemTemplate");
                foreach (TreeViewNodeHolderView child in holder.NodeChildrens.Where(x => x is TreeViewNodeHolderView))
                {
                    var checkBox = (child.NodeView as CheckBox);
                    if (lastItemToCheck != checkBox.IsChecked)
                    {
                        mainCheckBox.IconGeometry = InputKit.Shared.Controls.PredefinedShapes.Line;
                        if (!mainCheckBox.IsChecked)
                        {
                            mainCheckBox.IsChecked = true;
                        }
                        CheckItSelf(holder.ParentHolderView, true);
                        return;
                    }
                }
                if (mainCheckBox.IsChecked != lastItemToCheck)
                {
                    mainCheckBox.IsChecked = lastItemToCheck;
                }
            }
        }
        CheckItSelf(holder.ParentHolderView);
    }
}
