// <copyright file="BarCodes.cs" company="MNet">
//     Copyright (c) Matjaz Prtenjak All rights reserved.
// </copyright>
// <author>Matjaz Prtenjak</author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using MNet.SLOTaxService.Utils;

namespace MNet.SLOTaxService.Services
{
  public class BarCodes
  {
    public static BarCodes Create(XmlDocument invoice)
    {
      return new BarCodes(invoice);
    }

    public string BarCodeValue { get; private set; }

    public string[] GetBarCode128Lines(int noLines)
    {
      return BarCodesHelpers.SplitCode(this.BarCodeValue, noLines);
    }

    private BarCodes(XmlDocument invoice)
    {
      this.invoice = invoice;

      XmlNode protectedIDNode = XmlHelperFunctions.GetSubNode(invoice.DocumentElement, "fu:ProtectedID");
      XmlNode taxNumberNode = XmlHelperFunctions.GetSubNode(invoice.DocumentElement, "fu:TaxNumber");
      XmlNode timeStampNode = XmlHelperFunctions.GetSubNode(invoice.DocumentElement, "fu:IssueDateTime");

      if ((protectedIDNode == null) || (taxNumberNode == null) || (timeStampNode == null))
        this.BarCodeValue = string.Empty;
      else
        this.BarCodeValue = BarCodesHelpers.GenerateCode(protectedIDNode.InnerText, taxNumberNode.InnerText, Convert.ToDateTime(timeStampNode));
    }

    private XmlDocument invoice;
  }
}
