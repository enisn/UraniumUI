# Paginator

The paginator component is used to navigate through a list of paged items. It's not a standalone component, but a directive that can be used with other components such as DataGrid, ListView, CollectionView, etc.

## Usage

Paginator is included in the `UraniumUI.Material.Controls` namespace. To use it, add the following namespace to your XAML file:

```xml
xmlns:material="http://schemas.enisn-projects.io/dotnet/maui/uraniumui/material"
```

Then, you can use the paginator in your XAML file like this:

```xml
<material:Paginator 
        ChangePageCommand="{Binding SetPageCommand}"
        CurrentPage="{Binding CurrentPage}"
        TotalPageCount="{Binding TotalPages}"
        HorizontalOptions="Center"/>
```

![Paginator](../../../../images/paginator-preview.png)


## Properties

| Property | Type | Description |
|----------|------|-------------|
| ChangePageCommand | ICommand | The command that will be executed when the page is changed. |
| CurrentPage | int | The current page number. |
| TotalPageCount | int | The total number of pages. |
| PageStepCount | int | The number of the step pages to show in the paginator. Default is 2. _(2 for previous, 2 for next)_ |

