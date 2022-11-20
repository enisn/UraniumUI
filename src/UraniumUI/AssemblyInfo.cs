using System;

[assembly: XmlnsDefinition(Constants.XamlNamespace, Constants.NamespacePrefix + nameof(UraniumUI.Pages))]
[assembly: XmlnsDefinition(Constants.XamlNamespace, Constants.NamespacePrefix + nameof(UraniumUI.Resources))]
[assembly: XmlnsDefinition(Constants.XamlNamespace, Constants.NamespacePrefix + nameof(UraniumUI.Theming))]
[assembly: XmlnsDefinition(Constants.XamlNamespace, Constants.NamespacePrefix + nameof(UraniumUI.Triggers))]
[assembly: XmlnsDefinition(Constants.XamlNamespace, Constants.NamespacePrefix + nameof(UraniumUI.Views))]

[assembly: Microsoft.Maui.Controls.XmlnsPrefix(Constants.XamlNamespace, "uranium")]


class Constants
{
    public const string XamlNamespace = "http://schemas.microsoft.com/dotnet/2022/maui/uraniumui";

    public const string NamespacePrefix = $"{nameof(UraniumUI)}.";
}