# Migration Guide to v2.2
Version 2.1 comes with some changes. You should follow this docuemnt to migrate your code to the new version properly.

## Changes
UraniumUI has a couple of changes in this version. Applying following changes to your code will make it compatible with UraniumUI v2.2.

Following changes are applied to the following controls:

### DataGrid
DataGrid binding logic has been changed. You should follow the following steps to migrate your code to the new version.
- `PropertyNames` property has been removed. You should use `Binding` property instead.

```xml
<material:DataGrid>
  <material:DataGrid.Columns>

    <!--OLD-->
    <material:DataGridColumn PropertyName="Title" />
    
    <!--NEW-->
    <material:DataGridColumn Binding="{Binding Title}" />

  </material:DataGrid.Columns>
</material:DataGrid>

```

### TreeView
TreeView expander icon can be changed now. You can use `ExpanderTemplate` property to change the view for expander.

Default value is still arrow icon but this change may occur some spacing issues. Check your existing views and make sure they are working properly. You can arrange spacing between items by using `Spacing` property of TreeView.

```xml
<TreeView Binding="{Binding Nodes}" Spacing="20" />