// <copyright file="Settings.cs" company="MNet">
//     Copyright (c) Matjaz Prtenjak All rights reserved.
// </copyright>
// <author>Matjaz Prtenjak</author>
//-----------------------------------------------------------------------

using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Xml;
using System.Xml.Schema;

namespace MNet.SLOTaxService.Services
{
  public class Settings
  {
    public string FursXmlNamespace { get; private set; }
    public string FursWebServiceURL { get; private set; }
    public XmlSchemaSet Schemas { get; private set; }
    public X509Certificate2 Certificate { get; private set; }

    public RSACryptoServiceProvider CryptoProvider
    {
      get
      {
        if (this.cryptoProvider == null)
          this.cryptoProvider = this.GetCryptoProvider();

        return this.cryptoProvider;
      }
    }

    public static Settings Create(X509Certificate2 certificate, string fursWebServiceURL, string fursXmlNamespace)
    {
      return new Settings(certificate, fursWebServiceURL, fursXmlNamespace);
    }

    public static Settings CreateTestSettings(X509Certificate2 certificate)
    {
      return new Settings(certificate, "https://blagajne-test.fu.gov.si:9002/v1/cash_registers", "http://www.fu.gov.si/");
    }

    public static Settings CreateProductionSettings(X509Certificate2 certificate)
    {
      return new Settings(certificate, "https://blagajne.fu.gov.si:9003/v1/cash_registers", "http://www.fu.gov.si/");
    }

    private Settings(X509Certificate2 certificate, string fursWebServiceURL, string fursXmlNamespace)
    {
      this.FursXmlNamespace = fursXmlNamespace;
      this.FursWebServiceURL = fursWebServiceURL;
      this.Certificate = certificate;

      this.createSchemas();
    }

    private RSACryptoServiceProvider GetCryptoProvider()
    {
      return MNet.SLOTaxService.Services.Certificates.getCryptoProvider(this.Certificate);
    }

    private void createSchemas()
    {
      this.Schemas = new XmlSchemaSet();

      XmlReaderSettings xrs = new XmlReaderSettings();
      xrs.DtdProcessing = DtdProcessing.Parse;
      this.Schemas.Add(this.FursXmlNamespace, XmlReader.Create(this.getFiscalVerificationSchema(), xrs));
      this.Schemas.Add("http://www.w3.org/2000/09/xmldsig#", XmlReader.Create(this.getXmlSigSchema(), xrs));
    }

    private Stream getFiscalVerificationSchema()
    {
      return this.getResource(@"FiscalVerificationSchema.xsd");
    }

    private Stream getXmlSigSchema()
    {
      return this.getResource(@"xmldsig-core-schema.xsd");
    }

    private Stream getResource(string name)
    {
      return typeof(Settings).Assembly.GetManifestResourceStream(@"MNet.SLOTaxService.Resources." + name);
    }

    private RSACryptoServiceProvider cryptoProvider = null;
  }
}