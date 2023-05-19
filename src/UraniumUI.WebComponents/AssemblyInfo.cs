
using UraniumUI;

[assembly: XmlnsDefinition(Constants.XamlNamespace, Constants.NamespacePrefix + nameof(UraniumUI.WebComponents))]
[assembly: XmlnsDefinition(Constants.XamlNamespace, Constants.NamespacePrefix + nameof(UraniumUI.WebComponents) + "." + nameof(UraniumUI.WebComponents.Controls))]

[assembly: Microsoft.Maui.Controls.XmlnsPrefix(Constants.XamlNamespace, "uranium")]