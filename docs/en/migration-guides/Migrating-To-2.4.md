# Migration Guide to v2.4
Version 2.4 comes with some changes. You should follow this docuemnt to migrate your code to the new version properly.

## Changes
The biggest change in UraniumUI v2.4 is styling enhancements. Style resource items are replacing the old style resources by default. V2.4 prevents unwanted overrides for your application. Since it's not a completely breaking change but taking action is recommended. Please switch to new styling system.


### Styling
If you have the following pattern in your `App.xaml` file, you can easily replace with the suggested one.

```xml
<Application.Resources>
    <ResourceDictionary>
        <ResourceDictionary.MergedDictionaries>
            <ResourceDictionary Source="Resources/Styles/Colors.xaml" /> 
            <material:ColorResource />
            <ResourceDictionary Source="Resources/Styles/Styles.xaml" />
            <material:StyleResource />
        </ResourceDictionary.MergedDictionaries>
    </ResourceDictionary>
</Application.Resources>
```

You should change it to the following:

```xml
<Application.Resources>
    <ResourceDictionary>
        <ResourceDictionary.MergedDictionaries>
            <ResourceDictionary x:Name="appColors" Source="Resources/Styles/Colors.xaml" />
            <ResourceDictionary x:Name="appStyles" Source="Resources/Styles/Styles.xaml" />
            <material:StyleResource BasedOn="{x:Reference appStyles}" ColorsOverride="{x:Reference appColors}" />
        </ResourceDictionary.MergedDictionaries>
    </ResourceDictionary>
</Application.Resources>
```
- And your `Colors.xaml` file should be like the following:
    https://github.com/enisn/UraniumUI/blob/develop/templates/app-blank/MyCompany.MyProject/Resources/Styles/Colors.xaml


