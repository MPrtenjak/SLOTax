// <copyright file="NU_Base.cs" company="MNet">
//     Copyright (c) Matjaz Prtenjak All rights reserved.
// </copyright>
// <author>Matjaz Prtenjak</author>
//-----------------------------------------------------------------------

using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Xml;
using MNet.SLOTaxService.Services;
using MNet.SLOTaxService.Utils;

namespace MNet.SLOTaxService.UnitTests
{
  internal class NU_Base
  {
    protected string getFullFileName(string fileName)
    {
      string appPath = new Uri(Path.GetDirectoryName(typeof(NU_Certificate).Assembly.EscapedCodeBase)).LocalPath;
      string dataPath = Path.Combine(appPath, @"UnitTests\Data");

      return Path.Combine(dataPath, fileName);
    }

    protected XmlDocument getXml(string fileName)
    {
      string fullName = this.getFullFileName(fileName);

      XmlDocument xml = XmlHelperFunctions.CreateNewXmlDocument();
      xml.Load(fullName);

      // It is important to set the tax number to match the one that is in certificate or else FURS will not proceed messages
      XmlNode nodeTaxNumber = xml.GetElementsByTagName("fu:TaxNumber")[0];
      if (nodeTaxNumber != null) nodeTaxNumber.InnerText = this.myTaxNumber;

      return xml;
    }

    protected NU_Base()
    {
      this.myTaxNumber = "10129014";

      Certificates cert = new Certificates();
      X509Certificate2 certificate = cert.GetByTaxNumber(this.myTaxNumber);

      this.settings = Settings.CreateTestSettings(certificate);
      this.taxService = TaxService.Create(this.settings);
    }

    protected string myTaxNumber { get; set; }
    protected Settings settings { get; set; }
    protected TaxService taxService { get; set; }
  }
}