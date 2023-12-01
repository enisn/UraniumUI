# Migration Guide to v2.4
Version 2.4 comes with some changes. You should follow this docuemnt to migrate your code to the new version properly.

## Changes

- UraniumUI now prefers `Border` over `Frame` due to [dotnet/maui#18526](https://github.com/dotnet/maui/issues/18526)

  - Replace `Frame` with `Border` in XAML. You can use `SurfaceContainer` and `Rounded` classes to get the same look and feel as `Frame`.

    ```xml
    <Border StyleClass="SurfaceContainer,Rounded">
        <!-- You old Frame content -->
    </Border>
    ```

- _(OPTIONAL)_ This Step is optional. You can skip this step if you don't override existing UraniumUI styles in your project. 
  - The following styles has been added.
    ```xml
    <Style TargetType="c:TreeView">
        <Setter Property="SelectionColor" Value="{AppThemeBinding {StaticResource Tertiary}, Dark={StaticResource TertiaryDark}}" />
    </Style>

    <Style TargetType="Label" Class="TreeView.Label" BaseResourceKey="Microsoft.Maui.Controls.Label">
    </Style>

    <Style TargetType="Label" Class="TreeView.Label.Selected" BaseResourceKey="Microsoft.Maui.Controls.Label" >
        <Setter Property="TextColor" Value="{AppThemeBinding {StaticResource OnTertiary}, Dark={StaticResource OnTertiaryDark}}" />
    </Style>

    <Style TargetType="Path" Class="TreeView.Arrow" BaseResourceKey="Microsoft.Maui.Controls.Shapes.Path">
        <Setter Property="Fill" Value="{AppThemeBinding Light={StaticResource OnBackground},Dark={StaticResource OnBackgroundDark}}" />
    </Style>

    <Style TargetType="Path" Class="TreeView.Arrow.Selected" BaseResourceKey="Microsoft.Maui.Controls.StyleClass.TreeView.Arrow">
        <Setter Property="Fill" Value="{AppThemeBinding {StaticResource OnTertiary}, Dark={StaticResource OnTertiaryDark}}" />
    </Style>
    
    <Style TargetType="Border" Class="BottomSheet">
        <Setter Property="StrokeShape"  Value="RoundRectangle 8" />
        <Setter Property="StrokeThickness" Value="0" />
        <Setter Property="BackgroundColor" Value="{AppThemeBinding Light={StaticResource Surface}, Dark={StaticResource SurfaceDark}}" />
    </Style>
    ```

  - The `DataGrid` style has been updated:
    ```xml
    <Style TargetType="uranium:DataGrid" CanCascade="True">
        <Setter Property="BackgroundColor" Value="{AppThemeBinding Light={StaticResource  Surface},Dark={StaticResource  SurfaceDark}}" />
        <Setter Property="LineSeparatorColor" Value="{AppThemeBinding Light={StaticResource Outline}, Dark={StaticResource OutlineDark}}"/>
        <Setter Property="Stroke" Value="{AppThemeBinding Light={StaticResource Outline}, Dark={StaticResource OutlineDark}}" />
        <Setter Property="StrokeShape" Value="RoundRectangle 8" />
        <Setter Property="StrokeThickness" Value=".5" />
        <Setter Property="SelectionColor" Value="{AppThemeBinding Light={StaticResource Primary}, Dark={StaticResource PrimaryDark}}" />
    </Style>
    ```