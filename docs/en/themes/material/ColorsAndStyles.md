# Colors & Styles
Uranium UI Material Theme on MAUI uses [Static Resources](https://docs.microsoft.com/en-us/dotnet/maui/fundamentals/resource-dictionaries). It's best option for performance against **dynamic resources**. So, they can't be overriden in application. But they can be replaced entirely. Uranium UI provides a way to do it easily.

## Defining Resources

Material Theme includes its own resources that you should place in your app.xaml file.

- Firstly define the namespace of Uranium UI Material Theme.

    ```xml
    xmlns:m="clr-namespace:UraniumUI.Material.Resources;assembly=UraniumUI.Material"
    ```

- Then, you can add StyleResource of material in your app.xaml file.

```xml
 <ResourceDictionary.MergedDictionaries>
    <ResourceDictionary Source="Resources/Styles/Colors.xaml" />
    <ResourceDictionary Source="Resources/Styles/Styles.xaml" />
    <m:StyleResource />
</ResourceDictionary.MergedDictionaries>
```

### Merging Resources
By default, the last resource dictionary in the list will override the previous ones. So, your previous resources will be overridden by Uranium UI Material Theme resources. If you want to keep your resources and override by Uranium UI Material Theme resources, you can use `BasedOn` property of `StyleResource` as below:

```xml
 <ResourceDictionary.MergedDictionaries>
    <ResourceDictionary x:Name="appStyles" Source="Resources/Styles/Colors.xaml" />
    <ResourceDictionary Source="Resources/Styles/Styles.xaml" />
    <m:StyleResource BasedOn="{x:Reference appStyles}" />
</ResourceDictionary.MergedDictionaries>
```

### Overriding Styles
StyleResource has `Overrides` property that allows defining ResourceDictionaries that will override existing resources. You can define it like this:

```xml
    <m:StyleResource>
        <m:StyleResource.Overrides>
            <ResourceDictionary>
                <Style TargetType="Button">
                    <Setter Property="Background" Value="Red" />
                </Style>
            </ResourceDictionary>
        </m:StyleResource.Overrides>
    </m:StyleResource>
```

 You can use a sparated file for overrides.

```xml
    <m:StyleResource>
        <m:StyleResource.Overrides>
            <ResourceDictionary Source="Resources/Styles/Styles.xaml" />
        </m:StyleResource.Overrides>
    </m:StyleResource>
```

### Changing Colors
UraniumUI uses [Static Resources](https://learn.microsoft.com/en-us/dotnet/maui/fundamentals/resource-dictionaries?view=net-maui-7.0#consume-resources) for colors. So, you can't override them. But you can replace them entirely. Uranium UI provides a way to do it easily. You can use your own Colors.xaml file to replace Uranium UI Material Theme colors.

```xml
<ResourceDictionary.MergedDictionaries>
    <ResourceDictionary x:Name="appColors" Source="Resources/Styles/Colors.xaml" />
    <m:StyleResource ColorsOverride="{x:Reference appColors}" />
</ResourceDictionary.MergedDictionaries>
```
