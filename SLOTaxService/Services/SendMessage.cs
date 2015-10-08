// <copyright file="SendMessage.cs" company="MNet">
//     Copyright (c) Matjaz Prtenjak All rights reserved.
// </copyright>
// <author>Matjaz Prtenjak</author>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Xml;
using MNet.SLOTaxService.Messages;
using MNet.SLOTaxService.Utils;

namespace MNet.SLOTaxService.Services
{
  internal class SendMessage
  {
    public SendMessage()
    {
      this.soapActions = new Dictionary<MessageType, string>();
      this.soapActions.Add(MessageType.Invoice, @"SOAPAction: /invoices");
      this.soapActions.Add(MessageType.BusinessPremise, @"SOAPAction: /invoices/register");
      this.soapActions.Add(MessageType.Echo, @"SOAPAction: /echo");
    }

    public XmlDocument Send(XmlDocument message, MessageType messageType, Settings settings)
    {
      HttpWebRequest request = this.createWebRequest(message, messageType);

      ServicePointManager.Expect100Continue = true;
      ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
      ServicePointManager.ServerCertificateValidationCallback += new System.Net.Security.RemoteCertificateValidationCallback((sender, certificate, chain, sslPolicyErrors) => { return true; });

      request.ClientCertificates.Add(settings.Certificate);
      using (Stream stream = request.GetRequestStream())
      {
        StreamWriter sw = new StreamWriter(stream, new System.Text.UTF8Encoding(false, true));
        message.Save(sw);
      }

      XmlDocument result = XmlHelperFunctions.CreateNewXmlDocument();
      using (WebResponse response = request.GetResponse())
      {
        using (StreamReader rd = new StreamReader(response.GetResponseStream()))
        {
          string soapResult = rd.ReadToEnd();

          result.LoadXml(soapResult);
        }
      }

      return result;
    }

    public HttpWebRequest createWebRequest(XmlDocument message, MessageType messageType)
    {
      HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create("https://blagajne-test.fu.gov.si:9002/v1/cash_registers");

      webRequest.Headers.Add(this.soapActions[messageType]);
      webRequest.ContentType = "text/xml; charset=UTF-8";
      webRequest.Accept = "text/xml";
      webRequest.Method = "POST";
      webRequest.KeepAlive = true;

      return webRequest;
    }

    private Dictionary<MessageType, string> soapActions;
  }
}