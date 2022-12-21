using System;

[assembly: XmlnsDefinition(Constants.XamlNamespace, Constants.NamespacePrefix + nameof(UraniumUI.Validations))]

[assembly: Microsoft.Maui.Controls.XmlnsPrefix(Constants.XamlNamespace, "uranium")]

static class Constants
{
    public const string XamlNamespace = "http://schemas.enisn-projects.io/dotnet/maui/uraniumui";

    public const string NamespacePrefix = $"{nameof(UraniumUI)}.";
}