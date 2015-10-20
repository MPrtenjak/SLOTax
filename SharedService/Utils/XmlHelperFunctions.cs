// <copyright file="XmlHelperFunctions.cs" company="MNet">
//     Copyright (c) Matjaz Prtenjak All rights reserved.
// </copyright>
// <author>Matjaz Prtenjak</author>
//-----------------------------------------------------------------------

using System;
using System.Xml;

namespace MNet.SLOTaxService.Utils
{
  internal class XmlHelperFunctions
  {
    public static XmlNode GetSubNode(XmlElement element, string fullNodeName)
    {
      XmlNodeList nodeList = element.GetElementsByTagName(fullNodeName);
      if (nodeList.Count != 1) return null;

      return nodeList[0];
    }

    public static void Validate(XmlDocument message)
    {
      string errMsgs = string.Empty;

      message.Validate((sender, args) => { errMsgs += args.Message; });

      if (!string.IsNullOrEmpty(errMsgs))
        throw new ArgumentOutOfRangeException(errMsgs);
    }

    public static XmlNode CreateElement(XmlDocument message, string fursNamespace, string localName, string value = null)
    {
      XmlNode newNode = message.CreateElement("fu", localName, fursNamespace);
      if (value != null) newNode.InnerText = value;
      return newNode;
    }

    public static XmlDocument CreateNewXmlDocument(string fileName = null)
    {
      XmlDocument xml = new XmlDocument();
      xml.PreserveWhitespace = true;

      if (!string.IsNullOrEmpty(fileName))
        xml.Load(fileName);

      return xml;
    }
  }
}