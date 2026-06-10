# Chip
`Chip` is a compact material surface for showing a short value with an optional remove action.

## Usage

`Chip` is included in the `UraniumUI.Material.Controls` namespace.

```xml
xmlns:material="http://schemas.enisn-projects.io/dotnet/maui/uraniumui/material"
```

```xml
<material:Chip Text="Mango" />
```

## Properties

- `Text`: The text shown inside the chip.
- `TextColor`: The text color.
- `IsDestroyVisible`: Shows or hides the remove button. Default is `true`.
- `SelfDestruct`: Removes the chip from its parent after the remove action. Default is `true`.
- `DestroyCommand`: Runs when the remove button is tapped.

## Example

```xml
<HorizontalStackLayout Spacing="8">
    <material:Chip Text="Apple" />
    <material:Chip Text="Orange" IsDestroyVisible="False" />
</HorizontalStackLayout>
```
