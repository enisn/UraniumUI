# Migration Guide to v1.1
Version 1.1 comes with some changes. You should follow this docuemnt to migrate your code to the new version properly.

## Changes

### Disabled States
Each input has a disabled state now. It's working well after updating to v1.1. But you should add following style to your resources if you overrided the default style.

- Add following section in your StyleOverride file if you are using StyleOverride.
```xml
<Style TargetType="c:InputField" ApplyToDerivedTypes="True">
    <Setter Property="VisualStateManager.VisualStateGroups">
        <VisualStateGroupList>
            <VisualStateGroup x:Name="CommonStates">
                <VisualState x:Name="Normal">
                    <VisualState.Setters>
                        <Setter Property="Opacity" Value="1"/>
                        <Setter Property="AccentColor" Value="{AppThemeBinding Light={StaticResource Primary}, Dark={StaticResource PrimaryDark}}" />
                    </VisualState.Setters>
                </VisualState>
                <VisualState x:Name="Disabled">
                    <VisualState.Setters>
                        <Setter Property="Opacity" Value="0.6" />
                        <Setter Property="BorderColor" Value="{StaticResource DisabledText}" />
                    </VisualState.Setters>
                </VisualState>
            </VisualStateGroup>
        </VisualStateGroupList>
    </Setter>
</Style>

<Style TargetType="c:RadioButton" ApplyToDerivedTypes="True">
    <Setter Property="VisualStateManager.VisualStateGroups">
        <VisualStateGroupList>
            <VisualStateGroup x:Name="CommonStates">
                <VisualState x:Name="Normal">
                    <VisualState.Setters>
                        <Setter Property="Opacity" Value="1"/>
                    </VisualState.Setters>
                </VisualState>
                <VisualState x:Name="Disabled">
                    <VisualState.Setters>
                        <Setter Property="Opacity" Value="0.6" />
                    </VisualState.Setters>
                </VisualState>
            </VisualStateGroup>
        </VisualStateGroupList>
    </Setter>
</Style>
```

- Find input:CheckBox in your StyleOverride file and update CheckBox styles like following. _(**VisualStateManager.VisualStateGroups** should be added.)_
```xml

 <Style TargetType="input:CheckBox" ApplyToDerivedTypes="True">
    <!-- ... -->

    <!-- Following section should be added. 👇 -->
    <Setter Property="VisualStateManager.VisualStateGroups">
        <VisualStateGroupList>
            <VisualStateGroup x:Name="CommonStates">
                <VisualState x:Name="Normal">
                    <VisualState.Setters>
                        <Setter Property="Opacity" Value="1"/>
                    </VisualState.Setters>
                </VisualState>
                <VisualState x:Name="Disabled">
                    <VisualState.Setters>
                        <Setter Property="Opacity" Value="0.6" />
                    </VisualState.Setters>
                </VisualState>
            </VisualStateGroup>
        </VisualStateGroupList>
    </Setter>
</Style>
```

- Find input:RadioButton in your StyleOverride file and update RadioButton styles like following. _(**VisualStateManager.VisualStateGroups** should be added.)_
```xml
<Style TargetType="input:RadioButton" ApplyToDerivedTypes="True">
    <!-- ... -->

    <!-- Following section should be added. 👇 -->
    <Setter Property="VisualStateManager.VisualStateGroups">
        <VisualStateGroupList>
            <VisualStateGroup x:Name="CommonStates">
                <VisualState x:Name="Normal">
                    <VisualState.Setters>
                        <Setter Property="Opacity" Value="1"/>
                    </VisualState.Setters>
                </VisualState>
                <VisualState x:Name="Disabled">
                    <VisualState.Setters>
                        <Setter Property="Opacity" Value="0.6" />
                    </VisualState.Setters>
                </VisualState>
            </VisualStateGroup>
        </VisualStateGroupList>
    </Setter>
</Style>
```
> _**Note**: If you're not sure about it, you can back-up your custom styles and re-add styles with `dotnet new uranium-material-resources -n MaterialOverride` command._


