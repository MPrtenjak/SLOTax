// <copyright file="BarCodes.cs" company="MNet">
//     Copyright (c) Matjaz Prtenjak All rights reserved.
// </copyright>
// <author>Matjaz Prtenjak</author>
//-----------------------------------------------------------------------

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Xml;
using Gma.QrCodeNet.Encoding;
using Gma.QrCodeNet.Encoding.Windows.Render;
using MNet.SLOTaxService.Modulo;
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

    public Image DrawQRCode(int qrImageSize, ImageFormat imgFormat)
    {
      QrEncoder qrEncoder = new QrEncoder(ErrorCorrectionLevel.M);
      QrCode qrCode = qrEncoder.Encode(this.BarCodeValue);

      GraphicsRenderer renderer = new GraphicsRenderer(new FixedCodeSize(qrImageSize, QuietZoneModules.Two), Brushes.Black, Brushes.White);
      MemoryStream stream = new MemoryStream();
      renderer.WriteToStream(qrCode.Matrix, imgFormat, stream);
      return Image.FromStream(stream);
    }

    private BarCodes(XmlDocument invoice)
    {
      // People at FURS say that modulo is just easy modulo 10 and not luhn!
      IModulo modulo = new Modulo10_Easy();
      this.invoice = invoice;

      XmlNode protectedIDNode = XmlHelperFunctions.GetSubNode(invoice.DocumentElement, "fu:ProtectedID");
      XmlNode taxNumberNode = XmlHelperFunctions.GetSubNode(invoice.DocumentElement, "fu:TaxNumber");
      XmlNode timeStampNode = XmlHelperFunctions.GetSubNode(invoice.DocumentElement, "fu:IssueDateTime");

      if ((protectedIDNode == null) || (taxNumberNode == null) || (timeStampNode == null))
        this.BarCodeValue = string.Empty;
      else
        this.BarCodeValue = BarCodesHelpers.GenerateCode(protectedIDNode.InnerText, taxNumberNode.InnerText, Convert.ToDateTime(timeStampNode.InnerText), modulo);
    }

    private XmlDocument invoice;
  }
}