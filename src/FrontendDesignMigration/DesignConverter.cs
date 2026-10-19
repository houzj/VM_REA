using System.IO;
using System.Text;
using System.Xml;
using System.Windows;
namespace FrontendDesignMigration;

// Source-guided managed XML migration only. No vendor SDK or XAML execution.
public sealed class DesignConverter(string defaultDesign, string defaultSubDesign, Encoding encoding)
{
    const string ControlsNamespace = "xmlns:controls=\"clr-namespace:FrontendUI.WPF.Controls;assembly=FrontendUI.WPF\"";
    static void LoadXml(XmlDocument document, string xml)
    {
        using var text = new StringReader(xml);
        using var reader = XmlReader.Create(text, new XmlReaderSettings { DtdProcessing = DtdProcessing.Prohibit, XmlResolver = null, MaxCharactersInDocument = 32 * 1024 * 1024 });
        document.Load(reader);
    }
    XmlDocument GetDefaultDesign(string name)
    {
        var doc = new XmlDocument();
        LoadXml(doc, File.ReadAllText(name == "DefaultDesign" ? defaultDesign : defaultSubDesign));
        return doc;
    }
    public string ConvertV40ToV44(string xml)
    {
        var source = new XmlDocument(); LoadXml(source, xml);
        // Preserve original FirstChild dispatch, including its declaration behavior.
        if (source.FirstChild is not XmlElement first || first.LocalName != "Root")
            throw new InvalidDataException("Only the source-confirmed Root/V4.0 design branch is supported.");
        xml = first.OuterXml
            .Replace("xmlns:Lang=\"clr-namespace:Apps.Localization;assembly=Apps.Localization\"", "")
            .Replace("Title=\"{Lang:LangResource Lang_FrontDefaultTitle}\"", "")
            .Replace("参数配置\" ", "ParameterSetting\" ")
            .Replace("参数重置\" ", "ParameterReset\" ");
        source = new XmlDocument(); LoadXml(source, xml);
        bool modified = true;
        var result = DesignDocFromVM4_3ToVM4_4Convert(ref modified, source);
        if (result is null) throw new InvalidDataException("No converted design.");
        AddAttributeWidthValue(result, result.FirstChild, "VersionInfo", "V4.4.0");
        return result.OuterXml.Replace(ControlsNamespace, "");
    }
	public XmlDocument DesignDocFromVM4_3ToVM4_4Convert(ref bool isModifyOK, XmlDocument doc_430)
	{
		XmlNode xmlNode = doc_430.FirstChild.CloneNode(deep: true);
		XmlDocument defaultDesign = GetDefaultDesign("DefaultDesign");
		if (defaultDesign == null)
		{
			return null;
		}
		XmlNode firstChild = defaultDesign.FirstChild;
		XmlNode lastChild = defaultDesign.FirstChild.LastChild;
		XmlNode xmlNode2 = null;
		List<XmlNode> list = lastChild?.FirstChild?.ChildNodes.OfType<XmlNode>().ToList();
		if (list != null)
		{
			foreach (XmlElement item in list)
			{
				if (item == null || item.LocalName != "FrontendTabItem" || !(item.FirstChild.LocalName == "SubRoot"))
				{
					continue;
				}
				xmlNode2 = item.FirstChild;
				break;
			}
		}
		if (firstChild != null && xmlNode2 != null && xmlNode.ChildNodes.Count > 0)
		{
			XmlNode namedItem = xmlNode.Attributes.GetNamedItem("Size");
			if (namedItem != null && !string.IsNullOrWhiteSpace(namedItem.Value))
			{
				Size size = Size.Parse(namedItem.Value);
				AddAttributeWidthValue(defaultDesign, firstChild, "Width", size.Width.ToString());
				AddAttributeWidthValue(defaultDesign, firstChild, "Height", (size.Height + 60.0).ToString());
			}
			XmlNode namedItem2 = xmlNode.Attributes.GetNamedItem("Background");
			if (namedItem2 != null && !string.IsNullOrWhiteSpace(namedItem2.Value))
			{
				AddAttributeWidthValue(defaultDesign, firstChild, namedItem2.Name, namedItem2.Value);
			}
			foreach (XmlNode childNode in xmlNode.ChildNodes)
			{
				XmlNode xmlNode3 = defaultDesign.ImportNode(childNode, deep: true);
				ModifyAttributeForOneXmlNode(defaultDesign, xmlNode3);
				xmlNode2.AppendChild(xmlNode3);
			}
			isModifyOK = true;
		}
		return defaultDesign;
	}

	private void ModifyAttributeForOneXmlNode(XmlDocument doc_440, XmlNode newXmlNode)
	{
		if (newXmlNode.LocalName == "GroupBox")
		{
			AddAttributeWidthValue(doc_440, newXmlNode, "AutoStrechA", "false");
			if (newXmlNode.FirstChild != null)
			{
				foreach (XmlNode childNode in newXmlNode.FirstChild.ChildNodes)
				{
					if (childNode != null)
					{
						ModifyAttributeForOneXmlNode(doc_440, childNode);
					}
				}
			}
		}
		else if (newXmlNode.LocalName == "Tab")
		{
			AddAttributeWidthValue(doc_440, newXmlNode, "AutoStrechA", "false");
			foreach (XmlNode childNode2 in newXmlNode.ChildNodes)
			{
				if (childNode2 == null || childNode2.FirstChild == null)
				{
					continue;
				}
				foreach (XmlNode childNode3 in childNode2.FirstChild.ChildNodes)
				{
					if (childNode3 != null)
					{
						ModifyAttributeForOneXmlNode(doc_440, childNode3);
					}
				}
			}
		}
		else if (newXmlNode.LocalName == "SubWindow")
		{
			string attributeValueByPropertyName = GetAttributeValueByPropertyName(newXmlNode, "Document");
			XmlAttribute xmlAttribute = newXmlNode.Attributes?.OfType<XmlAttribute>().FirstOrDefault((XmlAttribute p) => p.Name == "Document");
			if (xmlAttribute != null)
			{
				string value = xmlAttribute.Value;
				string xml = Base64StringToString(value);
				XmlDocument xmlDocument = new XmlDocument();
				LoadXml(xmlDocument, xml);
				XmlNode xmlNode4 = xmlDocument.FirstChild.CloneNode(deep: true);
				XmlDocument defaultDesign = GetDefaultDesign("DefaultSubDesign");
				XmlNode firstChild = defaultDesign.FirstChild;
				if (firstChild != null && xmlNode4.ChildNodes.Count > 0)
				{
					foreach (XmlNode childNode4 in xmlNode4.ChildNodes)
					{
						XmlNode xmlNode5 = defaultDesign.ImportNode(childNode4, deep: true);
						ModifyAttributeForOneXmlNode(defaultDesign, xmlNode5);
						firstChild.AppendChild(xmlNode5);
					}
				}
				string text = defaultDesign.OuterXml.ToString();
				if (text.Contains("xmlns:controls=\"clr-namespace:FrontendUI.WPF.Controls;assembly=FrontendUI.WPF\""))
				{
					text = text.Replace("xmlns:controls=\"clr-namespace:FrontendUI.WPF.Controls;assembly=FrontendUI.WPF\"", "");
				}
				string value2 = String2Base64(text);
				xmlAttribute.Value = value2;
			}
		}
		else
		{
			switch (newXmlNode.LocalName)
			{
			case "ImageControl":
			case "MultiImageControl":
			case "TableControl":
			case "PicBox":
				AddAttributeWidthValue(doc_440, newXmlNode, "AutoStrechA", "false");
				break;
			}
		}
		XmlNode namedItem = newXmlNode.Attributes.GetNamedItem("Size");
		XmlNode namedItem2 = newXmlNode.Attributes.GetNamedItem("Width");
		XmlNode namedItem3 = newXmlNode.Attributes.GetNamedItem("Height");
		if (namedItem != null && !string.IsNullOrWhiteSpace(namedItem.Value))
		{
			Size size = Size.Parse(namedItem.FirstChild.Value);
			if (namedItem2 == null)
			{
				AddAttributeWidthValue(doc_440, newXmlNode, "Width", size.Width.ToString());
			}
			else if (namedItem2.Value != size.Width.ToString())
			{
				namedItem2.Value = size.Width.ToString();
			}
			if (namedItem3 == null)
			{
				AddAttributeWidthValue(doc_440, newXmlNode, "Height", size.Height.ToString());
			}
			else if (namedItem3.Value != size.Height.ToString())
			{
				namedItem3.Value = size.Height.ToString();
			}
			newXmlNode.Attributes.RemoveNamedItem("Size");
		}
		XmlNode namedItem4 = newXmlNode.Attributes.GetNamedItem("Background");
		if (namedItem4 != null && namedItem4.Value == "{x:Null}")
		{
			namedItem4.Value = "#00000000";
		}
		XmlNode namedItem5 = newXmlNode.Attributes.GetNamedItem("BorderBrush");
		if (namedItem5 != null && namedItem5.Value == "{x:Null}")
		{
			namedItem5.Value = "#00000000";
		}
	}


	public void AddAttributeWidthValue(XmlDocument doc, XmlNode node, string AttributeName, string value, string namespaceURI = "")
	{
		XmlAttribute xmlAttribute = doc.CreateAttribute(AttributeName, namespaceURI);
		xmlAttribute.Value = value;
		if (node.Attributes == null)
		{
			if (node is XmlElement)
			{
				(node as XmlElement).SetAttributeNode(xmlAttribute);
			}
		}
		else
		{
			node.Attributes.Append(xmlAttribute);
		}
	}

	public string Base64StringToString(string base64)
	{
		if (string.IsNullOrEmpty(base64))
		{
			return string.Empty;
		}
		try
		{
			byte[] bytes = Convert.FromBase64String(base64);
			return encoding.GetString(bytes);
		}
		catch (Exception)
		{
			return string.Empty;
		}
	}

	public string String2Base64(string input)
	{
		return string.IsNullOrEmpty(input) ? string.Empty : Bytes2Base64(encoding.GetBytes(input));
	}

	public string Bytes2Base64(byte[] buffer)
	{
		if (buffer == null)
		{
			return string.Empty;
		}
		return Convert.ToBase64String(buffer);
	}

	private string GetAttributeValueByPropertyName(XmlNode child, string name)
	{
		string text = child.Attributes?.OfType<XmlAttribute>().FirstOrDefault((XmlAttribute p) => p.Name == name)?.Value;
		if (text == "{x:Null}")
		{
			text = null;
		}
		return text;
	}

}
