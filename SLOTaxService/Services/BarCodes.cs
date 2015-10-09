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
  internal class BarCodes
  {
    public static BarCodes Create(XmlDocument invoice)
    {
      return new BarCodes(invoice, 1);
    }

    public static BarCodes Create(XmlDocument invoice, int numberOfCode128Lines)
    {
      if (numberOfCode128Lines < 1) numberOfCode128Lines = 1;
      if (numberOfCode128Lines > 6) numberOfCode128Lines = 6;

      return new BarCodes(invoice, numberOfCode128Lines);
    }

    public string BarCodeValue { get; private set; }
    public string[] BarCode128Lines { get; private set; }

    private BarCodes(XmlDocument invoice, int numberOfCode128Lines)
    {
      this.invoice = invoice;

      XmlNode protectedIDNode = XmlHelperFunctions.GetSubNode(invoice.DocumentElement, "fu:ProtectedID");
      XmlNode taxNumberNode = XmlHelperFunctions.GetSubNode(invoice.DocumentElement, "fu:TaxNumber");
      XmlNode timeStampNode = XmlHelperFunctions.GetSubNode(invoice.DocumentElement, "fu:IssueDateTime");

      if ((protectedIDNode == null) || (taxNumberNode == null) || (timeStampNode == null))
        this.BarCodeValue = string.Empty;
      else
        this.BarCodeValue = BarCodesHelpers.GenerateCode(protectedIDNode.InnerText, taxNumberNode.InnerText, Convert.ToDateTime(timeStampNode));

      this.BarCode128Lines = BarCodesHelpers.SplitCode(this.BarCodeValue, numberOfCode128Lines);
    }

    private XmlDocument invoice;
  }
}
