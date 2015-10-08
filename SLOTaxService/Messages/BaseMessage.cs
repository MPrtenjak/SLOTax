// <copyright file="BaseMessage.cs" company="MNet">
//     Copyright (c) Matjaz Prtenjak All rights reserved.
// </copyright>
// <author>Matjaz Prtenjak</author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Xml;
using MNet.SLOTaxService.Services;
using MNet.SLOTaxService.Utils;

namespace MNet.SLOTaxService.Messages
{
  internal class BaseMessage
  {
    public XmlDocument Message { get; private set; }
    public XmlDocument MessageSendToFurs { get; private set; }
    public XmlDocument MessageReceivedFromFurs { get; private set; }

    public MessageType MessageType { get; private set; }
    public MessageAction MessageAction { get; private set; }

    public BaseMessage(XmlDocument message, Settings settings, MessageAction messageAction)
    {
      this.Message = message;
      this.Settings = settings;
      this.MessageAction = messageAction;

      Dictionary<string, MessageType> types = new Dictionary<string, MessageType>();
      types.Add("InvoiceRequest", MessageType.Invoice);
      types.Add("BusinessPremiseRequest", MessageType.BuisinessPremise);
      types.Add("EchoRequest", MessageType.Echo);

      string root = this.Message.DocumentElement.LocalName;
      if (types.ContainsKey(root))
        this.MessageType = types[root];
      else
        this.MessageType = MessageType.Unknown;
    }

    public void SurroundWithSoap()
    {
      string emptySoap = string.Format(
        @"<soapenv:Envelope xmlns:soapenv='http://schemas.xmlsoap.org/soap/envelope/' " +
         "                  xmlns:fu='{0}' " +
         "                  xmlns:xd='http://www.w3.org/2000/09/xmldsig#' " +
         "                  xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance'> " +
         "    <soapenv:Header /> " +
         "    <soapenv:Body /> " +
         "</soapenv:Envelope>", this.Settings.FursXmlNamespace);

      this.MessageSendToFurs = XmlHelperFunctions.CreateNewXmlDocument();
      this.MessageSendToFurs.Schemas = this.Settings.Schemas;
      this.MessageSendToFurs.LoadXml(emptySoap);

      XmlNode body = this.MessageSendToFurs.ImportNode(this.Message.DocumentElement, true);
      XmlNode soapBody = XmlHelperFunctions.GetSubNode(this.MessageSendToFurs.DocumentElement, "soapenv:Body");
      soapBody.AppendChild(body);
    }

    public void Validate()
    {
      XmlHelperFunctions.Validate(this.MessageSendToFurs);
    }

    public void SendToFURS()
    {
      SendMessage sm = new SendMessage();
      this.MessageReceivedFromFurs = sm.Send(this.MessageSendToFurs, this.MessageType, this.Settings);
    }

    protected void checkRoot(MessageType expectedType)
    {
      if (this.MessageType != expectedType)
        throw new ArgumentOutOfRangeException("Unknown document");
    }

    protected void checkHeader()
    {
      XmlNode header = XmlHelperFunctions.GetSubNode(this.Message.DocumentElement, "fu:Header");
      if (header != null) return;

      XmlNode headerNode = XmlHelperFunctions.CreateElement(this.Message, this.Settings.FursXmlNamespace, "Header");
      headerNode.AppendChild(XmlHelperFunctions.CreateElement(this.Message, this.Settings.FursXmlNamespace, "MessageID", Guid.NewGuid().ToString()));
      headerNode.AppendChild(XmlHelperFunctions.CreateElement(this.Message, this.Settings.FursXmlNamespace, "DateTime", DateTime.Now.ToString("s")));

      this.Message.DocumentElement.InsertBefore(headerNode, this.Message.DocumentElement.FirstChild);
    }

    protected void executeSign()
    {
      this.signMessage.Sign(this.MessageSendToFurs, this.MessageType, this.Settings);
    }

    protected Settings Settings { get; set; }

    private SignMessage signMessage = new SignMessage();
  }
}