# Creating a Theme

In a UraniumUI app, a theme is a set of .NET MAUI resource dictionaries that define your app's colors, base styles, style classes, and reusable visual patterns. You do not need to create another library to theme an app. Most apps can keep the Material controls and replace the parts that make the UI feel like your product.

This page is a handbook for app developers who want to create their own visual system on top of UraniumUI.

## Theme Scope

Choose the smallest scope that solves the problem.

| Goal | Recommended approach |
| --- | --- |
| Change app colors | Provide a colors dictionary through `material:StyleResource ColorsOverride`. |
| Change existing Material styles | Use `material:StyleResource.Overrides` for existing keyed styles. |
| Add app-specific visual variants | Add your own `StyleClass` styles after the theme resources are merged. |
| Style a group of child controls together | Use `CascadingStyle.Resources` and `CascadingStyle.StyleClass`. |
| Build reusable field or button controls | Compose UraniumUI primitives such as `InputField`, `StatefulContentView`, `ButtonView`, `Select`, and `EntryView`. |

Start with colors and styles in your app. Create custom controls only when a repeated pattern needs behavior, bindable properties, validation integration, or a cleaner API for page authors.

## Runtime Setup

Every app that uses UraniumUI controls should register the core handlers:

```csharp
builder
    .UseMauiApp<App>()
    .UseUraniumUI();
```

Apps that use the Material theme or Material controls should also register Material:

```csharp
builder
    .UseMauiApp<App>()
    .UseUraniumUI()
    .UseUraniumUIMaterial();
```

`UseUraniumUI()` registers core handlers and dependencies, including `StatefulContentView`, `Select`, `Dropdown`, InputKit handlers, and Plainer controls. `UseUraniumUIMaterial()` registers Material-specific handlers and configures `AutoFormView` to use Material editors.

## Resource Setup

Material resources are exposed through the Material XAML namespace:

```xml
xmlns:material="http://schemas.enisn-projects.io/dotnet/maui/uraniumui/material"
```

The common app setup is:

```xml
<Application xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:material="http://schemas.enisn-projects.io/dotnet/maui/uraniumui/material"
             x:Class="MyApp.App">
    <Application.Resources>
        <ResourceDictionary>
            <ResourceDictionary.MergedDictionaries>
                <ResourceDictionary x:Name="appColors" Source="Resources/Themes/AppColors.xaml" />
                <ResourceDictionary x:Name="appBaseStyles" Source="Resources/Themes/AppBaseStyles.xaml" />

                <material:StyleResource
                    ColorsOverride="{x:Reference appColors}"
                    BasedOn="{x:Reference appBaseStyles}">
                    <material:StyleResource.Overrides>
                        <ResourceDictionary Source="Resources/Themes/AppMaterialOverrides.xaml" />
                    </material:StyleResource.Overrides>
                </material:StyleResource>

                <ResourceDictionary Source="Resources/Themes/AppStyleClasses.xaml" />
            </ResourceDictionary.MergedDictionaries>
        </ResourceDictionary>
    </Application.Resources>
</Application>
```

Use this order deliberately:

1. `AppColors.xaml` provides color keys that `ColorsOverride` can copy into Material's color dictionary.
2. `AppBaseStyles.xaml` can provide keyed base styles that match Material style keys; Material styles inherit missing setters from those styles.
3. `AppMaterialOverrides.xaml` can override setters on existing Material keyed styles with matching keys.
4. `AppStyleClasses.xaml` can add new app-specific styles after Material resources exist.

`StyleResource.Overrides` changes existing theme styles. It is not a general merge point for new styles. Put new styles in a normal merged dictionary after `material:StyleResource`.

## Color Tokens

UraniumUI Material uses named color tokens and the dark-token suffix convention. The light token is named `Primary`; the dark token is named `PrimaryDark`. Styles bind both with `AppThemeBinding`:

```xml
<Setter Property="TextColor"
        Value="{AppThemeBinding Light={StaticResource OnSurface}, Dark={StaticResource OnSurfaceDark}}" />
```

Use these token groups when designing a palette:

| Tokens | Purpose |
| --- | --- |
| `Primary`, `OnPrimary`, `PrimaryContainer`, `OnPrimaryContainer` | Main brand actions and high-emphasis surfaces. |
| `Secondary`, `OnSecondary`, `SecondaryContainer`, `OnSecondaryContainer` | Supporting actions and lower-emphasis brand surfaces. |
| `Tertiary`, `OnTertiary`, `TertiaryContainer`, `OnTertiaryContainer` | Accent areas and alternate selection states. |
| `Surface`, `OnSurface`, `SurfaceVariant`, `OnSurfaceVariant` | Cards, sheets, dialogs, inputs, dividers, and text on surfaces. |
| `Background`, `OnBackground` | Page-level backgrounds and default text. |
| `Error`, `ErrorContainer`, `OnError`, `OnErrorContainer` | Validation and destructive/error states. |
| `Outline`, `OutlineVariant` | Borders, separators, and low-emphasis strokes. |
| `Shadow`, `Scrim`, `InverseSurface`, `InverseOnSurface`, `InversePrimary` | Elevation, overlays, and inverse surfaces. |

A minimal color override file should keep the same keys that Material expects:

```xml
<?xml version="1.0" encoding="utf-8" ?>
<ResourceDictionary xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
                    xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml">
    <Color x:Key="Primary">#2458D3</Color>
    <Color x:Key="PrimaryDark">#ADC6FF</Color>
    <Color x:Key="OnPrimary">#FFFFFF</Color>
    <Color x:Key="OnPrimaryDark">#082B73</Color>

    <Color x:Key="Surface">#FCFBFF</Color>
    <Color x:Key="SurfaceDark">#111318</Color>
    <Color x:Key="OnSurface">#1B1B21</Color>
    <Color x:Key="OnSurfaceDark">#E4E2EA</Color>

    <Color x:Key="Background">#F7F8FF</Color>
    <Color x:Key="BackgroundDark">#0C0E13</Color>
    <Color x:Key="OnBackground">#1B1B21</Color>
    <Color x:Key="OnBackgroundDark">#E4E2EA</Color>

    <Color x:Key="Error">#BA1A1A</Color>
    <Color x:Key="ErrorDark">#FFB4AB</Color>
    <Color x:Key="Outline">#74777F</Color>
    <Color x:Key="OutlineDark">#8E9099</Color>
</ResourceDictionary>
```

Keep contrast in mind when choosing every `On...` token. `OnPrimary` must be readable on `Primary`, `OnSurface` must be readable on `Surface`, and validation text must remain readable in both light and dark themes.

## Style Layers

App theme styles normally use three layers.

| Layer | Example | Usage |
| --- | --- | --- |
| Keyed base style | `x:Key="UraniumUI.Styles.Button.Filled"` | Stable extension point for app-level overrides. |
| Implicit style | `<Style TargetType="Label" />` | Default appearance for a type. |
| Class style | `Class="FilledButton"` | Variant selected by `StyleClass`. |

The Material theme defines keyed styles first, then maps built-in `StyleClass` names to those keyed styles:

```xml
<Style x:Key="UraniumUI.Styles.Button.Filled"
       TargetType="Button"
       BaseResourceKey="BaseButtonStyle"
       CanCascade="True">
    <Setter Property="BackgroundColor" Value="{AppThemeBinding Light={StaticResource Primary}, Dark={StaticResource PrimaryDark}}" />
    <Setter Property="TextColor" Value="{AppThemeBinding Light={StaticResource OnPrimary}, Dark={StaticResource OnPrimaryDark}}" />
</Style>

<Style BaseResourceKey="UraniumUI.Styles.Button.Filled"
       TargetType="Button"
       Class="FilledButton"
       CanCascade="True" />
```

That lets app code stay simple:

```xml
<Button Text="Save" StyleClass="FilledButton" />
```

It also gives your app a stable style key to override:

```xml
<Style x:Key="UraniumUI.Styles.Button.Filled" TargetType="Button">
    <Setter Property="CornerRadius" Value="14" />
    <Setter Property="HeightRequest" Value="44" />
</Style>
```

Use `BaseResourceKey` instead of copying large styles. This keeps future theme fixes and state definitions intact unless you intentionally replace them.

## Material Style Overrides

Use `material:StyleResource.Overrides` when you want to change existing Material keyed styles.

```xml
<ResourceDictionary xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
                    xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
                    xmlns:material="http://schemas.enisn-projects.io/dotnet/maui/uraniumui/material">

    <Style x:Key="UraniumUI.Styles.InputField"
           TargetType="material:InputField"
           ApplyToDerivedTypes="True">
        <Setter Property="CornerRadius" Value="12" />
        <Setter Property="BorderThickness" Value="1.5" />
        <Setter Property="InputBackgroundColor" Value="{AppThemeBinding Light={StaticResource Surface}, Dark={StaticResource SurfaceDark}}" />
    </Style>

    <Style x:Key="UraniumUI.Styles.TextField" TargetType="material:TextField">
        <Setter Property="SelectionHighlightColor" Value="{StaticResource Primary}" />
    </Style>
</ResourceDictionary>
```

Only setters are merged into the existing Material style. If the override contains a setter for a property that already exists, the value is replaced. If it contains a new setter, the setter is added.

## New Style Classes

Add new class styles in a normal dictionary after the theme is merged:

```xml
<ResourceDictionary xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
                    xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml">

    <Style TargetType="Button"
           Class="DangerButton"
           BaseResourceKey="UraniumUI.Styles.Button.Filled">
        <Setter Property="BackgroundColor" Value="{AppThemeBinding Light={StaticResource Error}, Dark={StaticResource ErrorDark}}" />
        <Setter Property="TextColor" Value="{AppThemeBinding Light={StaticResource OnError}, Dark={StaticResource OnErrorDark}}" />
    </Style>

    <Style TargetType="Border" Class="Card" BaseResourceKey="UraniumUI.Styles.Border.Rounded">
        <Setter Property="Padding" Value="16" />
        <Setter Property="BackgroundColor" Value="{AppThemeBinding Light={StaticResource Surface}, Dark={StaticResource SurfaceDark}}" />
        <Setter Property="Stroke" Value="{AppThemeBinding Light={StaticResource OutlineVariant}, Dark={StaticResource OutlineVariantDark}}" />
    </Style>
</ResourceDictionary>
```

Usage:

```xml
<Button Text="Delete" StyleClass="DangerButton" />

<Border StyleClass="Card">
    <Label Text="Account details" />
</Border>
```

Use class styles for variants that developers select explicitly. Use implicit styles only when every control of that type should change.

## Inherited Styles

Use `BaseResourceKey` to create variants from existing styles:

```xml
<Style TargetType="Button"
       Class="CompactFilledButton"
       BaseResourceKey="UraniumUI.Styles.Button.Filled">
    <Setter Property="HeightRequest" Value="34" />
    <Setter Property="Padding" Value="14,0" />
</Style>
```

Use `ApplyToDerivedTypes="True"` when a base control style should also apply to subclasses:

```xml
<Style x:Key="App.Styles.InputField"
       TargetType="material:InputField"
       ApplyToDerivedTypes="True">
    <Setter Property="CornerRadius" Value="10" />
</Style>
```

Use `CanCascade="True"` on styles that should participate in UraniumUI cascading style behavior.

## Cascading Styles

Cascading styles are useful when a container should set the appearance of its children without requiring every child to specify a `StyleClass`.

```xml
<ResourceDictionary xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
                    xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
                    xmlns:uranium="http://schemas.enisn-projects.io/dotnet/maui/uraniumui">

    <Style TargetType="Border" Class="PrimaryCard" BaseResourceKey="UraniumUI.Styles.Border.Rounded" CanCascade="True">
        <Setter Property="Padding" Value="20" />
        <Setter Property="BackgroundColor" Value="{AppThemeBinding Light={StaticResource PrimaryContainer}, Dark={StaticResource PrimaryContainerDark}}" />
        <Setter Property="uranium:CascadingStyle.Resources">
            <ResourceDictionary>
                <Style TargetType="Label">
                    <Setter Property="TextColor" Value="{AppThemeBinding Light={StaticResource OnPrimaryContainer}, Dark={StaticResource OnPrimaryContainerDark}}" />
                </Style>

                <Style TargetType="Button">
                    <Setter Property="uranium:CascadingStyle.StyleClass" Value="TextButton" />
                </Style>
            </ResourceDictionary>
        </Setter>
    </Style>
</ResourceDictionary>
```

Usage:

```xml
<Border StyleClass="PrimaryCard">
    <VerticalStackLayout Spacing="12">
        <Label Text="Subscription" />
        <Button Text="Manage" />
    </VerticalStackLayout>
</Border>
```

See [Cascading Styling](CascadingStyling.md) for the full cascading style API.

## App Control Primitives

App themes should prefer composition over platform-specific handlers. UraniumUI already provides primitives that solve common interaction and accessibility problems.

| Primitive | Use it for |
| --- | --- |
| `InputField` | A Material field shell with floating label, border, icon, attachments, focus state, and validation area. |
| `TextField` | Text input built from `InputField` plus a chrome-free `EntryView`. Use it directly or copy its pattern for custom text fields. |
| `StatefulContentView` | Clickable custom surfaces with pressed, pointer-over, focus, and command support. |
| `ButtonView` | Material clickable content when a normal MAUI `Button` is not flexible enough. |
| `Select` and `Dropdown` | Selection and overlay patterns that need keyboard-aware behavior. |
| `CascadingStyle` | Container-level child styling and style-class assignment. |

For example, a fully custom text input does not need a new handler just to draw a border. Wrap a chrome-free entry in a `Border`, or use `InputField` when you need the Material floating label and validation behavior. See [Building Theme Controls](BuildingThemeControls.md) for advanced examples.

## App Theme Folder Structure

Keep app theme files close to the MAUI resource files that developers already know. A practical structure is:

```text
Resources/
  Themes/
    AppColors.xaml
    AppBaseStyles.xaml
    AppMaterialOverrides.xaml
    AppStyleClasses.xaml
  Styles/
    Colors.xaml
    Styles.xaml
```

Use `Resources/Styles/Colors.xaml` and `Resources/Styles/Styles.xaml` for ordinary MAUI starter-template resources. Use `Resources/Themes` for UraniumUI-specific tokens, Material overrides, and style classes. This separation makes it easier to see which files are part of your UraniumUI visual system.

If your app only needs a small color change, you may only need `AppColors.xaml`. Add the other files when the app starts to need shared button variants, card styles, input-field changes, or custom control styles.

## Accessibility Contract

Accessibility is part of the app theme contract. Every interactive style and custom control should preserve these behaviors:

| Area | Requirement |
| --- | --- |
| Keyboard | Interactive controls are reachable by `Tab` and activate with the expected keys. |
| Focus | Focus state is visible in light and dark themes. |
| Pointer states | Hover, pressed, disabled, selected, and error states remain visually distinct. |
| Screen readers | Icon-only actions have `SemanticProperties.Description` and useful hints. |
| Validation | Error messages are visible as text and not communicated only by color or an icon. |
| Contrast | Text and icons have sufficient contrast against their container tokens. |

Use `StatefulContentView` or `ButtonView` for custom clickable surfaces instead of a passive layout with only `TapGestureRecognizer`. See [Accessibility Best Practices](../best-practices/Accessibility.md) and [Clickable Areas](../best-practices/ClickableAreas.md).

## Related Pages

- [Color System](ColorSystem.md)
- [Cascading Styling](CascadingStyling.md)
- [Building Theme Controls](BuildingThemeControls.md)
- [Material Colors & Styles](../themes/material/ColorsAndStyles.md)
- [InputField](../themes/material/components/InputField.md)
- [StatefulContentView](../infrastructure/StatefulContentView.md)
