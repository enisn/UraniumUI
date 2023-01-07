# Colors & Styles
Uranium UI Material Theme on MAUI uses [Static Resources](https://docs.microsoft.com/en-us/dotnet/maui/fundamentals/resource-dictionaries). It's best option for performance against **dynamic resources**. So, they can't be overriden in application. But they can be replaced entirely. Uranium UI provides a way to do it easily.


## Replacing Resources
Best way to replace resources is putting files that include resources into `Resources/Styles` folder in your project. Then, you can override any resource by editing an existing resource in those files or creating a new one.

You can check the latest version and place it into yor project from GitHub repository:

- [StyleResource.xaml](https://github.com/enisn/UraniumUI/blob/master/src/UraniumUI.Material/Resources/StyleResource.xaml)
- [ColorResource.xaml](https://github.com/enisn/UraniumUI/blob/master/src/UraniumUI.Material/Resources/ColorResource.xaml)

Manual copying and placing into project isn't the easiest way and it's not recommended. You can create those resource files in your project with **dotnet CLI**. [UraniumUI.Templates](https://www.nuget.org/packages/UraniumUI.Templates) already includes those files. So, you can create them with following command.

- Run following command under `/Resources/Styles` folder

```bash
dotnet new uranium-material-resources -n MaterialOverride
```
> _That command will create `MaterialOverrideColor.xaml` and `MaterialOverrideStyle.xaml` files in your project. You can edit them and override any resource._

- Then, go to `App.xaml` file and add those newly created resources. Final state should be like this:

    ```xml	
    <ResourceDictionary.MergedDictionaries>
        <ResourceDictionary Source="Resources/Styles/Colors.xaml" />
        <ResourceDictionary Source="Resources/Styles/MaterialColorOverride.xaml" />
        <ResourceDictionary Source="Resources/Styles/Styles.xaml" />
        <ResourceDictionary Source="Resources/Styles/MaterialStyleOverride.xaml" />
    </ResourceDictionary.MergedDictionaries>
    ```

> _You can remove `<material:ColorResource />` and `<material:StyleResource />` declarions from `App.xaml` file if you configured them before. They're already included in your project now. But after removing them, you'll not be able to get updates. So, you have to update those files after each project update. You can consider keeping your customizations in a separate file._

