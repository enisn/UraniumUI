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

- You should add following style changes into your resources **if you overrided** the default style.

    - The following xml namespace should be added to the root element of your resource file.
        ```xml
            xmlns:views="clr-namespace:UraniumUI.Views;assembly=UraniumUI"
        ```

    Make sure those changes are applied in your style file: 
    https://github.com/enisn/UraniumUI/commit/3e1038d0b6d6d1cacc8d160e44fc4ae93d69fc34