using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace UraniumUI.StyleBuilder.StyleManager;

public static class Xml
{

    [XmlRoot(ElementName = "Color", Namespace = "http://schemas.microsoft.com/dotnet/2021/maui")]
    public class Color
    {

        [XmlAttribute(AttributeName = "Key", Namespace = "http://schemas.microsoft.com/winfx/2009/xaml")]
        public string Key { get; set; }

        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "SolidColorBrush", Namespace = "http://schemas.microsoft.com/dotnet/2021/maui")]
    public class SolidColorBrush
    {

        [XmlAttribute(AttributeName = "Key", Namespace = "http://schemas.microsoft.com/winfx/2009/xaml")]
        public string Key { get; set; }

        [XmlAttribute(AttributeName = "Color", Namespace = "")]
        public string Color { get; set; }
    }

    [XmlRoot(ElementName = "ResourceDictionary", Namespace = "http://schemas.microsoft.com/dotnet/2021/maui")]
    public class ResourceDictionary
    {

        [XmlElement(ElementName = "Color", Namespace = "http://schemas.microsoft.com/dotnet/2021/maui")]
        public List<Color> Colors { get; set; }

        [XmlElement(ElementName = "SolidColorBrush", Namespace = "http://schemas.microsoft.com/dotnet/2021/maui")]
        public List<SolidColorBrush> SolidColorBrushes { get; set; }

        [XmlAttribute(AttributeName = "xmlns", Namespace = "")]
        public string Xmlns { get; set; }

        [XmlAttribute(AttributeName = "x", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string X { get; set; }

        [XmlText]
        public string Text { get; set; }
    }

}
