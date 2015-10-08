// <copyright file="SignMessage.cs" company="MNet">
//     Copyright (c) Matjaz Prtenjak All rights reserved.
// </copyright>
// <author>Matjaz Prtenjak</author>
//-----------------------------------------------------------------------

using System;
using System.Deployment.Internal.CodeSigning;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Xml;
using MNet.SLOTaxService.Messages;
using MNet.SLOTaxService.Utils;

namespace MNet.SLOTaxService.Services
{
  internal class SignMessage
  {
    public void Sign(XmlDocument message, MessageType messageType, Settings settings)
    {
      XmlNode mainNode = this.getMainNode(message, messageType);
      if (mainNode == null) return;

      CryptoConfig.AddAlgorithm(typeof(RSAPKCS1SHA256SignatureDescription), "http://www.w3.org/2001/04/xmldsig-more#rsa-sha256");

      SignedXml signedXml = new SignedXml(message);
      signedXml.SigningKey = settings.CryptoProvider;
      signedXml.AddReference(this.getReference(mainNode));
      signedXml.KeyInfo = this.getKeyInfo(settings);
      signedXml.SignedInfo.SignatureMethod = "http://www.w3.org/2001/04/xmldsig-more#rsa-sha256";
      signedXml.ComputeSignature();

      XmlElement xmlDigitalSignature = signedXml.GetXml();
      mainNode.AppendChild(xmlDigitalSignature);
    }

    private KeyInfo getKeyInfo(Settings settings)
    {
      X509Extension extension = settings.Certificate.Extensions[1];
      AsnEncodedData asndata = new AsnEncodedData(extension.Oid, extension.RawData);

      KeyInfoX509Data keyInfoData = new KeyInfoX509Data();
      keyInfoData.AddIssuerSerial(settings.Certificate.Issuer, settings.Certificate.SerialNumber);
      keyInfoData.AddSubjectName(settings.Certificate.SubjectName.Name);

      KeyInfo keyInfo = new KeyInfo();
      keyInfo.AddClause(keyInfoData);
      return keyInfo;
    }

    private Reference getReference(XmlNode mainNode)
    {
      Reference reference = new Reference();

      string mainNodeID = mainNode.Attributes["Id"].InnerText;
      reference.Uri = "#" + mainNodeID;
      reference.DigestMethod = @"http://www.w3.org/2001/04/xmlenc#sha256";
      reference.AddTransform(new XmlDsigEnvelopedSignatureTransform());

      return reference;
    }

    private XmlNode getMainNode(XmlDocument message, MessageType messageType)
    {
      switch (messageType)
      {
        case MessageType.Invoice:
          return XmlHelperFunctions.GetSubNode(message.DocumentElement, "fu:InvoiceRequest");

        case MessageType.BusinessPremise:
          return XmlHelperFunctions.GetSubNode(message.DocumentElement, "fu:BusinessPremiseRequest");
      }

      return null;
    }
  }
}