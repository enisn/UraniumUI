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

    - Make sure those changes are applied in your style file: 
      - https://github.com/enisn/UraniumUI/commit/9a1628409a94fd97f9a32aa318d6eca0742d0746 _(Style changes)_
      - https://github.com/enisn/UraniumUI/commit/b2ff5e5209b046ef9252a3786b93d4ac4b18b7d5 _(Color changes)_