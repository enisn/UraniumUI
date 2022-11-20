# Color System
Uranium UI has a color system that allows you to use colors in a more flexible way. Uranium defines color names and a regular color palette. Each theme can customize and override it. So, application theme can be easily changed.

Uranium Core provides a base color palette that can be used in your application. Each theme can provide their own colors. You can also use your own colors.

## Configuration
You should configure default Theme resources in your `App.xaml`. Default resources are included in `UraniumUI.Resources` namespace, it can be defined `xmlns:u="http://schemas.microsoft.com/dotnet/2022/maui/uraniumui"` as xml namespace.

You can either use only `ColorResource` or you can use `StyleResource` to get all theme resources. 

> If you're not developing a theme, you should use both of `StyleResource` and `ColorResource` to get all theme resources.
> Resources are separated, because you may need specific resources while developing a theme.

```xml
<?xml version = "1.0" encoding = "UTF-8" ?>
<Application xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:local="clr-namespace:App1"
             xmlns:u="http://schemas.microsoft.com/dotnet/2022/maui/uraniumui"
             x:Class="App1.App">
    <Application.Resources>
        <ResourceDictionary>
            <ResourceDictionary.MergedDictionaries>
                <ResourceDictionary Source="Resources/Styles/Colors.xaml" />
                <!-- 👇 You can use theme colors right after your Colors.xaml -->
                <u:ColorResource />
                <ResourceDictionary Source="Resources/Styles/Styles.xaml" />
                
            </ResourceDictionary.MergedDictionaries>
        </ResourceDictionary>
    </Application.Resources>
</Application>
```

## Colors

Default colors are imported from [Material Design](https://m3.material.io/styles/color/the-color-system/tokens#7fd4440e-986d-443f-8b3a-4933bff16646) color system. You can use these colors in your application. You can also override these colors in your theme.

![MAUI Material Design](https://lh3.googleusercontent.com/KvS4VAUMcFwCX3FDFOzuAGoO6Okpk5JXhnCkzRx9ehxCML6-FZ_cdJw20-mDbifLYrbTlULpM_SvIhl9n_T9zjQtT78MDThbxeEvIb1brKq0=s0)

![MAUI Material Design Dark](https://lh3.googleusercontent.com/nQHmWgLpXxjfV9nC_xIabgJDagi5V3aBB9qbFRA_EHEkEeTaq3uh-rYwoXnkRqL1eHCobVjb8lmQgdistb_XNcCfVdsQqUC-h0hvje4j6Qk=s0)

Color Palette is presented below. You can get it as a base for your own color palette.

```xml

    <Color x:Key="Primary">#6750A4</Color>
    <Color x:Key="PrimaryDark">#D0BCFF</Color>
    <Color x:Key="PrimaryContainer">#EADDFF</Color>
    <Color x:Key="PrimaryContainerDark">#4F378B</Color>

    <Color x:Key="Secondary">#625B71</Color>
    <Color x:Key="SecondaryDark">#CCC2DC</Color>
    <Color x:Key="SecondaryContainer">#E8DEF8</Color>
    <Color x:Key="SecondaryContainerDark">#4A4458</Color>

    <Color x:Key="Tertiary">#7D5260</Color>
    <Color x:Key="TertiaryDark">#EFB8C8</Color>
    <Color x:Key="TertiaryContainer">#FFD8E4</Color>
    <Color x:Key="TertiaryContainerDark">#633B48</Color>

    <Color x:Key="Surface">#FFFBFE</Color>
    <Color x:Key="SurfaceDark">#1C1B1F</Color>

    <Color x:Key="SurfaceVariant">#E7E0EC</Color>
    <Color x:Key="SurfaceVariantDark">#49454F</Color>

    <Color x:Key="Background">#FFFBFE</Color>
    <Color x:Key="BackgroundDark">#1C1B1F</Color>

    <Color x:Key="Error">#B3261E</Color>
    <Color x:Key="ErrorDark">#F2B8B5</Color>
    <Color x:Key="ErrorContainer">#F9DEDC</Color>
    <Color x:Key="ErrorContainerDark">#8C1D18</Color>

    <Color x:Key="OnPrimary">#FFFFFF</Color>
    <Color x:Key="OnPrimaryDark">#371E73</Color>
    <Color x:Key="OnPrimaryContainer">#21005E</Color>
    <Color x:Key="OnPrimaryContainerDark">#EADDFF</Color>

    <Color x:Key="OnSecondary">#FFFFFF</Color>
    <Color x:Key="OnSecondaryDark">#332D41</Color>
    <Color x:Key="OnSecondaryContainer">#1E192B</Color>
    <Color x:Key="OnSecondaryContainerDark">#E8DEF8</Color>

    <Color x:Key="OnTertiary">#FFFFFF</Color>
    <Color x:Key="OnTertiaryDark">#492532</Color>
    <Color x:Key="OnTertiaryContainer">#370B1E</Color>
    <Color x:Key="OnTertiaryContainerDark">#FFD8E4</Color>

    <Color x:Key="OnSurface">#1C1B1F</Color>
    <Color x:Key="OnSurfaceDark">#E6E1E5</Color>

    <Color x:Key="OnSurfaceVariant">#49454E</Color>
    <Color x:Key="OnSurfaceVariantDark">#CAC4D0</Color>

    <Color x:Key="OnError">#FFFFFF</Color>
    <Color x:Key="OnErrorDark">#601410</Color>
    <Color x:Key="OnErrorContainer">#370B1E</Color>
    <Color x:Key="OnErrorContainerDark">#F9DEDC</Color>

    <Color x:Key="OnBackground">#1C1B1F</Color>
    <Color x:Key="OnBackgroundDark">#E6E1E5</Color>

    <Color x:Key="Outline">#79747E</Color>
    <Color x:Key="OutlineDark">#938F99</Color>

    <Color x:Key="OutlineVariant">#C4C7C5</Color>
    <Color x:Key="OutlineVariantDark">#444746</Color>

    <Color x:Key="Shadow">#000000</Color>
    <Color x:Key="ShadowDark">#000000</Color>

    <Color x:Key="SurfaceTint">#6750A4</Color>
    <Color x:Key="SurfaceTintDark">#D0BCFF</Color>

    <Color x:Key="InverseSurface">#313033</Color>
    <Color x:Key="InverseSurfaceDark">#E6E1E5</Color>
    <Color x:Key="InverseOnSurface">#F4EFF4</Color>
    <Color x:Key="InverseOnSurfaceDark">#313033</Color>

    <Color x:Key="InversePrimary">#D0BCFF</Color>
    <Color x:Key="InversePrimaryDark">#6750A4</Color>

    <Color x:Key="Scrim">#000000</Color>
    <Color x:Key="ScrimDark">#000000</Color>
```

