// <copyright file="SignMessage.cs" company="MNet">
//     Copyright (c) Matjaz Prtenjak All rights reserved.
// </copyright>
// <author>Matjaz Prtenjak</author>
//-----------------------------------------------------------------------

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
    public static SignMessage Create(Settings settings)
    {
      return new SignMessage(settings);
    }

    public void Sign(XmlDocument message, MessageType messageType)
    {
      XmlNode mainNode = this.getMainNode(message, messageType);
      if (mainNode == null) return;

      SetCryptoConfig.SetAlgorithm();

      SignedXml signedXml = new SignedXml(message);
      signedXml.SigningKey = this.settings.CryptoProvider;
      signedXml.AddReference(this.getReference(mainNode));
      signedXml.KeyInfo = this.getKeyInfo();
      signedXml.SignedInfo.SignatureMethod = "http://www.w3.org/2001/04/xmldsig-more#rsa-sha256";
      signedXml.ComputeSignature();

      XmlElement xmlDigitalSignature = signedXml.GetXml();
      mainNode.AppendChild(xmlDigitalSignature);
    }

    private KeyInfo getKeyInfo()
    {
      X509Extension extension = this.settings.Certificate.Extensions[1];
      AsnEncodedData asndata = new AsnEncodedData(extension.Oid, extension.RawData);

      KeyInfoX509Data keyInfoData = new KeyInfoX509Data();
      keyInfoData.AddIssuerSerial(this.settings.Certificate.Issuer, this.settings.Certificate.SerialNumber);
      keyInfoData.AddSubjectName(this.settings.Certificate.SubjectName.Name);

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

    private SignMessage(Settings settings)
    {
      this.settings = settings;
    }

    private Settings settings;
  }
}