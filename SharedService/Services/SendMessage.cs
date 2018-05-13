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
    public static SendMessage Create(Settings settings)
    {
      return new SendMessage(settings);
    }

    public XmlDocument Send(XmlDocument message, MessageType messageType)
    {
      ServicePointManager.Expect100Continue = true;
      ServicePointManager.SecurityProtocol = SetCryptoConfig.SetTLSProtocol();
      ServicePointManager.ServerCertificateValidationCallback += new System.Net.Security.RemoteCertificateValidationCallback((sender, certificate, chain, sslPolicyErrors) => { return true; });

      HttpWebRequest request = this.createWebRequest(message, messageType);

      request.ClientCertificates.Add(this.settings.Certificate);
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
      HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(this.settings.FursWebServiceURL);

      webRequest.Headers.Add(this.soapActions[messageType]);
      webRequest.ContentType = "text/xml; charset=UTF-8";
      webRequest.Accept = "text/xml";
      webRequest.Method = "POST";
      webRequest.KeepAlive = true;

      webRequest.Timeout = webRequest.ReadWriteTimeout = this.settings.TimeOutInSec * 1000;

      return webRequest;
    }

    private SendMessage(Settings settings)
    {
      this.settings = settings;
      this.soapActions = new Dictionary<MessageType, string>();
      this.soapActions.Add(MessageType.Invoice, @"SOAPAction: /invoices");
      this.soapActions.Add(MessageType.BusinessPremise, @"SOAPAction: /invoices/register");
      this.soapActions.Add(MessageType.Echo, @"SOAPAction: /echo");
    }

    private Dictionary<MessageType, string> soapActions;
    private Settings settings;
  }
}