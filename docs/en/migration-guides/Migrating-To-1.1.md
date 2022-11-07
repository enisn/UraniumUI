# Migration Guide to v1.1
Version 1.1 comes with some changes. You should follow this docuemnt to migrate your code to the new version properly.

## Changes

### Disabled States
Each input has a disabled state now. It's working well after updating to v1.1. But you should add following style to your resources if you overrided the default style.

- Add following section in your StyleOverride file if you are using StyleOverride.
```xml

                <Style TargetType="c:CheckBox" ApplyToDerivedTypes="True">
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