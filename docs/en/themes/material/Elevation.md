# Elevation

Elevation is a visual effect that can be applied to a component to make it appear to be raised above the surface of the page. It is used to indicate the relative importance of an element on the page.

_You can visit original [Material Design Elevation Guideline]_

## Configuration
Elevation styles come from `StyleResource` from `UraniumUI.Material.Resources` namespace. It should be added to `App.xaml` as a merged dictionary to use it.

```xml
<?xml version = "1.0" encoding = "UTF-8" ?>
<Application xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:local="clr-namespace:MyCompany.MyProject"
             xmlns:material="clr-namespace:UraniumUI.Material.Resources;assembly=UraniumUI.Material"
             x:Class="MyCompany.MyProject.App">
    <Application.Resources>
        <ResourceDictionary>
            <ResourceDictionary.MergedDictionaries>
                <ResourceDictionary Source="Resources/Styles/Colors.xaml" />
                <material:ColorResource />
                <ResourceDictionary Source="Resources/Styles/Styles.xaml" />
                <material:StyleResource /> <!-- 👈 It should be added right after Styles.xaml -->
            </ResourceDictionary.MergedDictionaries>
        </ResourceDictionary>
    </Application.Resources>
</Application>

```

## Usage

Elevation can be applied any type of `View` by using Elevation style classes.
- `Elevation0`
- `Elevation1`
- `Elevation2`
- `Elevation3`
- `Elevation4`
- `Elevation5`

```xml
<StackLayout>
    <BoxView StyleClass="Elevation1" Margin="30" />
</StackLayout>
```

![MAUI Material Design Elevation](images/elevation-single-demo.png)


All elevation levels are listed below.

```xml
<VerticalStackLayout Padding="50" Spacing="20">
    <BoxView StyleClass="Elevation1" />
    <BoxView StyleClass="Elevation2" />
    <BoxView StyleClass="Elevation3" />
    <BoxView StyleClass="Elevation4" />
    <BoxView StyleClass="Elevation5" />
</VerticalStackLayout>
```


![MAUI Material Design Elevation](images/elevation-all-demo.png)
