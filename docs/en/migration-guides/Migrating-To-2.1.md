# Migration Guide to v2.1
Version 2.1 comes with some changes. You should follow this docuemnt to migrate your code to the new version properly.

## Changes
UraniumUI has a couple of changes in this version. Applying following changes to your code will make it compatible with UraniumUI v2.1.

- **Material Theme** should be initialized in **MauiProgram.cs**

    ```csharp
    builder
        .UseMauiApp<App>()
        .UseUraniumUI()
        .UseUraniumUIMaterial() // 👈 This is new!
        .ConfigureFonts(fonts =>
        {
            fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            fonts.AddMaterialIconFonts();
        });
    ```

- You should add following style to your resources **if you overrided** the default style.

    ```xml
        xmlns:views="clr-namespace:UraniumUI.Views;assembly=UraniumUI"
    ```


    ```xml
    <Style TargetType="views:StatefulContentView" ApplyToDerivedTypes="True">
        <Setter Property="VisualStateManager.VisualStateGroups">
            <VisualStateGroupList>
                <VisualStateGroup x:Name="CommonStates">
                    <VisualState x:Name="PointerOver">
                        <VisualState.Setters>
                            <Setter Property="Opacity" Value="0.8" />
                        </VisualState.Setters>
                    </VisualState>
                    <VisualState x:Name="Normal">
                        <VisualState.Setters>
                            <Setter Property="Opacity" Value="1.0" />
                        </VisualState.Setters>
                    </VisualState>
                    <VisualState x:Name="Pressed">
                        <VisualState.Setters>
                            <Setter Property="Opacity" Value="0.5" />
                        </VisualState.Setters>
                    </VisualState>
                </VisualStateGroup>
            </VisualStateGroupList>
        </Setter>
    </Style>

    <Style TargetType="c:ButtonView" ApplyToDerivedTypes="True">
        <Setter Property="BackgroundColor"  Value="{StaticResource Primary}" />
        <Setter Property="VisualStateManager.VisualStateGroups">
            <VisualStateGroupList>
                <VisualStateGroup x:Name="CommonStates">
                    <VisualState x:Name="PointerOver">
                        <VisualState.Setters>
                            <Setter Property="BackgroundColor" Value="{StaticResource SurfaceTint1}" />
                            <Setter Property="Opacity" Value="0.9" />
                        </VisualState.Setters>
                    </VisualState>
                    <VisualState x:Name="Normal"/>
                    <VisualState x:Name="Pressed">
                        <VisualState.Setters>
                            <Setter Property="BackgroundColor" Value="{StaticResource SurfaceTint2}" />
                            <Setter Property="Opacity" Value="0.8" />
                        </VisualState.Setters>
                    </VisualState>
                </VisualStateGroup>
            </VisualStateGroupList>
        </Setter>
    </Style>
    ```