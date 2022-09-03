# Color System
Uranium UI has a color system that allows you to use colors in a more flexible way. Uranium defines color names and a regular color palette. Each theme can customize and override it. So, application theme can be easily changed.

Uranium Core provides a base color palette that can be used in your application. Each theme can provide their own colors. You can also use your own colors.

## Configuration
You should configure default Theme resources in your `App.xaml`. Default resources are included in `UraniumUI.Resources` namespace, it can be defined `xmlns:u="clr-namespace:UraniumUI.Resources;assembly=UraniumUI"` as xml namespace.

You can either use only `ColorResource` or you can use `ThemeResource` to get all theme resources. 

> If you're not developing a theme, you should use `ThemeResource` to get all theme resources.
> Resources are separated, because you may need specific resources while developing a theme.

```xml
<?xml version = "1.0" encoding = "UTF-8" ?>
<Application xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:local="clr-namespace:App1"
             xmlns:u="clr-namespace:UraniumUI.Resources;assembly=UraniumUI"
             x:Class="App1.App">
    <Application.Resources>
        <ResourceDictionary>
            <ResourceDictionary.MergedDictionaries>
                <ResourceDictionary Source="Resources/Styles/Colors.xaml" />
                <ResourceDictionary Source="Resources/Styles/Styles.xaml" />
                
                <!-- 👇 You can configure entire resources -->
                <u:ThemeResource />
                <!-- 👇 Or you can use only colors -->
                <u:ColorResource />
                
            </ResourceDictionary.MergedDictionaries>
        </ResourceDictionary>
    </Application.Resources>
</Application>
```

## Color Names

An example color scheme is shown below:

```xml
    <Color x:Key="Primary">#512bdf</Color>
    <Color x:Key="OnPrimary">#D3CEDF</Color>
    <Color x:Key="PrimaryContainer">#C8B6E2</Color>
    <Color x:Key="OnPrimaryContainer">#363062</Color>

    <Color x:Key="Secondary">#0096FF</Color>
    <Color x:Key="OnSecondary">#C1EFFF</Color>
    <Color x:Key="SecondaryContainer">#C1EFFF</Color>
    <Color x:Key="OnSecondaryContainer">#3120E0</Color>

    <Color x:Key="Tertiary">#B5FF7D</Color>
    <Color x:Key="OnTertiary">#F5F1DA</Color>
    <Color x:Key="TertiaryContainer">#B3E8E5</Color>
    <Color x:Key="OnTertiaryContainer">#2F8F9D</Color>

    <Color x:Key="Background">#fff</Color>
    <Color x:Key="OnBackground">#000</Color>

    <Color x:Key="Surface">#f1f1f2</Color>
    <Color x:Key="OnSurface">#010102</Color>
```

