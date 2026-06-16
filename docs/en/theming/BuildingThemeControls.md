# Building Theme Controls

This page is the advanced companion to [Creating a Theme](CreatingTheme.md). It explains how to build app-specific controls by composing UraniumUI primitives instead of starting with platform handlers.

Most app controls should be MAUI controls, UraniumUI primitives, and styles. Add a platform handler only when composition cannot provide the required native behavior.

## Primitive Selection

| Requirement | Start with |
| --- | --- |
| Text input with a border, floating title, icon, validation, or attachments | `InputField` |
| Plain text input with fully custom chrome | `EntryView` from `Plainer.Maui.Controls` inside a `Border` or layout |
| Custom clickable card, list row, tile, or icon action | `StatefulContentView` |
| Material-style clickable content | `ButtonView` |
| Dropdown or selection overlay | `Dropdown`, `Select`, or the Material field controls built on them |
| Container that changes child text or button styles | `CascadingStyle.Resources` and `CascadingStyle.StyleClass` |
| Dynamic field generation | `AutoFormView` with editor mappings |

`UseUraniumUI()` registers the core handlers used by these primitives. `UseUraniumUIMaterial()` registers Material-specific handlers and Material `AutoFormView` mappings.

## Plain Text Input With Custom Chrome

UraniumUI Material `TextField` uses `InputField` as the shell and `EntryView` as the inner text entry. `EntryView` comes from `Plainer.Maui.Controls`; UraniumUI registers Plainer in `UseUraniumUI()`.

If your design does not need floating labels or `InputField` validation, a border around `EntryView` is enough:

```xml
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:plainer="clr-namespace:Plainer.Maui.Controls;assembly=Plainer.Maui">

    <Border StyleClass="SearchBox" Padding="12,8">
        <Grid ColumnDefinitions="Auto,*" ColumnSpacing="8">
            <Label Text="#" VerticalOptions="Center" />
            <plainer:EntryView
                Grid.Column="1"
                BackgroundColor="Transparent"
                Placeholder="Search"
                SemanticProperties.Description="Search" />
        </Grid>
    </Border>
</ContentPage>
```

Add the style as a normal class style:

```xml
<Style TargetType="Border" Class="SearchBox" BaseResourceKey="UraniumUI.Styles.Border.Rounded">
    <Setter Property="BackgroundColor" Value="{AppThemeBinding Light={StaticResource Surface}, Dark={StaticResource SurfaceDark}}" />
    <Setter Property="Stroke" Value="{AppThemeBinding Light={StaticResource Outline}, Dark={StaticResource OutlineDark}}" />
    <Setter Property="StrokeThickness" Value="1" />
</Style>
```

This approach is useful for simple search bars, filter chips with text entry, and compact inputs where Material floating-label behavior would be too heavy.

## InputField as a Wrapper

Use `InputField` when the inner control should look and behave like a Material input.

```xml
<material:InputField
    xmlns:material="http://schemas.enisn-projects.io/dotnet/maui/uraniumui/material"
    Title="Start time"
    HasValue="True"
    ContentAutomationId="StartTimeInput">
    <TimePicker
        BackgroundColor="Transparent"
        SemanticProperties.Description="Start time" />
</material:InputField>
```

`InputField` owns the shell. The wrapped control owns the actual value and input behavior. Set `HasValue` correctly so the floating title stays above the content when the control has a value.

Use this wrapper approach when you do not need a reusable control class.

## Reusable Text Field Control

Create a subclass when your app needs a reusable control API.

```csharp
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using Plainer.Maui.Controls;
using UraniumUI.Material.Controls;

namespace MyApp.Controls;

public class AppTextField : InputField
{
    private readonly EntryView entryView = new()
    {
        BackgroundColor = Colors.Transparent,
        Margin = new Thickness(16, 0),
        VerticalOptions = LayoutOptions.Center,
    };

    public AppTextField()
    {
        Content = entryView;

        entryView.SetBinding(Entry.TextProperty, new Binding(nameof(Text), BindingMode.TwoWay, source: this));
        entryView.SetBinding(Entry.KeyboardProperty, new Binding(nameof(Keyboard), source: this));
        entryView.SetBinding(Entry.IsReadOnlyProperty, new Binding(nameof(IsReadOnly), source: this));
    }

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public static readonly BindableProperty TextProperty = BindableProperty.Create(
        nameof(Text),
        typeof(string),
        typeof(AppTextField),
        string.Empty,
        BindingMode.TwoWay,
        propertyChanged: (bindable, _, _) => ((AppTextField)bindable).UpdateState());

    public Keyboard Keyboard
    {
        get => (Keyboard)GetValue(KeyboardProperty);
        set => SetValue(KeyboardProperty, value);
    }

    public static readonly BindableProperty KeyboardProperty = BindableProperty.Create(
        nameof(Keyboard),
        typeof(Keyboard),
        typeof(AppTextField),
        Keyboard.Default);

    public bool IsReadOnly
    {
        get => (bool)GetValue(IsReadOnlyProperty);
        set => SetValue(IsReadOnlyProperty, value);
    }

    public static readonly BindableProperty IsReadOnlyProperty = BindableProperty.Create(
        nameof(IsReadOnly),
        typeof(bool),
        typeof(AppTextField),
        false);

    public override bool HasValue
    {
        get => !string.IsNullOrEmpty(Text);
        set { }
    }

    protected override object GetValueForValidator()
    {
        return Text;
    }
}
```

This is intentionally smaller than `UraniumUI.Material.Controls.TextField`. For a production text field, inspect Material `TextField` and add only the API you need, such as `TextChanged`, `Completed`, `AllowClear`, selection properties, password behavior, and command support.

## InputField Styling Surface

`InputField` exposes style classes for its parts:

| Style class | Target type | Purpose |
| --- | --- | --- |
| `InputField.Title` | `Label` | Floating title text. |
| `InputField.Border` | `Border` | Field stroke, shape, and background container. |
| `InputField.Icon` | `Image` | Leading icon. |
| `InputField.Attachments` | `HorizontalStackLayout` | Trailing attachment container. |
| `InputField.ValidationIcon` | `Path` | Validation icon. |
| `InputField.ValidationLabel` | `Label` | Validation message text. |

Example:

```xml
<Style TargetType="Label" Class="InputField.Title">
    <Setter Property="FontAttributes" Value="Bold" />
</Style>

<Style TargetType="Border" Class="InputField.Border">
    <Setter Property="StrokeThickness" Value="1.5" />
</Style>

<Style TargetType="Label" Class="InputField.ValidationLabel">
    <Setter Property="FontSize" Value="12" />
    <Setter Property="TextColor" Value="{AppThemeBinding Light={StaticResource Error}, Dark={StaticResource ErrorDark}}" />
</Style>
```

Use these part styles when the shell should change globally. Use bindable properties such as `AccentColor`, `BorderColor`, `InputBackgroundColor`, `CornerRadius`, and `TitleFontSize` when a single control instance should differ.

## Attachments and Icon Actions

Attachments are views placed at the trailing side of `InputField` and `TextField`. Use them for actions such as clear, show password, scan code, or open picker.

Use `StatefulContentView` for icon-only actions so the attachment can be focusable and command-driven:

```xml
<material:TextField
    xmlns:material="http://schemas.enisn-projects.io/dotnet/maui/uraniumui/material"
    Title="Password"
    IsPassword="True">
    <material:TextField.Attachments>
        <uranium:StatefulContentView
            xmlns:uranium="http://schemas.enisn-projects.io/dotnet/maui/uraniumui"
            TappedCommand="{Binding TogglePasswordCommand}"
            SemanticProperties.Description="Show or hide password">
            <Path StyleClass="PasswordVisibilityIcon" />
        </uranium:StatefulContentView>
    </material:TextField.Attachments>
</material:TextField>
```

If the attachment is decorative, make it non-focusable and do not expose it as an action. If it changes state, update the visible icon and the semantic description.

## Clickable Surfaces

Use `StatefulContentView` for custom cards, rows, and tiles that respond to taps or keyboard activation.

```xml
<uranium:StatefulContentView
    xmlns:uranium="http://schemas.enisn-projects.io/dotnet/maui/uraniumui"
    StyleClass="AccountRow"
    TappedCommand="{Binding OpenAccountCommand}"
    SemanticProperties.Description="Open account details">
    <Grid Padding="16" ColumnDefinitions="*,Auto">
        <Label Text="Checking account" />
        <Label Grid.Column="1" Text=">" />
    </Grid>
</uranium:StatefulContentView>
```

Style the states through `VisualStateManager`:

```xml
<ResourceDictionary xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
                    xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
                    xmlns:uranium="http://schemas.enisn-projects.io/dotnet/maui/uraniumui">
    <Style TargetType="uranium:StatefulContentView" Class="AccountRow">
        <Setter Property="VisualStateManager.VisualStateGroups">
            <VisualStateGroupList>
                <VisualStateGroup x:Name="CommonStates">
                    <VisualState x:Name="Normal">
                        <VisualState.Setters>
                            <Setter Property="Opacity" Value="1" />
                        </VisualState.Setters>
                    </VisualState>
                    <VisualState x:Name="PointerOver">
                        <VisualState.Setters>
                            <Setter Property="Opacity" Value="0.9" />
                        </VisualState.Setters>
                    </VisualState>
                    <VisualState x:Name="Pressed">
                        <VisualState.Setters>
                            <Setter Property="Opacity" Value="0.75" />
                        </VisualState.Setters>
                    </VisualState>
                </VisualStateGroup>
            </VisualStateGroupList>
        </Setter>
    </Style>
</ResourceDictionary>
```

Prefer this over a `TapGestureRecognizer` on a `Grid` when the area is important to the workflow.

## Container-Level Styling

Use `CascadingStyle` when the parent style should control child labels, buttons, paths, or nested controls.

```xml
<ResourceDictionary xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
                    xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
                    xmlns:uranium="http://schemas.enisn-projects.io/dotnet/maui/uraniumui">
    <Style TargetType="Border" Class="WarningPanel" CanCascade="True">
        <Setter Property="BackgroundColor" Value="{AppThemeBinding Light={StaticResource ErrorContainer}, Dark={StaticResource ErrorContainerDark}}" />
        <Setter Property="uranium:CascadingStyle.Resources">
            <ResourceDictionary>
                <Style TargetType="Label">
                    <Setter Property="TextColor" Value="{AppThemeBinding Light={StaticResource OnErrorContainer}, Dark={StaticResource OnErrorContainerDark}}" />
                </Style>
                <Style TargetType="Button">
                    <Setter Property="uranium:CascadingStyle.StyleClass" Value="TextButton" />
                </Style>
            </ResourceDictionary>
        </Setter>
    </Style>
</ResourceDictionary>
```

This keeps page markup simple and makes the container responsible for its own local design rules.

## AutoFormView Integration

If your app provides its own input controls, configure `AutoFormView` mappings so generated forms use them.

Material does this in `ConfigureAutoFormViewForMaterial()`. An app can follow the same pattern in its own startup extension:

```csharp
public static MauiAppBuilder ConfigureAutoFormViewForAppTheme(this MauiAppBuilder builder)
{
    builder.Services.Configure<AutoFormViewOptions>(options =>
    {
        options.EditorMapping[typeof(string)] = (property, propertyNameFactory, source) =>
        {
            var editor = new AppTextField
            {
                Title = propertyNameFactory(property),
            };

            editor.SetBinding(AppTextField.TextProperty, new Binding(property.Name, source: source));

            return editor;
        };
    });

    return builder;
}
```

Keep mappings in app startup or an app-level startup extension so form generation remains predictable.

## Accessibility Checklist

Custom app controls must preserve accessibility behavior from the primitives they use.

1. Use `Title`, visible `Label` text, or `SemanticProperties.Description` for every input.
2. Use `ContentAutomationId` when tests need to locate the inner input of an `InputField`.
3. Keep icon-only actions focusable when they perform work.
4. Provide `SemanticProperties.Description` and `SemanticProperties.Hint` for icon-only actions.
5. Keep focus, pressed, hover, disabled, selected, and error states visually distinct.
6. Do not rely only on color to communicate validation or selection.
7. Test hardware keyboard navigation for text inputs, attachments, clickable rows, dropdowns, and dialogs.

See [Accessibility Best Practices](../best-practices/Accessibility.md) for the app-level checklist.

## Design Guidelines

- Use composition first. Add a handler only for native behavior that cannot be composed.
- Keep reusable controls small. Expose bindable properties only for behavior the app needs.
- Put visuals in styles. Put behavior and value APIs in controls.
- Use existing UraniumUI style class names for common concepts and app-specific names for app-only variants.
- Keep validation text close to the field.
- Document shared style classes and custom control properties that the app team should use consistently.
