# CodeView
`CodeView` displays source code inside a WebView-based syntax highlighter.

## Getting Started

Install the package:

```bash
dotnet add package UraniumUI.WebComponents
```

Register it in `MauiProgram.cs`:

```csharp
builder
    .UseMauiApp<App>()
    .UseUraniumUI()
    .UseUraniumUIWebComponents();
```

## Usage

`CodeView` is exported through the UraniumUI XAML namespace.

```xml
xmlns:uranium="http://schemas.enisn-projects.io/dotnet/maui/uraniumui"
```

```xml
<uranium:CodeView
    Language="csharp"
    SourceCode="public string Message => \"Hello\";"
    HeightRequest="160" />
```

## Properties

- `SourceCode`: The code to render.
- `Language`: The syntax highlighting language.
- `Theme`: The highlight theme. By default it uses `github` in light theme and `github-dark` in dark theme.
