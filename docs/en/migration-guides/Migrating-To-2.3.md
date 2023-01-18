# Migration Guide to v2.3
Version 2.3 comes with some changes. You should follow this docuemnt to migrate your code to the new version properly.

## Changes
UraniumUI has a couple of changes in this version. Applying following changes to your code will make it compatible with UraniumUI v2.3.

Following changes are applied to the following controls:

### Dialogs
Dialogs Implementation has been changed. Dialogs implementation is now separated from the core UraniumUI assembly to 2 new assemblies. You should choose one of `UraniumUI.Dialogs.CommunityToolkit` or `UraniumUI.Dialogs.Mopups` implementations in your project to show dialogs. 

- You should use `IDialogService` to show dialogs via injecting it after registering it in your DI container for newer versions of UraniumUI (v2.3+).

Follow [the dialogs documentation](../dialogs/Index.md) to learn more about installation and usage.


### AutoCompleteView
AutoCompleteView has been moved to `UraniumUI.Controls` namesppace in `UraniumUI` assembly from `UraniumUI.Material` assembly.
