# Getting Started
Uranium UI is a UI framework for .NET MAUI. It is built on top of the .NET MAUI infrastructure and provides a set of controls and layouts to build modern UIs. It also provides infrastructure for building custom controls and themes on it.

## Installation
- Install the [Uranium.UI](https://www.nuget.org/packages/Uranium.UI/) NuGet package.
- Go to `MauiProgram.cs` and add UraniumUI Handlers

    ```csharp
    .ConfigureMauiHandlers(handlers =>
    {
        handlers.AddUraniumUIHandlers(); // 👈 This line should be added.
    });
    ```

- Uranium UI doesnt include any theme by default. Pick one of the themes and install it.

    - [Material Theme](themes/material/Index.md)