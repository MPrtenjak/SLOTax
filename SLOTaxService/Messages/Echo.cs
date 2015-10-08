// <copyright file="Echo.cs" company="MNet">
//     Copyright (c) Matjaz Prtenjak All rights reserved.
// </copyright>
// <author>Matjaz Prtenjak</author>
//-----------------------------------------------------------------------

using System.Xml;
using MNet.SLOTaxService.Services;
using MNet.SLOTaxService.Utils;

namespace MNet.SLOTaxService.Messages
{
  internal class Echo : BaseMessage, IMessage
  {
    public static Echo Create(string message, Settings settings)
    {
      string xmlEcho = @"<?xml version='1.0' encoding='UTF-8'?><fu:EchoRequest xmlns:fu='http://www.fu.gov.si/' />";
      XmlDocument echoDoc = XmlHelperFunctions.CreateNewXmlDocument();
      echoDoc.LoadXml(xmlEcho);

      XmlNode xmlMessage = echoDoc.CreateTextNode(message);
      echoDoc.DocumentElement.AppendChild(xmlMessage);
      return new Echo(echoDoc, settings);
    }

    public static Echo Create(XmlDocument message, Settings settings)
    {
      return new Echo(message, settings);
    }

    public void Check()
    {
      this.checkRoot(MessageType.Echo);
    }

    public void Sign()
    {
    }

    private Echo(XmlDocument message, Settings settings) :
      base(message, settings, MessageAction.Send)
    {
    }
  }
}