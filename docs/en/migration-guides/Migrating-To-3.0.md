# Migration Guide to v3.0

Version 3.0 is the migration from UraniumUI v2.16 to the .NET 10 generation.

You can see the related v3 breaking-change PRs [from here](https://github.com/enisn/UraniumUI/pulls?q=is%3Apr+milestone%3Av3.0+label%3A%22breaking-change+%F0%9F%92%94%22) and all v3 milestone PRs [from here](https://github.com/enisn/UraniumUI/pulls?q=is%3Apr+milestone%3Av3.0).

## Before you update

- Upgrade your application to .NET 10 and .NET MAUI 10 first.
- Upgrade all UraniumUI packages together. Do not mix v2.16 and v3 packages in the same app.
- If your app must stay on .NET 9, stay on UraniumUI v2.16.0.
- Rebuild after each section below if you have custom controls, custom dialog services, or custom TreeView integrations.

## .NET 10 is required

UraniumUI v3 targets .NET 10 only. All .NET 9 target frameworks were removed from the core and add-on packages.

Update your application target frameworks from `net9.0` to `net10.0`.

**Before:**

```xml
<TargetFrameworks>net9.0;net9.0-android;net9.0-ios;net9.0-maccatalyst</TargetFrameworks>
<TargetFrameworks Condition="$([MSBuild]::IsOSPlatform('windows'))">$(TargetFrameworks);net9.0-windows10.0.19041.0</TargetFrameworks>
```

**After:**

```xml
<TargetFrameworks>net10.0;net10.0-android;net10.0-ios;net10.0-maccatalyst</TargetFrameworks>
<TargetFrameworks Condition="$([MSBuild]::IsOSPlatform('windows'))">$(TargetFrameworks);net10.0-windows10.0.19041.0</TargetFrameworks>
```

If your app pins transitive packages directly, review these package versions after the upgrade:

- `Microsoft.Maui.Controls` is based on MAUI `10.0.71` in the v3 branch.
- `InputKit.Maui` is updated to `4.6.0`.
- `Plainer.Maui` is updated to `1.8.0`.
- `CommunityToolkit.Maui` is updated to `13.0.0` in `UraniumUI.Dialogs.CommunityToolkit`.

## DatePickerField now uses nullable dates

`material:DatePickerField` now supports empty values. The date-related bindable properties changed from `DateTime` to `DateTime?`:

- `DatePickerField.Date`
- `DatePickerField.MinimumDate`
- `DatePickerField.MaximumDate`

The default `Date` value is now `null`, and clear/reset operations also set `Date` to `null`.

**Before:**

```csharp
public DateTime BirthDate { get; set; } = DateTime.Today;
```

**After:**

```csharp
public DateTime? BirthDate { get; set; }
```

If the field is required, make the bound property nullable and apply validation instead of relying on a default date value.

```csharp
[Required]
public DateTime? BirthDate { get; set; }
```

If existing code requires a non-null value, coalesce where you use it.

```csharp
var selectedDate = BirthDate ?? DateTime.Today;
```

## DatePickerField opens a date prompt dialog

`DatePickerField` no longer exposes a native picker as its main content. It renders the selected value through a label and opens `IDialogService.DisplayDatePromptAsync(...)` when tapped.

No change is required if your app uses `.UseUraniumUI()` and the built-in dialog service.

Check your code if you customized the old native picker behavior:

- If you replaced or inspected `DatePickerField.Content` as a `DatePicker`, update that code. `Content` is now a `Label` by default.
- `DatePickerField.DatePickerView` still exists for compatibility with date picker properties, but it is not the visible interactive control.
- Apps with a custom `IDialogService` must implement the new date prompt method described below.

## Custom IDialogService implementations must add the date prompt API

`IDialogService` has a new required method:

```csharp
Task<DateTime?> DisplayDatePromptAsync(
    string title,
    DateTime? selectedDate = null,
    DateTime? minimumDate = null,
    DateTime? maximumDate = null,
    string accept = "OK",
    string cancel = "Cancel",
    string clear = "Clear",
    string today = "Today");
```

If your app implements `IDialogService`, add this method or the app will not compile against v3.

The built-in behavior is:

- `accept` returns the selected date.
- `cancel` returns the original selected date.
- `clear` returns `null`.
- `today` selects `DateTime.Today` when it is inside the allowed date range.

`IDialogService` also has a cancellable custom view dialog overload:

```csharp
Task<bool> DisplayViewAsync(
    string title,
    View content,
    string okText,
    string cancelText);
```

This overload has a default interface implementation that throws `NotSupportedException`. The built-in dialog services implement it. Override it in custom services if your app uses cancellable custom view dialogs or controls that depend on this overload.

## Custom IDropdown implementations must add Close

`IDropdown` now includes a `Close()` method.

```csharp
public interface IDropdown : IView
{
    // Existing members...
    void Close();
}
```

If you implemented `IDropdown` directly, add `Close()` and close the open dropdown/popup from that method.

No change is required for the built-in `Dropdown` and `DropdownField` controls.

## TreeView was reworked for virtualization

`material:TreeView` now uses a flat, virtualized `CollectionView` internally instead of recursively creating node holder views.

This improves large tree performance, but it affects code that depended on TreeView internals.

Removed API:

```csharp
TreeViewNodeHolderView
TreeView.AllNodeViews
```

Use the public selection APIs instead of walking generated node views:

```csharp
treeView.SelectedItem
treeView.SelectedItems
treeView.SelectedItemChanged
treeView.SelectedItemsChanged
treeView.SelectedItemChangedCommand
treeView.SelectedItemsChangedCommand
```

Do not wrap large `TreeView` instances in a `ScrollView`. Let `TreeView` own scrolling so virtualization can work.

```xml
<!-- Before -->
<ScrollView>
    <material:TreeView ItemsSource="{Binding Nodes}" />
</ScrollView>

<!-- After -->
<material:TreeView ItemsSource="{Binding Nodes}" />
```

`ItemTemplate` still receives your original item as its binding context.

```xml
<material:TreeView.ItemTemplate>
    <DataTemplate>
        <Label Text="{Binding Name}" />
    </DataTemplate>
</material:TreeView.ItemTemplate>
```

`ExpanderTemplate` now receives a `TreeViewNode`. Existing bindings such as `IsExpanded` and `IsLeaf` still work. If the expander template needs the original item, bind through `Item`.

```xml
<material:TreeView.ExpanderTemplate>
    <DataTemplate>
        <Label Text="{Binding Item.Name}" />
    </DataTemplate>
</material:TreeView.ExpanderTemplate>
```

If you subclassed `TreeViewHierarchicalSelectBehavior`, update overrides that used `TreeViewNodeHolderView`.

**Before:**

```csharp
protected override void CheckStateItself(TreeViewNodeHolderView holder, bool forcedSemiSelected = false)
```

**After:**

```csharp
protected override void CheckStateItself(TreeViewNodeView row, bool forcedSemiSelected = false)
```

## FormView has async validation support

v3 adds `UraniumUI.Controls.FormView`, which extends the InputKit `FormView` with async validation APIs:

- `SubmitAsync(...)`
- `ValidateFormAsync(...)`
- `IFormValidator`
- `FormValidationHandler`
- `ValidationModel`
- `ShowValidationSummary`
- `IsBusy` and `IsValidating`
- attached `FormView.ValidationPath`
- attached `FormView.IsBusyIndicator`

`AutoFormView` and built-in form dialogs now use this validation flow. Dialog form submissions no longer close when validation fails.

If you have custom form dialog code, follow the v3 pattern:

```csharp
if (await formView.SubmitAsync())
{
    // Close the dialog and return the submitted model.
}
```

If C# code imports both `InputKit.Shared.Controls` and `UraniumUI.Controls`, unqualified `FormView` references may become ambiguous. Fully qualify the type or remove the unused namespace.

```csharp
var formView = new UraniumUI.Controls.FormView();
```

## Manual handler registration

No startup method was renamed. `UseUraniumUI()`, `UseUraniumUIMaterial()`, `UseUraniumUIBlurs()`, and `UseUraniumUIWebComponents()` are unchanged.

If you use `.UseUraniumUI()`, no action is required for handlers.

If your app copied UraniumUI handler registrations manually, add the `Select` handler:

```csharp
handlers.AddHandler(typeof(UraniumUI.Controls.Select), typeof(StatefulContentViewHandler));
```

## XAML namespaces are unchanged

The existing XAML namespace URIs are unchanged:

```xml
xmlns:uranium="http://schemas.enisn-projects.io/dotnet/maui/uraniumui"
xmlns:material="http://schemas.enisn-projects.io/dotnet/maui/uraniumui/material"
```

v3 adds new XAML-visible controls under these existing namespaces, including `uranium:CalendarView`, `uranium:Select`, `uranium:FormView`, and `material:SelectField`.

## Migration checklist

- Update the app to .NET 10 and .NET MAUI 10.
- Update all UraniumUI packages to v3 together.
- Change `DatePickerField` view model properties from `DateTime` to `DateTime?` where they bind to `Date`, `MinimumDate`, or `MaximumDate`.
- Implement `IDialogService.DisplayDatePromptAsync(...)` in custom dialog services.
- Implement `IDropdown.Close()` in custom dropdown controls.
- Remove usages of `TreeViewNodeHolderView` and `TreeView.AllNodeViews`.
- Remove external `ScrollView` wrappers around large `TreeView` controls.
- Update custom form dialog code to await `SubmitAsync()` before closing.
- Add the `Select` handler if your app registers UraniumUI handlers manually.
