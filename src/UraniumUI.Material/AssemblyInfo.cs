[assembly: XmlnsDefinition(Constants.XamlNamespace, Constants.NamespacePrefix + nameof(UraniumUI.Material.Attachments))]
[assembly: XmlnsDefinition(Constants.XamlNamespace, Constants.NamespacePrefix + nameof(UraniumUI.Material.Controls))]
[assembly: XmlnsDefinition(Constants.XamlNamespace, Constants.NamespacePrefix + nameof(UraniumUI.Material.Extensions))]
[assembly: XmlnsDefinition(Constants.XamlNamespace, Constants.NamespacePrefix + nameof(UraniumUI.Material.Resources))]

[assembly: Microsoft.Maui.Controls.XmlnsPrefix(Constants.XamlNamespace, "material")]


class Constants
{
    public const string XamlNamespace = "http://schemas.enisn-projects.io/dotnet/maui/uraniumui/material";

    public const string NamespacePrefix = $"{nameof(UraniumUI)}.{nameof(UraniumUI.Material)}.";
}